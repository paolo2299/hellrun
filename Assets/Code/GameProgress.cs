using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameProgress {

	public List<GameChapter> chapters = new List<GameChapter>();
	public bool hasPermanentGrapple = false;

	private static GameProgress _gameProgress;

	public static GameProgress Load() {
		if (_gameProgress != null) {
			return _gameProgress;
		}
		#if UNITY_WEBPLAYER
			Debug.Log ("Not loading game progress as insode web player"); 
			_gameProgress = new GameProgress ();
		#else
			_gameProgress = LoadFromFile();
		#endif

		return _gameProgress;
	}

	private static GameProgress LoadFromFile() {
		if (File.Exists (Application.persistentDataPath + "/gameProgress.dat")) {
			Debug.Log ("Loading game progress from file " + Application.persistentDataPath + "/gameProgress.dat");
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/gameProgress.dat", FileMode.Open);
			var gameProgress = (GameProgress)bf.Deserialize (file);
			foreach (GameChapter gc in gameProgress.chapters) {
				Debug.Log ("Loaded state of chapter " + gc.name);
				foreach (GameLevel gl in gc.levels) {
					Debug.Log ("******* " + gl.name + " ********");
					Debug.Log ("name of next scene: " + gl.nextSceneName);
					Debug.Log ("complete?");
					Debug.Log (gl.complete);
					Debug.Log ("last level in chapter?");
					Debug.Log (gl.lastLevelInChapter);
					Debug.Log ("best time");
					Debug.Log (gl.bestTime);
				}
			}
			Debug.Log ("has permanent grapple");
			Debug.Log (gameProgress.hasPermanentGrapple);
			file.Close ();
			return gameProgress;
		} else {
			Debug.Log ("No game progress file found at " + Application.persistentDataPath + "/gameProgress.dat");
			return new GameProgress ();
		}
	}

	public bool GetLevelComplete(string sceneName) {
		return GetLevelWithSceneName (sceneName).complete;
	}

	public void SetLevelComplete(string sceneName, bool levelComplete) {
		var level = GetLevelWithSceneName (sceneName);
		level.complete = levelComplete;
		SaveGameProgress ();
	}

	public float GetLevelBestTime(string sceneName) {
		return GetLevelWithSceneName (sceneName).bestTime;
	}

	public string GetMedalAttained(string sceneName) {
		return GetLevelWithSceneName (sceneName).medalAttained();
	}

	public string GetMedalAttained(string sceneName, float timeTaken) {
		return GetLevelWithSceneName (sceneName).medalAttained(timeTaken);
	}

	public float GetGoldMedalTime(string sceneName) {
		return GetLevelWithSceneName (sceneName).goldTime;
	}

	public float GetSilverMedalTime(string sceneName) {
		return GetLevelWithSceneName (sceneName).silverTime;
	}

	public float GetBronzeMedalTime(string sceneName) {
		return GetLevelWithSceneName (sceneName).bronzeTime;
	}
	
	public void SetLevelBestTime(string sceneName, float bestTime) {
		var level = GetLevelWithSceneName (sceneName);
		level.bestTime = bestTime;
		SaveGameProgress ();
	}

	public bool GetIsLastLevelInChapter(string sceneName) {
		return GetLevelWithSceneName (sceneName).lastLevelInChapter;
	}

	public string GetNextSceneName (string sceneName) {
		return GetLevelWithSceneName (sceneName).nextSceneName;
	}

	public string GetLevelName (string sceneName) {
		return GetLevelWithSceneName (sceneName).name;
	}

	public GameProgress () {
		chapters.Add (GameChapter.TheCastle);
	}

	public void CollectPermanentGrapple() {
		hasPermanentGrapple = true;
		SaveGameProgress ();
	}

	private GameLevel GetLevelWithSceneName(string sceneName) {
		char[] delimiterChars = {'_'};
		var sceneElements = sceneName.Split (delimiterChars); //sceneElements is level_<chapterIndex + 1>_<levelIndex + 1>
		var chapterIndex = int.Parse(sceneElements [1]) - 1;
		var levelIndex = int.Parse (sceneElements [2]) - 1;
		return chapters [chapterIndex].levels [levelIndex];
	}

	public void SaveGameProgress() {
		#if UNITY_WEBPLAYER
			Debug.Log ("not saving game progress as this is the web player");
		#else
			SaveGameProgressToFile ();
		#endif
	}
	
	private void SaveGameProgressToFile() {
		Debug.Log ("Saving Game");
		foreach (GameChapter gc in chapters) {
			Debug.Log ("State of chapter " + gc.name);
			foreach (GameLevel gl in gc.levels) {
				Debug.Log (gl.name);
				Debug.Log (gl.nextSceneName);
				Debug.Log (gl.lastLevelInChapter);
			}
		}
		
		BinaryFormatter bf = new BinaryFormatter();
		Debug.Log ("Saving game progress to file " + Application.persistentDataPath + "/gameProgress.dat");
		FileStream file = File.Create (Application.persistentDataPath + "/gameProgress.dat");
		bf.Serialize (file, this);
		file.Close ();
	}
}

