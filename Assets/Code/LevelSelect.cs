using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

	public string world;

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
		//TODO can we set medal icons from here?
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
	
	public void LoadCurrentLevel(Button button) {
		var sceneName = "level_" + world + "_" + button.name;
		Application.LoadLevel (sceneName);
	}
}
