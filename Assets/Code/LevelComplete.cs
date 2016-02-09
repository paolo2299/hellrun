using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelComplete : MonoBehaviour {
	private GameProgress gameProgess;
	private LevelManager levelManager;
	public Text fastestTime;
	public Text thisTime;

	void Start () {
		gameProgess = GameProgress.Load ();
		if (Application.loadedLevelName != "level_complete") {
			fastestTime.text = StopWatch.Format (gameProgess.GetLevelBestTime (Application.loadedLevelName));
			thisTime.text = StopWatch.Format (LevelManager.Instance.ElapsedTime());
		}
	}

	void Update () {
		if (Input.GetKey (KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevelName);
		} else if (Input.GetKey (KeyCode.Return)) {
			if (gameProgess.GetIsLastLevelInChapter(Application.loadedLevelName)) {
				Application.LoadLevel("level_select");
			} else {
				Application.LoadLevel(gameProgess.GetNextSceneName(Application.loadedLevelName));
			}
		}
	}
}
