using TMPro;
using UnityEngine;

public class DialogueTextManager : MonoBehaviour
{
	private TextMeshProUGUI dialogueTextArea;

	[SerializeField] private LocalisedText[] endingTexts;

	[Header("Parameters")]
	[SerializeField, Min(1)] private float talkingSpeed = 1;
	public bool automatic = true;

	private bool talking;
	private float newCharTimer;

	private LocalisedText currentText;

	private int frIndex;
	private int enIndex;
	private string currentStringFR;
	private string currentStringEN;
	private string tmpCloseTag;

	private void Awake()
	{
		dialogueTextArea = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		if (!talking) return;

		if (automatic)
		{
			if (newCharTimer <= 0)
			{
				UpdateText();
				newCharTimer = 1;
			}
			else newCharTimer -= Time.deltaTime * talkingSpeed;
		}
		else if (Input.GetKeyDown(KeyCode.Space)) UpdateText();
	}

	public void StartTalking(LocalisedText text, bool keepPrevText = false)
	{
		currentText = text;
		if(!keepPrevText)
		{
			dialogueTextArea.text = "";
			currentStringFR = "";
			currentStringEN = "";
		}
		else
		{
			currentStringFR += "\n";
			currentStringEN += "\n";
		}

		frIndex = 0;
		enIndex = 0;
		talking = true;
	}

	public void StartTalking(int endingCode) => StartTalking(endingTexts[endingCode], true);

	public void UpdateText()
	{
		bool finishedFR = false;
		bool finishedEN = false;

		if (frIndex < currentText.textFR.Length) WriteNewChar(true);
		else finishedFR = true;
		if (enIndex < currentText.textEN.Length) WriteNewChar(false);
		else finishedEN = true;

		if (finishedFR && finishedEN)
		{
			talking = false;
			GameManager.Instance.OnFinishedTalking();
		}
		else
		{
			string fr = currentStringFR + tmpCloseTag;
			dialogueTextArea.text = $"{fr}<size=50%>\n\n</size><i><size=80%>{currentStringEN}</size></i>";
		}

		SoundManager.Instance.PlaySound(SoundType.Talking);
	}

	public void WriteNewChar(bool fr)
	{
		string text = fr ? currentText.textFR : currentText.textEN;
		int index = fr ? frIndex : enIndex;
		int skipChar = 1;
		char c = text[index];
		string add = c.ToString();

		if (c == '<')
		{
			bool endTmpTag = false;
			while (text[index + skipChar] != '>')
			{
				if (text[index + skipChar] == '/') endTmpTag = true;
				++skipChar;
			}

			++skipChar;
			if (fr)
			{
				if (endTmpTag) tmpCloseTag = "";
				else tmpCloseTag = skipChar <= 3 ? "</b>" : "</color>";
			}

			add = text.Substring(index, skipChar);
		}

		if (fr)
		{
			currentStringFR += add;
			frIndex += skipChar;
		}
		else
		{
			currentStringEN += add;
			enIndex += skipChar;
		}
	}
}
