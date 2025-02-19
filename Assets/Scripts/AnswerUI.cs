using TMPro;
using UnityEngine;

public class AnswerUI : MonoBehaviour
{
	[SerializeField] private int number;
	private TextMeshProUGUI textArea;

	private void Awake()
	{
		textArea = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void SetLocalisedText(LocalisedText text)
	{
		textArea.text = text.ToString();
	}

	public void OnChoose() { GameManager.Instance.AnswerSelected(number); }
}
