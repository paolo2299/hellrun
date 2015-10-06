using UnityEngine;
using System.Collections;

public class LevelComplete : MonoBehaviour {
	private LevelProgress levelProgess;
	private LevelManager levelManager;
	public GUIText fastestTime;

	// Use this for initialization
	void Start () {
		levelProgess = GameData.Instance.GetLevelProgress (Application.loadedLevelName);
		levelManager = LevelManager.Instance;
		fastestTime.text = "fastet time ever: " + levelProgess.bestTime.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevelName);
		} else if (Input.GetKey (KeyCode.Return)) {
			Application.LoadLevel(levelManager.nextLevel);
		}
	}
}
