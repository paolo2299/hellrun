using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelComplete : MonoBehaviour {
	private GameProgress gameProgess;
	private LevelManager levelManager;
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
		gameProgess = GameProgress.Load ();
		if (sceneName != "level_complete") {   //Check that we are not loading level_complete as a standalone in the editor
			//Upper left panel
			var timeTaken = LevelManager.Instance.ElapsedTime ();
			var medalThisTime = gameProgess.GetMedalAttained (sceneName, timeTaken);
			var personalBest = gameProgess.GetLevelBestTime (sceneName);
			var personalBestMedal = gameProgess.GetMedalAttained (sceneName);
			var difference = timeTaken - personalBest;
			var parityString = (difference > 0) ? "+" : "-";
			differenceText.text = "(" + parityString + StopWatch.Format(difference) + ")";
			thisTime.text = StopWatch.Format (timeTaken);
			fastestTime.text = StopWatch.Format (personalBest);
			AssignMedalSprite(fastestTimeImage, personalBestMedal);
			AssignMedalSprite(thisTimeImage, medalThisTime);
			AssignMedalSprite(medalAwarded, personalBestMedal);

			//Upper right panel
			goldTimeText.text = StopWatch.Format(gameProgess.GetGoldMedalTime(sceneName));
			silverTimeText.text = StopWatch.Format(gameProgess.GetSilverMedalTime(sceneName));
			bronzeTimeText.text = StopWatch.Format(gameProgess.GetBronzeMedalTime(sceneName));
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