[System.Serializable]
public class GameLevel {
	public GameLevel (string name,
	                  string sceneName,
	                  string nextSceneName,
	                  float goldTime,
	                  float silverTime,
	                  float bronzeTime,
	                  bool firstLevelInChapter = false,
	                  bool lastLevelInChapter = false) {
		this.name = name;
		this.sceneName = sceneName;
		this.nextSceneName = nextSceneName;
		this.bronzeTime = bronzeTime;
		this.silverTime = silverTime;
		this.goldTime = goldTime;
		this.firstLevelInChapter = firstLevelInChapter;
		this.lastLevelInChapter = lastLevelInChapter;
	}

	public string name;
	public bool firstLevelInChapter;
	public bool lastLevelInChapter;
	public bool complete;
	public float bestTime;
	public float goldTime;
	public float silverTime;
	public float bronzeTime;
	public string sceneName;
	public string nextSceneName;

	public string medalAttained() {  //TODO use enum instead of strings
		if (!complete) {
			return "";
		}
		return medalAttained (bestTime);
	}

	public string medalAttained(float timeTaken) { //TODO use enum instead of strings
		if (timeTaken > bronzeTime) {
			return "";
		}
		if (timeTaken > silverTime) {
			return "bronze";
		}
		if (timeTaken > goldTime) {
			return "silver";
		}
		return "gold";
	}
}

[System.Serializable]
public class GameChapter {
	public static GameChapter TheCastle { 
		get {
			var levels = new List<GameLevel> {
				new GameLevel("the stairs",
				              "level_1_1",
				              "level_1_2",
				              4f,
				              6f,
				              10f,
				              true,
				              false),
				new GameLevel("scaling the turret",
				              "level_1_2",
				              "level_1_3",
				              5.5f,
				              8f,
				              11f,
				              false,
				              false), //TODO saving the next scene name is a bit weird - should be a more elegant way
				new GameLevel("gate hopper",
				              "level_1_3",
				              "level_1_4",
				              11.1f,
				              12f,
				              16f,
				              false,
				              false),
				new GameLevel("climbing the walls",
				              "level_1_4",
				              "level_1_5",
				              14.5f,
				              18f,
				              23f,
				              false,
				              false),
				new GameLevel("the haunted platform",
				              "level_1_5",
				              "level_1_6",
				              20.8f,
				              22f,
				              30f,
				              false,
				              false),
				new GameLevel("the descent",
				              "level_1_6",
				              "level_1_7",
				              25f,
				              29f,
				              40f,
				              false,
				              true),
			};
			return new GameChapter("the castle", levels);
		} 
	}

	public GameChapter (string name, List<GameLevel> levels) {
		this.name = name;
		this.levels = levels;
	}

	public string name;
	public List<GameLevel> levels;
}
