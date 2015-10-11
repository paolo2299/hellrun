using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour {

	public Text levelNameText;
	public Image leftArrow;
	public Image rightArrow;

	public Sprite leftArrowLight;
	public Sprite leftArrowDark;
	public Sprite rightArrowLight;
	public Sprite rightArrowDark;

	private GameProgress _gameProgress;
	private GameLevel _selectedLevel;
	private int _selectedLevelIndex;
	private GameChapter _selectedChapter;

	// Use this for initialization
	void Start () {
		_gameProgress = GameProgress.Load ();
		_selectedChapter = _gameProgress.chapters [0];
		_selectedLevelIndex = 0;
		_selectedLevel = _selectedChapter.levels [_selectedLevelIndex];
		SetSelectedLevelText ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Space)) {
			LoadCurrentLevel ();
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			IncrementLevel ();
		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			DecrementLevel();
		}
	}

	private void SetSelectedLevelText () {
		levelNameText.text = _selectedLevel.name;
		if (_selectedLevel.firstLevelInChapter) {
			leftArrow.sprite = leftArrowDark;
		} else {
			leftArrow.sprite = leftArrowLight;
		}
		if (_selectedLevel.lastLevelInChapter) {
			rightArrow.sprite = rightArrowDark;
		} else {
			rightArrow.sprite = rightArrowLight;
		}
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
