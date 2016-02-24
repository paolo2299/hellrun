using UnityEngine;
using System.Collections;

public class MusicSingleton : MonoBehaviour {

	public AudioSource audio;

	private static MusicSingleton instance = null;
	public static MusicSingleton Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			AudioSource existingAudio = instance.audio;;
			if (existingAudio.clip.name != audio.clip.name) {
				existingAudio.Stop();
				existingAudio.clip = audio.clip;
				existingAudio.Play();
			}
			Destroy(this.gameObject);
			Destroy(audio.gameObject);
			return;
		} else {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
			DontDestroyOnLoad(audio.gameObject);
			audio.Play();
		}
	}
}	
