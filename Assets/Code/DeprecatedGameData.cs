using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DeprecatedGameData {

	//make this class a singleton
	private static DeprecatedGameData _instance;
	private DeprecatedGameData() {}

	private DeprecatedGameProgress _gameProgress;

	public static DeprecatedGameData Instance {
		get {
			if (_instance == null) {
				_instance = new DeprecatedGameData();
			}
			return _instance;
		}
	}

	private DeprecatedGameProgress gameProgress;

	public DeprecatedLevelProgress GetLevelProgress(string levelName) {
#if UNITY_WEBPLAYER
		Debug.Log ("not loading level progress from file as this is the web player");
		var levelProgress = new LevelProgress ();
		levelProgress.levelName = levelName;
		return levelProgress;
#else
		return LoadLevelProgressFromFile(levelName);
#endif
	}

	private DeprecatedLevelProgress LoadLevelProgressFromFile(string levelName) {
		if (_gameProgress == null) {
			LoadGameProgress();
		}
		if (_gameProgress.levels == null) {;
			_gameProgress.levels = new List<DeprecatedLevelProgress>();
		};
		foreach (DeprecatedLevelProgress lp in _gameProgress.levels) {
			if (lp.levelName == levelName) {
				return lp;
			}
		}
		var levelProgress = new DeprecatedLevelProgress ();
		levelProgress.levelName = levelName;
		_gameProgress.levels.Add (levelProgress);
		return levelProgress;
	}
	
	public void LoadGameProgress() {
		if (File.Exists (Application.persistentDataPath + "/gameData.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/gameData.dat", FileMode.Open);
			_gameProgress = (DeprecatedGameProgress)bf.Deserialize (file);
			file.Close ();
		} else {
			_gameProgress = new DeprecatedGameProgress ();
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
		foreach (DeprecatedLevelProgress lp in _gameProgress.levels) {
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
public class DeprecatedGameProgress {
	public List<DeprecatedLevelProgress> levels;
}

[System.Serializable]
public class DeprecatedLevelProgress {
	public string levelName;
	public bool complete;
	public float bestTime;
}
