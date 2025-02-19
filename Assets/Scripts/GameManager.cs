using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using System;

public class GameManager : MonoBehaviour
{
	#region Singleton

	private static GameManager instance;
	public static GameManager Instance => instance;
	#endregion

	#region Attributs

	private const int MAX_ROUNDS = 9;

	#region Références

	#region Références d'assets

	[Header("Assets")]
	[SerializeField, SerializedDictionary("Devil states", "Sprites")]
	private SerializedDictionary<DevilState, Sprite> devilSprites;
	[SerializeField] private AnswerUI[] answerUIs;
	#endregion

	[Header("References")]
	[SerializeField] private EndScreenManager endScreenManager;
	[SerializeField] private DialogueTextManager dialogueTextManager;
	[SerializeField] private Image devilImg;
	#endregion

	private DevilState devilState;
	private int nbRounds;

	[SerializeField] private DialogueData currentDialogue;
	private AnswerData currentAnswer;

	private bool devilReacted = true;
	private bool end;
	private int endingCode = -1;
	#endregion

	private void Awake()
	{
		instance = this;
		NextDialogue(true);
	}

	public void AnswerSelected(int answerIndex)
	{
		foreach (AnswerUI ans in answerUIs) ans.gameObject.SetActive(false);

		currentAnswer = currentDialogue.answers[answerIndex];
		UpdateDevilState(currentAnswer.devilStateModifier);

		++nbRounds;
		endingCode = CheckEndGame();
		if (endingCode != 0)
		{
			end = true;
			dialogueTextManager.StartTalking(endingCode - 1);
			return;
		}

		dialogueTextManager.StartTalking(currentAnswer.devilReactionText);
		devilReacted = false;
	}

	public void OnFinishedTalking()
	{
		if(end)
		{
			endScreenManager.gameObject.SetActive(true);
			endScreenManager.ShowEndScreen(endingCode-1);
			return;
		}

		if (!devilReacted) 
		{
			NextDialogue(); 
			devilReacted = true;
		}
		else ShowAnswers();
	}

	private void NextDialogue(bool first = false)
	{
		if (!first) currentDialogue = currentAnswer.nextDialogue;
		dialogueTextManager.StartTalking(currentDialogue.text, !first);
	}

	private void ShowAnswers()
	{
		for (int i = 0; i < currentDialogue.answers.Length; ++i)
		{
			AnswerUI ans = answerUIs[i];
			ans.gameObject.SetActive(true);
			ans.SetLocalisedText(currentDialogue.answers[i].text);
		}
	}

	public void UpdateDevilState(int modifier)
	{
		int currentStateIndex = (int)devilState;
		int nextStateIndex = currentStateIndex + modifier;

		// Limits of the axis.
		if (nextStateIndex < -2) nextStateIndex = -2;
		if (nextStateIndex > 20) nextStateIndex = 20;

		print(nextStateIndex);

		if (!Enum.IsDefined(typeof(DevilState), nextStateIndex))
		{
			print("Undefined");

			// Going up the angry axis via love.
			if (currentStateIndex < 0 && modifier == 10) nextStateIndex = 0;

			// Going down the love axis.
			if (currentStateIndex % 10 == 0 && modifier == -1) nextStateIndex -= 9;

			// Transitions between love & friendly axis.
			if (nextStateIndex == 21) nextStateIndex = 1;
			if (nextStateIndex == 12) nextStateIndex = 10;
			if (nextStateIndex == 11) nextStateIndex -= currentStateIndex;

			if (nextStateIndex >= 3 && nextStateIndex < 10) nextStateIndex = 2;
		}

		devilState = (DevilState)nextStateIndex;
		//devilImg.sprite = devilSprites[devilState];

		// TMP Devil states' colors
		devilImg.color = devilState switch
		{
			DevilState.Interested => new Color(0.8f,0.6f,0.9f),
			DevilState.InLove => new Color(1,0,0.8f),
			DevilState.Happy => new Color(0.6f, 1, 0.6f),
			DevilState.Friendly => Color.green,
			DevilState.Angry => new Color(1, 0.6f, 0.6f),
			DevilState.Furious => Color.red,
			_ => Color.white

		};
	}

	public int CheckEndGame()
	{
		if (devilState == DevilState.Furious) return 1;

		if(nbRounds == MAX_ROUNDS)
		{
			if (devilState == DevilState.InLove) return 3;
			if (devilState == DevilState.Friendly) return 2;
			return 1;
		}

		return 0;
	}
}

public enum DevilState
{
	Neutral = 0,
	Angry = -1, Furious = -2,
	Happy = 1, Friendly = 2,
	Interested = 10, InLove = 20,
}