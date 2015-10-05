using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameData {

	//make this class a singleton
	private static GameData _instance;
	private GameData() {}

	private GameProgress _gameProgress;

	public static GameData Instance {
		get {
			if (_instance == null) {
				_instance = new GameData();
			}
			return _instance;
		}
	}

	private GameProgress gameProgress;

	public LevelProgress GetLevelProgress(string levelName) {
#if UNITY_WEBPLAYER
		Debug.Log ("not loading level progress from file as this is the web player");
		var levelProgress = new LevelProgress ();
		levelProgress.levelName = levelName;
		return levelProgress;
#else
		return LoadLevelProgressFromFile(levelName);
#endif
	}

	private LevelProgress LoadLevelProgressFromFile(string levelName) {
		if (_gameProgress == null) {
			LoadGameProgress();
		}
		if (_gameProgress.levels == null) {;
			_gameProgress.levels = new List<LevelProgress>();
		};
		foreach (LevelProgress lp in _gameProgress.levels) {
			if (lp.levelName == levelName) {
				return lp;
			}
		}
		var levelProgress = new LevelProgress ();
		levelProgress.levelName = levelName;
		_gameProgress.levels.Add (levelProgress);
		return levelProgress;
	}
	
	public void LoadGameProgress() {
		if (File.Exists (Application.persistentDataPath + "/gameData.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/gameData.dat", FileMode.Open);
			_gameProgress = (GameProgress)bf.Deserialize (file);
			file.Close ();
		} else {
			_gameProgress = new GameProgress ();
		}
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
		foreach (LevelProgress lp in _gameProgress.levels) {
			Debug.Log ("State of level " + lp.levelName);
			Debug.Log (lp.complete);
			Debug.Log (lp.bestTime);
		}
		
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/gameData.dat");
		bf.Serialize (file, _gameProgress);
		file.Close ();
	}
}

[System.Serializable]
public class GameProgress {
	public List<LevelProgress> levels;
}

[System.Serializable]
public class LevelProgress {
	public string levelName;
	public bool complete;
	public float bestTime;
}
