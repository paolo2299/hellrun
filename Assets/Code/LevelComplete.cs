using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelComplete : MonoBehaviour {
	private GameProgress gameProgress;
	public Text thisTime;
	public Image thisTimeImage;
	public Text fastestTime;
	public Image fastestTimeImage;

	public Text goldTimeText;
	public Text silverTimeText;
	public Text bronzeTimeText;
	public Text differenceText;

	public Image medalAwarded;

	public Sprite goldMedal;
	public Sprite silverMedal;
	public Sprite bronzeMedal;

	void Start () {
		var sceneName = Application.loadedLevelName;
		gameProgress = GameProgress.Load ();
		if (sceneName != "level_complete") {   //Check that we are not loading level_complete as a standalone in the editor
			//Upper left panel
			var timeTaken = LevelManager.Instance.ElapsedTime ();
			var medalThisTime = gameProgress.GetMedalAttained (sceneName, timeTaken);
			var personalBest = gameProgress.GetLevelBestTime (sceneName);
			var personalBestMedal = gameProgress.GetMedalAttained (sceneName);
			var difference = timeTaken - personalBest;
			var parityString = (difference > 0) ? "+" : "-";
			differenceText.text = "(" + parityString + StopWatch.Format(difference) + ")";
			thisTime.text = StopWatch.Format (timeTaken);
			fastestTime.text = StopWatch.Format (personalBest);
			AssignMedalSprite(fastestTimeImage, personalBestMedal);
			AssignMedalSprite(thisTimeImage, medalThisTime);
			AssignMedalSprite(medalAwarded, personalBestMedal);

			//Upper right panel
			goldTimeText.text = StopWatch.Format(gameProgress.GetGoldMedalTime(sceneName));
			silverTimeText.text = StopWatch.Format(gameProgress.GetSilverMedalTime(sceneName));
			bronzeTimeText.text = StopWatch.Format(gameProgress.GetBronzeMedalTime(sceneName));
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
			image.enabled = false;
		}
	}
	
	void Update () {
		if (Input.GetKey (KeyCode.R)) {
			LevelManager.Instance.Reset ();
		} else if (Input.GetKey (KeyCode.Return)) {
			if (gameProgress.GetIsLastLevelInChapter (Application.loadedLevelName)) {
				Application.LoadLevel ("level_select");
			} else {
				Application.LoadLevel (gameProgress.GetNextSceneName (Application.loadedLevelName));
			}
		} else if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel ("level_select");
		}
	}
}
