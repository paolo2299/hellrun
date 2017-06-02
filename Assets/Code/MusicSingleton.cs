using UnityEngine;
using System.Collections;

public class MusicSingleton : MonoBehaviour {

	public AudioSource theAudio;

	private static MusicSingleton instance = null;
	public static MusicSingleton Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			AudioSource existingAudio = instance.theAudio;
			if (existingAudio.clip.name != theAudio.clip.name) {
				existingAudio.Stop();
				existingAudio.clip = theAudio.clip;
				existingAudio.Play();
			}
			Destroy(this.gameObject);
			Destroy(theAudio.gameObject);
			return;
		} else {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
			DontDestroyOnLoad(theAudio.gameObject);
			theAudio.Play();
			if (theAudio.clip.name == "destoroya") { //TODO clip the mp3 instead
				theAudio.time = 13.3f;
			}
		}
	}
}	
