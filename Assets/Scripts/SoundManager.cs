using AYellowpaper.SerializedCollections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	#region Singleton

	private static SoundManager instance;
	public static SoundManager Instance => instance;
	#endregion

	[SerializeField, SerializedDictionary("Sound types", "Audio clips")] 
	private SerializedDictionary<SoundType, AudioClip[]> sounds;

	private readonly AudioSource[] audioSources = new AudioSource[3];

	private void Awake()
	{
		instance = this;

		for(int i=0; i<3; ++i)
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
}

public enum SoundType
{
	DevilStateAngry, DevilStateHappy, DevilStateInLove,
	Talking,
	Answered,
	WinEnd, LoseEnd, LoveEnd,
	Start
}
