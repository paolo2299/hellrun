using UnityEngine;
using System.Collections;

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
	
	// Update is called once per frame
	void Update () {
		if (!playerRecordingMode && (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Space))) {
			Application.LoadLevel("level_select");
		}
	}
}
