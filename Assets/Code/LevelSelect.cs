using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {
	public string chapterRef;

	public List<Button> levelButtons;
	public List<Text> selectTexts;
	public List<Image> selectMedals;
	public List<Image> padlocks;

	public Sprite goldMedal;
	public Sprite silverMedal;
	public Sprite bronzeMedal;
	public Image currentMedal;
	public Image playerImage;

	public Text goldMedalTime;
	public Text silverMedalTime;
	public Text bronzeMedalTime;

	public Text personalBestText;

	public Text levelName;

	private GameProgress _gameProgress;
	private int _selectedLevelIndex;
	private GameChapter _selectedChapter;

	// Use this for initialization
	void Start () {
		Time.timeScale = 1; //In case we arrived here from a paused state
		_gameProgress = GameProgress.Load ();

		var numLevels = levelButtons.Count;

		for (var i = 0; i < numLevels; i++) {
			padlocks [i].enabled = false;
		}

		for (var i = 0; i < numLevels; i++) {
			var sceneName = SceneNameFromIndex (i);
			if (!padlocks [i].enabled) {
				selectTexts [i].text = GameStructure.GetLevelDisplayName (sceneName);
				AssignMedalSprite (selectMedals [i], _gameProgress.GetMedalAttained (sceneName));
			}
			if (i < numLevels - 1 && !_gameProgress.GetLevelComplete (sceneName)) {
				LockLevel (i + 1);
			}
		}
	}

	private string SceneNameFromIndex(int i) {
		return GameStructure.GetSceneName (chapterRef, i);
	}

	private void LockLevel(int levelIndex) {
		selectTexts [levelIndex].text = "";
		selectMedals [levelIndex].enabled = false;
		DisableOnwardNavigation (levelIndex - 1);
		levelButtons [levelIndex].interactable = false;
		padlocks [levelIndex].enabled = true;
	}

	private void DisableOnwardNavigation(int levelIndex) {
		var navigation = levelButtons [levelIndex].navigation;
		switch (levelIndex) {
		case 0:
		case 1:
		case 2:
			navigation.selectOnRight = null;
			break;
		case 3:
			navigation.selectOnDown = null;
			break;
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
			navigation.selectOnLeft = null;
			break;
		}
		levelButtons [levelIndex].navigation = navigation;
	}

	private void AssignMedalSprite(Image image, string medal) {
		image.enabled = true;
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
		
	public void LoadLevel(int levelIndex) {
		var sceneName = SceneNameFromIndex (levelIndex);
		SceneManager.LoadScene (sceneName);
	}

	public void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			SceneManager.LoadScene ("main_menu");
		}
	}
}
