using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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
		var sceneName = LevelManagerSingleton.Instance.loadedScene;
		gameProgress = GameProgress.Load ();
		if (sceneName != "level_complete") {   //Check that we are not loading level_complete as a standalone in the editor
			//Upper left panel
			var timeTaken = LevelManagerSingleton.Instance.ElapsedTime ();
			var medalThisTime = GameStructure.GetMedalAttained (sceneName, timeTaken);
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
			goldTimeText.text = StopWatch.Format(GameStructure.GetGoldMedalTime(sceneName));
			silverTimeText.text = StopWatch.Format(GameStructure.GetSilverMedalTime(sceneName));
			bronzeTimeText.text = StopWatch.Format(GameStructure.GetBronzeMedalTime(sceneName));
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
			LevelManagerSingleton.Instance.ResetLevel ();
		} else if (Input.GetKey (KeyCode.Return)) {
			if (GameStructure.GetIsLastLevelInChapter (LevelManagerSingleton.Instance.loadedScene)) {
				SceneManager.LoadScene ("level_select");
			} else {
				Debug.Log ("Attempting to load next level");
				Debug.Log ("Next level is " + GameStructure.GetNextSceneName (LevelManagerSingleton.Instance.loadedScene));
				LevelManagerSingleton.Instance.LoadLevel(GameStructure.GetNextSceneName (LevelManagerSingleton.Instance.loadedScene));
			}
		} else if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene ("level_select");
		}
	}
}
