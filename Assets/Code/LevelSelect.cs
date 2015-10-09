using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {

	public GUIText selectedLevelText;

	private GameProgress _gameProgress;
	private GameLevel _selectedLevel;
	private int _selectedLevelIndex;
	private GameChapter _selectedChapter;
	
	public void Start () {
		_gameProgress = GameProgress.Load ();
		_selectedChapter = _gameProgress.chapters [0];
		_selectedLevelIndex = 0;
		_selectedLevel = _selectedChapter.levels [_selectedLevelIndex];
		SetSelectedLevelText ();
	}

	public void Update () {
		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Space)) {
			LoadCurrentLevel ();
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			IncrementLevel ();
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			DecrementLevel();
		}
	}
	
	private void SetSelectedLevelText () {
		selectedLevelText.text = SelectedLevelText();
	}

	private string SelectedLevelText () {
		var text = "";
		if (!_selectedLevel.firstLevelInChapter) {
			text += "< ";
		}
		text += _selectedLevel.name;
		if (!_selectedLevel.lastLevelInChapter) {
			text += " >";
		}
		return text;
	}
	
	private void IncrementLevel () {
		if (!_selectedLevel.lastLevelInChapter) {
			_selectedLevelIndex += 1;
			_selectedLevel = _selectedChapter.levels [_selectedLevelIndex];
			SetSelectedLevelText ();
		}
	}
	
	private void DecrementLevel () {
		if (!_selectedLevel.firstLevelInChapter) {
			_selectedLevelIndex -= 1;
			_selectedLevel = _selectedChapter.levels [_selectedLevelIndex];
			SetSelectedLevelText ();
		}
	}
	
	private void LoadCurrentLevel() {
		Application.LoadLevel (_selectedLevel.sceneName);
	}
}
