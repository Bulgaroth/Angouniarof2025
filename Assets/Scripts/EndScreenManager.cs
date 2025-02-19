using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenManager : MonoBehaviour
{
	[SerializeField] private Color[] backgroundColors;
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
		background.color = backgroundColors[endingCode];
		title.text = endTexts[endingCode].ToString();
	}
}
