using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LevelProgress {
	public LevelProgress (string sceneName,
							float bestTime,
							bool complete)
	{
		this.sceneName = sceneName;
		this.bestTime = bestTime;
		this.complete = complete;
	}
	
	public string sceneName;
	public float bestTime;
	public bool complete;
}

[System.Serializable]
public class GameProgress {
	public List<LevelProgress> levelsProgress = new List<LevelProgress>();
	public bool hasPermanentGrapple = false;
	
	private static GameProgress _gameProgress;
	private static string saveFileName = "gameProgressv1002.dat";
	
	public static GameProgress Load() {
		if (_gameProgress != null) {
			return _gameProgress;
		}
		#if UNITY_WEBPLAYER
		Debug.Log ("Not loading game progress as inside web player"); 
		_gameProgress = new GameProgress ();
		#else
		_gameProgress = LoadFromFile();
		#endif

		return _gameProgress;
	}
	
	private static GameProgress LoadFromFile() {
		if (File.Exists (Application.persistentDataPath + "/" + saveFileName)) {
			Debug.Log ("Loading game progress from file " + Application.persistentDataPath + "/" + saveFileName);
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/" + saveFileName, FileMode.Open);
			var gameProgressJSON = (string)bf.Deserialize (file);
			var gameProgress = JsonUtility.FromJson<GameProgress>(gameProgressJSON);
			file.Close ();
			return gameProgress;
		} else {
			Debug.Log ("No game progress file found at " + Application.persistentDataPath + "/" + saveFileName);
			return new GameProgress ();
		}
	}
	
	public bool GetLevelComplete(string sceneName) {
		return GetLevelProgressWithSceneName (sceneName).complete;
	}

	private LevelProgress GetLevelProgressWithSceneName(string sceneName) {
		//TODO what happens if null here?
		var progress = levelsProgress.Find (lp => lp.sceneName == sceneName);
		if( progress != null ) {
			return progress;
		} else {
			//TODO only create the new level progress if it exists in GameStructure, otherwise throw exception
			//TODO prune levels_progress each time depending on what levels currently exist
			var levelProgress = new LevelProgress (
				sceneName,
				0.0f,
				false
			);
			levelsProgress.Add (levelProgress);
			SaveGameProgress ();
			return levelProgress;
		}
	}
	
	public void SetLevelComplete(string sceneName, bool levelComplete) {
		var levelProgress = GetLevelProgressWithSceneName (sceneName);
		levelProgress.complete = levelComplete;
		SaveGameProgress ();
	}
	
	public float GetLevelBestTime(string sceneName) {
		return GetLevelProgressWithSceneName (sceneName).bestTime;
	}
	
	public string GetMedalAttained(string sceneName) {
		var bestTime = GetLevelBestTime (sceneName);
		return GameStructure.GetMedalAttained (sceneName, bestTime);
	}
	
	public void SetLevelBestTime(string sceneName, float bestTime) {
		var levelProgress = GetLevelProgressWithSceneName (sceneName);
		levelProgress.bestTime = bestTime;
		SaveGameProgress ();
	}
	
	public void CollectPermanentGrapple() {
		hasPermanentGrapple = true;
		SaveGameProgress ();
	}
	
	public void SaveGameProgress() {
		#if UNITY_WEBPLAYER
		Debug.Log ("not saving game progress as this is the web player");
		#else
		SaveGameProgressToFile ();
		#endif
	}
	
	private void SaveGameProgressToFile() {
		var gameProgressJSON = JsonUtility.ToJson (this);
		BinaryFormatter bf = new BinaryFormatter();
		Debug.Log ("Saving game progress to file " + Application.persistentDataPath + "/" + saveFileName);
		FileStream file = File.Create (Application.persistentDataPath + "/" + saveFileName);
		bf.Serialize (file, gameProgressJSON);
		file.Close ();
	}
}
