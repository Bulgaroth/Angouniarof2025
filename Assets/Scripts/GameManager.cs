using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.Collections;

public class GameManager : MonoBehaviour
{
	#region Singleton

	private static GameManager instance;
	public static GameManager Instance => instance;
	#endregion

	#region Attributs

	#region Références

	#region Références d'assets

	[Header("Assets")]
	[SerializeField, SerializedDictionary("Devil states", "Devil sprites")]
	private SerializedDictionary<DevilState, Sprite> devilSprites;
	[SerializeField, SerializedDictionary("Devil states", "Backgrounds")]
	private SerializedDictionary<DevilState, Sprite> backgroundSprites;
	[SerializeField] private AnswerUI[] answerUIs;
	#endregion

	[Header("References")]
	[SerializeField] private EndScreenManager endScreenManager;
	[SerializeField] private DialogueTextManager dialogueTextManager;
	[SerializeField] private Image backgroundImg;
	[SerializeField] private Image devilImg;
	[SerializeField] private DialogueData startingDialogue;
	#endregion

	private DevilState devilState;

	private DialogueData currentDialogue;
	private AnswerData currentAnswer;
	private List<AnswerData> shuffledAnswers;

	private bool devilReacted = true;
	private bool end;
	private int endingCode = -1;
	private int lastModifier = 0;
	#endregion

	#region API d'Unity

	private void Awake() => instance = this;

	private void Start() => Restart();
	#endregion

	public void AnswerSelected(int answerIndex)
	{
		foreach (AnswerUI ans in answerUIs) ans.gameObject.SetActive(false);

		currentAnswer = shuffledAnswers[answerIndex];
		UpdateDevilState(currentAnswer.devilStateModifier);
		SoundManager.Instance.PlaySound(SoundType.Answered);

		endingCode = CheckEndGame();
		if (endingCode != 0)
		{
			end = true;
			StartCoroutine(DelayedTalking(endingCode - 1));
			return;
		}

		devilReacted = false;
		StartCoroutine(DelayedTalking(currentAnswer.devilReactionText));
	}

	public void OnFinishedTalking()
	{
		if (end)
		{
			endScreenManager.gameObject.SetActive(true);
			endScreenManager.ShowEndScreen(endingCode - 1);
			return;
		}

		if (!devilReacted)
		{
			NextDialogue();
			devilReacted = true;
		}
		else ShowAnswers();
	}

	public void Restart()
	{
		end = false;
		endScreenManager.gameObject.SetActive(false);

		devilState = DevilState.Neutral;
		UpdateVisuals();

		currentDialogue = startingDialogue;
		NextDialogue(true);

		//SoundManager.Instance.PlaySound(SoundType.Start);
	}

	private void NextDialogue(bool first = false)
	{
		if (!first) currentDialogue = currentAnswer.nextDialogue;
		dialogueTextManager.StartTalking(currentDialogue.text, !first);
	}

	private void ShowAnswers()
	{
		// Shuffle answers.
		shuffledAnswers = new(currentDialogue.answers);
		int n = shuffledAnswers.Count;
		while (n > 1)
		{
			n--;
			int k = Random.Range(0, shuffledAnswers.Count);
			(shuffledAnswers[n], shuffledAnswers[k]) = (shuffledAnswers[k], shuffledAnswers[n]);
		}

		for (int i = 0; i < currentDialogue.answers.Length; ++i)
		{
			AnswerUI ans = answerUIs[i];
			ans.gameObject.SetActive(true);
			ans.SetLocalisedText(shuffledAnswers[i].text);
		}
	}

	private void UpdateDevilState(int modifier)
	{
		int currentStateIndex = (int)devilState;
		int nextStateIndex = currentStateIndex + modifier;
		lastModifier = modifier;

		// Limits of the axis.
		if (nextStateIndex < -2) nextStateIndex = -2;
		if (nextStateIndex >= 30) nextStateIndex = 20;

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

			print($"Redefinition : {nextStateIndex}");
		}

		devilState = (DevilState)nextStateIndex;
		
	}

	private void UpdateVisuals()
	{
		devilImg.sprite = devilSprites[devilState];
		/* devilImg.color = devilState switch
		{
				DevilState.Interested => new Color(0.8f, 0.6f, 0.9f),
				DevilState.InLove => new Color(1, 0, 0.8f),
				DevilState.Happy => new Color(0.6f, 1, 0.6f),
				DevilState.Friendly => Color.green,
				DevilState.Angry => new Color(1, 0.6f, 0.6f),
				DevilState.Furious => Color.red,
				_ => Color.white
		};*/

		SoundManager.Instance.PlaySound(lastModifier switch
		{
			-1 => SoundType.DevilStateAngry,
			10 => SoundType.DevilStateInLove,
			_ => SoundType.DevilStateHappy
		});

		backgroundImg.sprite = backgroundSprites[devilState];
	}

	private int CheckEndGame()
	{
		if (devilState == DevilState.Furious) return 1;

		//WIP
		//if (devilState == DevilState.InLove) return 3;
		//if (devilState == DevilState.Friendly) return 2;

		if(currentAnswer.nextDialogue == null)
		{
			if (devilState == DevilState.InLove) return 3;
			if (devilState == DevilState.Friendly) return 2;
			return 1;
		}

		return 0;
	}

	private IEnumerator DelayedTalking(LocalisedText text, bool keepPrevText = false)
	{
		yield return new WaitForSeconds(0.5f);
		dialogueTextManager.StartTalking(text, keepPrevText);
		UpdateVisuals();
	}

	private IEnumerator DelayedTalking(int endingCode)
	{
		yield return new WaitForSeconds(0.5f);
		dialogueTextManager.StartTalking(endingCode);
		UpdateVisuals();
	}
}

public enum DevilState
{
	Neutral = 0,
	Angry = -1, Furious = -2,
	Happy = 1, Friendly = 2,
	Interested = 10, InLove = 20,
}