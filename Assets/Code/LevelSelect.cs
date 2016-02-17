using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

	public string world;

	private GameProgress _gameProgress;
	private int _selectedLevelIndex;
	private GameChapter _selectedChapter;

	// Use this for initialization
	void Start () {
		_gameProgress = GameProgress.Load ();
		//TODO can we set medal icons from here?
	}

	public void LoadCurrentLevel(Button button) {
		var sceneName = "level_" + world + "_" + button.name;
		Application.LoadLevel (sceneName);
	}
}
