using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Scriptable Objects/DialogueData")]
public class DialogueData : ScriptableObject
{
	public LocalisedText text;

	public AnswerData[] answers;

	public void AddDevilReaction(LocalisedText devilReaction)
	{
		text.textFR = $"{devilReaction.textFR}\n{text.textFR}";
		text.textEN = $"{devilReaction.textEN}\n{text.textEN}";
	}
}
