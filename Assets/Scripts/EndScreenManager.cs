using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
	[SerializeField] private Sprite[] backgroundSprites;
	[SerializeField] private LocalisedText[] endTexts;

	private Image background;
	private TextMeshProUGUI title;

	private void Awake()
	{
		background = GetComponentInChildren<Image>();
		title = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void ShowEndScreen(int endingCode)
	{
		RectTransform bgTransform = background.transform as RectTransform;
		Vector2 size = bgTransform.sizeDelta;
		size.y = endingCode == 0 ? 900 : 525;
		bgTransform.sizeDelta = size;

		background.sprite = backgroundSprites[endingCode];
		title.text = endTexts[endingCode].ToString();
	}
}
