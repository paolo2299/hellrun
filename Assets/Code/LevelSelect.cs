using UnityEngine;
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
	public Button level7Button;
	public Button level8Button;
	public Button level9Button;
	public Button level10Button;

	public Text selectText1;
	public Text selectText2;
	public Text selectText3;
	public Text selectText4;
	public Text selectText5;
	public Text selectText6;
	public Text selectText7;
	public Text selectText8;
	public Text selectText9;
	public Text selectText10;
	
	public Image selectMedal1;
	public Image selectMedal2;
	public Image selectMedal3;
	public Image selectMedal4;
	public Image selectMedal5;
	public Image selectMedal6;
	public Image selectMedal7;
	public Image selectMedal8;
	public Image selectMedal9;
	public Image selectMedal10;

	public Image padlock1;
	public Image padlock2;
	public Image padlock3;
	public Image padlock4;
	public Image padlock5;
	public Image padlock6;
	public Image padlock7;
	public Image padlock8;
	public Image padlock9;
	public Image padlock10;

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

		selectText1.text = GameStructure.GetLevelDisplayName ("level_" + world + "_1");
		selectText2.text = GameStructure.GetLevelDisplayName ("level_" + world + "_2");
		selectText3.text = GameStructure.GetLevelDisplayName ("level_" + world + "_3");
		selectText4.text = GameStructure.GetLevelDisplayName ("level_" + world + "_4");
		selectText5.text = GameStructure.GetLevelDisplayName ("level_" + world + "_5");
		selectText6.text = GameStructure.GetLevelDisplayName ("level_" + world + "_6");
		selectText7.text = GameStructure.GetLevelDisplayName ("level_" + world + "_7");
		selectText8.text = GameStructure.GetLevelDisplayName ("level_" + world + "_8");
		selectText9.text = GameStructure.GetLevelDisplayName ("level_" + world + "_9");
		selectText10.text = GameStructure.GetLevelDisplayName ("level_" + world + "_10");

		AssignMedalSprite (selectMedal1, _gameProgress.GetMedalAttained ("level_" + world + "_1"));
		AssignMedalSprite (selectMedal2, _gameProgress.GetMedalAttained ("level_" + world + "_2"));
		AssignMedalSprite (selectMedal3, _gameProgress.GetMedalAttained ("level_" + world + "_3"));
		AssignMedalSprite (selectMedal4, _gameProgress.GetMedalAttained ("level_" + world + "_4"));
		AssignMedalSprite (selectMedal5, _gameProgress.GetMedalAttained ("level_" + world + "_5"));
		AssignMedalSprite (selectMedal6, _gameProgress.GetMedalAttained ("level_" + world + "_6"));
		AssignMedalSprite (selectMedal7, _gameProgress.GetMedalAttained ("level_" + world + "_7"));
		AssignMedalSprite (selectMedal8, _gameProgress.GetMedalAttained ("level_" + world + "_8"));
		AssignMedalSprite (selectMedal9, _gameProgress.GetMedalAttained ("level_" + world + "_9"));
		AssignMedalSprite (selectMedal10, _gameProgress.GetMedalAttained ("level_" + world + "_10"));

		padlock1.enabled = false;

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_1")) {
			selectText2.text = "";
			selectMedal2.enabled = false;
			var navigation = level1Button.navigation;
			navigation.selectOnRight = null;
			level1Button.navigation = navigation;
			level2Button.interactable = false;
			padlock2.enabled = true;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_2")) {
			selectText3.text = "";
			selectMedal3.enabled = false;
			var navigation = level2Button.navigation;
			navigation.selectOnRight = null;
			level2Button.navigation = navigation;
			level3Button.interactable = false;
			padlock3.enabled = true;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_3")) {
			selectText4.text = "";
			selectMedal4.enabled = false;
			var navigation = level3Button.navigation;
			navigation.selectOnRight = null;
			level3Button.navigation = navigation;
			level4Button.interactable = false;
			padlock4.enabled = true;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_4")) {
			selectText5.text = "";
			selectMedal5.enabled = false;
			var navigation = level4Button.navigation;
			navigation.selectOnDown = null;
			level4Button.navigation = navigation;
			level5Button.interactable = false;
			padlock5.enabled = true;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_5")) {
			selectText6.text = "";
			selectMedal6.enabled = false;
			var navigation = level5Button.navigation;
			navigation.selectOnLeft = null;
			level5Button.navigation = navigation;
			level6Button.interactable = false;
			padlock6.enabled = true;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_6")) {
			selectText7.text = "";
			selectMedal7.enabled = false;
			var navigation = level6Button.navigation;
			navigation.selectOnLeft = null;
			level6Button.navigation = navigation;
			level7Button.interactable = false;
			padlock7.enabled = true;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_7")) {
			selectText8.text = "";
			selectMedal8.enabled = false;
			var navigation = level7Button.navigation;
			navigation.selectOnLeft = null;
			level7Button.navigation = navigation;
			level8Button.interactable = false;
			padlock8.enabled = true;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_8")) {
			selectText9.text = "";
			selectMedal9.enabled = false;
			var navigation = level8Button.navigation;
			navigation.selectOnLeft = null;
			level8Button.navigation = navigation;
			level9Button.interactable = false;
			padlock9.enabled = true;
		}

		if (!_gameProgress.GetLevelComplete ("level_" + world + "_9")) {
			selectText10.text = "";
			selectMedal10.enabled = false;
			var navigation = level9Button.navigation;
			navigation.selectOnLeft = null;
			level9Button.navigation = navigation;
			level10Button.interactable = false;
			padlock10.enabled = true;
		}
	}

	private void DisableLevel(string sceneName) {
		Debug.Log ("foo");
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
	
	public void LoadCurrentLevel(Button button) {
		var sceneName = "level_" + world + "_" + button.name;
		Application.LoadLevel (sceneName);
	}

	public void DisplayLevel(Button button) {
		var sceneName = "level_" + world + "_" + button.name;
		levelName.text = GameStructure.GetLevelDisplayName (sceneName);
		goldMedalTime.text = StopWatch.Format(GameStructure.GetGoldMedalTime(sceneName));
		silverMedalTime.text = StopWatch.Format(GameStructure.GetSilverMedalTime(sceneName));
		bronzeMedalTime.text = StopWatch.Format(GameStructure.GetBronzeMedalTime(sceneName));
		if (_gameProgress.GetLevelComplete (sceneName)) {
			personalBestText.text = StopWatch.Format (_gameProgress.GetLevelBestTime (sceneName));
			AssignMedalSprite(currentMedal, _gameProgress.GetMedalAttained(sceneName));
			playerImage.enabled = true;
		} else {
			personalBestText.text = "";
			currentMedal.enabled = false;
			playerImage.enabled = false;
		}
	}

	public void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.LoadLevel("main_menu");
		}
	}
}
