using UnityEngine;
using AYellowpaper.SerializedCollections;

public class SoundManager : MonoBehaviour
{
	#region Singleton

	private static SoundManager instance;
	public static SoundManager Instance => instance;
	#endregion

	[SerializeField] private AudioClip[] musics;

	[SerializeField, SerializedDictionary("Sound types", "Audio clips")] 
	private SerializedDictionary<SoundType, AudioClip[]> sounds;

	private readonly AudioSource[] audioSources = new AudioSource[4];

	private void Awake()
	{
		instance = this;

		for(int i=0; i<transform.childCount; ++i)
			audioSources[i] = transform.GetChild(i).GetComponent<AudioSource>();
	}

	public void PlaySound(SoundType soundType, int index = -1)
	{
		var selectedSounds = sounds[soundType];
		if (index == -1) index = Random.Range(0, selectedSounds.Length);

		int aSIndex = soundType switch
		{
			SoundType.DevilStateAngry or SoundType.DevilStateInLove or SoundType.DevilStateHappy => 0,
			SoundType.Talking => 1,
			_=> 2
		};

		audioSources[aSIndex].PlayOneShot(selectedSounds[index]);
	}

	public void ChangeMusic(bool intro)
	{
		AudioSource aS = audioSources[^1];
		aS.clip = musics[intro ? 0 : 1];
		aS.time = 0;
		aS.Play();
	}
}

public enum SoundType
{
	DevilStateAngry, DevilStateHappy, DevilStateInLove,
	Talking,
	Answered,
	WinEnd, LoseEnd, LoveEnd,
}
