﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

	public string world;

	public Button level1Button;
	public Button level2Button;
	public Button level3Button;
	public Button level4Button;
	public Button level5Button;
	public Button level6Button;

	public Text selectText1;
	public Text selectText2;
	public Text selectText3;
	public Text selectText4;
	public Text selectText5;
	public Text selectText6;
	
	public Image selectMedal1;
	public Image selectMedal2;
	public Image selectMedal3;
	public Image selectMedal4;
	public Image selectMedal5;
	public Image selectMedal6;

	public Sprite goldMedal;
	public Sprite silverMedal;
	public Sprite bronzeMedal;

	private GameProgress _gameProgress;
	private int _selectedLevelIndex;
	private GameChapter _selectedChapter;

	// Use this for initialization
	void Start () {
		_gameProgress = GameProgress.Load ();

		selectText1.text = _gameProgress.GetLevelName ("level_" + world + "_1");
		selectText2.text = _gameProgress.GetLevelName ("level_" + world + "_2");
		selectText3.text = _gameProgress.GetLevelName ("level_" + world + "_3");
		selectText4.text = _gameProgress.GetLevelName ("level_" + world + "_4");
		selectText5.text = _gameProgress.GetLevelName ("level_" + world + "_5");
		selectText6.text = _gameProgress.GetLevelName ("level_" + world + "_6");

		AssignMedalSprite (selectMedal1, _gameProgress.GetMedalAttained ("level_" + world + "_1"));
		AssignMedalSprite (selectMedal2, _gameProgress.GetMedalAttained ("level_" + world + "_2"));
		AssignMedalSprite (selectMedal3, _gameProgress.GetMedalAttained ("level_" + world + "_3"));
		AssignMedalSprite (selectMedal4, _gameProgress.GetMedalAttained ("level_" + world + "_4"));
		AssignMedalSprite (selectMedal5, _gameProgress.GetMedalAttained ("level_" + world + "_5"));
		AssignMedalSprite (selectMedal6, _gameProgress.GetMedalAttained ("level_" + world + "_6"));

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_1")) {
			selectText2.text = "";
			selectMedal2.enabled = false;
			var navigation = level1Button.navigation;
			navigation.selectOnRight = null;
			level1Button.navigation = navigation;
			level2Button.interactable = false;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_2")) {
			selectText3.text = "";
			selectMedal3.enabled = false;
			var navigation = level2Button.navigation;
			navigation.selectOnRight = null;
			level2Button.navigation = navigation;
			level3Button.interactable = false;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_3")) {
			selectText4.text = "";
			selectMedal4.enabled = false;
			var navigation = level3Button.navigation;
			navigation.selectOnRight = null;
			level3Button.navigation = navigation;
			level4Button.interactable = false;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_4")) {
			selectText5.text = "";
			selectMedal5.enabled = false;
			var navigation = level4Button.navigation;
			navigation.selectOnDown = null;
			level4Button.navigation = navigation;
			level5Button.interactable = false;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_5")) {
			selectText6.text = "";
			selectMedal6.enabled = false;
			var navigation = level5Button.navigation;
			navigation.selectOnLeft = null;
			level5Button.navigation = navigation;
			level6Button.interactable = false;
		}
	}

	private void DisableLevel(string sceneName) {
		Debug.Log ("foo");
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
			image.enabled = false;
		}
	}
	
	public void LoadCurrentLevel(Button button) {
		var sceneName = "level_" + world + "_" + button.name;
		Application.LoadLevel (sceneName);
	}
}
