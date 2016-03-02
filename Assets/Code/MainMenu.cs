using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	//set all of these when making a player recording
	public bool playerRecordingMode = false;
	public Player player;

	// Use this for initialization
	void Start () {
		if (playerRecordingMode) {
			player.CollectGrapple ();
		}
	}

	public void ButtonSelected(Button button) {
		var text = button.GetComponentInChildren<Text> ();
		text.color = new Color (1f, 1f, 1f, 1f);
	}

	public void ButtonDeselected(Button button) {
		var text = button.GetComponentInChildren<Text> ();
		text.color = new Color (0.412f, 0.412f, 0.412f, 1f);
	}

	public void StartGame(Button button) {
		var text = button.GetComponentInChildren<Text> ();
		text.color = new Color (1f, 1f, 0f, 1f);
		Application.LoadLevel ("level_select");
	}

	public void QuitGame(Button button) {
		var text = button.GetComponentInChildren<Text> ();
		text.color = new Color (1f, 1f, 0f, 1f);
		Application.Quit ();
	}
}
