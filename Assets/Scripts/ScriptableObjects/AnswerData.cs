using UnityEngine;

[CreateAssetMenu(fileName = "AnswerData", menuName = "Scriptable Objects/AnswerData")]
public class AnswerData : ScriptableObject
{
	public LocalisedText text;
	public LocalisedText devilReactionText;

	public DialogueData nextDialogue;

	public int devilStateModifier;
}
