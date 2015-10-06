using UnityEngine;
using System.Collections;

public class LevelComplete : MonoBehaviour {
	private LevelProgress levelProgess;
	private LevelManager levelManager;
	public GUIText fastestTime;

	void Start () {
		levelProgess = GameData.Instance.GetLevelProgress (Application.loadedLevelName);
		fastestTime.text = "fastet time ever: " + StopWatch.Format(levelProgess.bestTime);
	}

	void Update () {
		if (Input.GetKey (KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevelName);
		} else if (Input.GetKey (KeyCode.Return)) {
			Application.LoadLevel(LevelManager.Instance.nextLevel);
		}
	}
}
