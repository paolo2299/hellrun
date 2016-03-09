﻿using UnityEngine;
using System.Collections;

public class LevelManagerSingleton : MonoBehaviour {

	private static LevelManagerSingleton instance = null;
	public static LevelManagerSingleton Instance {
		get { return instance; }
	}
	
	public Player player;
	public GUIText elapsedTimeText;
	public GUIText levelNameText;
	public string nextLevel;
	private StopWatch stopWatchThisTry;
	private StopWatch stopWatchSinceLevelStart;
	private GameProgress gameProgress;
	private string levelName;
	
	private bool _paused = false;
	private bool _complete = false;
	public string loadedScene;
	
	public void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}

		gameProgress = GameProgress.Load();
		loadedScene = Application.loadedLevelName;
		Reset ();
	}
	
	public void Reset() {
		Time.timeScale = 1;
		player = GameObject.FindObjectOfType<Player> ();
		stopWatchThisTry = new StopWatch ();
		stopWatchSinceLevelStart = new StopWatch ();
		levelName = gameProgress.GetLevelName(loadedScene);
	}
	
	public void LevelComplete() {
		stopWatchThisTry.Stop ();
		player.Disable ();
		SaveLevelData ();
		_complete = true;
		
		Application.LoadLevelAdditive ("level_complete");
	}
	
	public void LoadLevel(string sceneNameToLoad) {
		var destroyables = GameObject.FindGameObjectsWithTag("Destroyable");
		foreach (GameObject destroyable in destroyables) {
			GameObject.Destroy (destroyable);
		}
		Application.LoadLevelAdditive (sceneNameToLoad);
		loadedScene = sceneNameToLoad;
	}
	
	public float ElapsedTime() {
		return stopWatchThisTry.time;
	}
	
	private void Pause () {
		Time.timeScale = 0;
		Application.LoadLevelAdditive ("pause");
		_paused = true;
	}
	
	private void Unpause () {
		Time.timeScale = 1;
		var pauseObject = GameObject.Find ("Pause");
		Destroy (pauseObject);
		_paused = false;
	}
	
	public bool Paused () {
		return _paused;
	}
	
	public bool Complete() {
		return _complete;
	}
	
	private void SaveLevelData() {
		var alreadyComplete = gameProgress.GetLevelComplete (Application.loadedLevelName);
		var bestTime = gameProgress.GetLevelBestTime (Application.loadedLevelName);
		if (!alreadyComplete || bestTime > stopWatchThisTry.time) {
			gameProgress.SetLevelBestTime(Application.loadedLevelName, stopWatchThisTry.time);
		}
		gameProgress.SetLevelComplete (Application.loadedLevelName, true);
	}
	
	public void ResetLevel() {
		LoadLevel (loadedScene);
		Reset ();
	}
	
	public void KillPlayer() {
		StartCoroutine (KillPlayerCo());
	}
	
	private IEnumerator KillPlayerCo() {
		player.Die ();
		yield return new WaitForSeconds (0.5f);
		ResetLevel ();
	}
	
	public bool PlayerHasPermanentGrapple() {
		return gameProgress.hasPermanentGrapple;
	}
	
	public void CollectPermanentGrapple() {
		gameProgress.CollectPermanentGrapple ();
	}
	
	public void LateUpdate() {
		elapsedTimeText.text = stopWatchThisTry.formattedTime;
		if (stopWatchSinceLevelStart.time > 5f) {
			levelNameText.enabled = false;
		} else {
			levelNameText.text = levelName;
		}
		
		if (Input.GetKeyDown (KeyCode.R) && Paused()) {
			ResetLevel ();
			return;
		}
		
		if (Input.GetKeyDown (KeyCode.P)) {
			if (_paused) {
				Unpause();
			} else {
				Pause ();
			}
			return;
		}
		
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (!Paused() && !Complete()) {
				Pause ();
				return;
			}
		}
		
		if (Input.GetKeyDown (KeyCode.Escape) && Paused ()) {
			var pauseObject = GameObject.Find ("Pause");
			pauseObject.SendMessage("ShowLoading");
			Application.LoadLevel("level_select");
		}
	}


}
