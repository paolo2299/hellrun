using UnityEngine;
using System.Collections;

public class LevelComplete : MonoBehaviour {
	private GameProgress gameProgess;
	private LevelManager levelManager;
	public GUIText fastestTime;

	void Start () {
		gameProgess = GameProgress.Load ();
		fastestTime.text = "fastet time ever: " + StopWatch.Format(gameProgess.GetLevelBestTime(Application.loadedLevelName));
	}

	void Update () {
		if (Input.GetKey (KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevelName);
		} else if (Input.GetKey (KeyCode.Return)) {
			Application.LoadLevel(gameProgess.GetNextSceneName(Application.loadedLevelName));
		}
	}
}
