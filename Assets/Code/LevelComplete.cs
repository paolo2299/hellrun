using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelComplete : MonoBehaviour {
	private GameProgress gameProgess;
	private LevelManager levelManager;
	public Text thisTime;
	public Text fastestTime;
	public Image fastestTimeImage;

	public Sprite goldMedal;
	public Sprite silverMedal;
	public Sprite bronzeMedal;

	void Start () {
		var sceneName = Application.loadedLevelName;
		gameProgess = GameProgress.Load ();
		var timeTaken = LevelManager.Instance.ElapsedTime ();
		var medalThisTime = gameProgess.GetMedalAttained (sceneName, timeTaken);
		var personalBest = gameProgess.GetLevelBestTime (sceneName);
		var personalBestMedal = gameProgess.GetMedalAttained (sceneName);
		if (Application.loadedLevelName != "level_complete") {
			thisTime.text = StopWatch.Format (timeTaken);
			fastestTime.text = StopWatch.Format (personalBest);
			AssignMedalSprite(fastestTimeImage, personalBestMedal);
		}
	}

	private void AssignMedalSprite(Image image, string medal) {
		if (medal == "bronze") {  //TODO case statement instead
			image.sprite = bronzeMedal;
		} else if (medal == "silver") {
			image.sprite = silverMedal;
		} else if (medal == "gold") {
			image.sprite = goldMedal;
		} else {
			image.sprite = null;
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
