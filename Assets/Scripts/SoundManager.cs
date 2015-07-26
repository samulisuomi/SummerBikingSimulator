using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioSource fxSource;
	public AudioSource musicSource;
	public static SoundManager instance = null;

	public float lowPitchRange = 1f;
	public float highPitchRange = 1f;

	// Use this for initialization
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}
	
	public void PlaySingle(AudioClip clip) {
		fxSource.clip = clip;
		fxSource.pitch = 1.0f;
		fxSource.Play();
	}

	public void RandomiseSfx(params AudioClip [] clips) {
		int randomIndex = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(lowPitchRange, highPitchRange);

		fxSource.pitch = randomPitch;
		fxSource.clip = clips[randomIndex];
		fxSource.Play();
	}
}
