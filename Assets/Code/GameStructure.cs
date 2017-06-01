using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameStructure {
	private static List<GameChapter> chapters {
		get {
			return new List<GameChapter> {
				GameChapter.TheCastle
			};
		}
	}

	private static List<GameLevel> levels {
		get {
			return chapters.SelectMany(chapter => chapter.levels).ToList();
		}
	}

	public static GameLevel GetLevel(string sceneName) {
		//TODO throw exception if not found
		return levels.Find (level => level.sceneName == sceneName);
	}

	public static string GetMedalAttained(string sceneName, float timeTaken) {
		return GetLevel (sceneName).medalAttained(timeTaken);
	}

	public static float GetGoldMedalTime(string sceneName) {
		return GetLevel (sceneName).goldTime;
	}

	public static float GetSilverMedalTime(string sceneName) {
		return GetLevel (sceneName).silverTime;
	}

	public static float GetBronzeMedalTime(string sceneName) {
		return GetLevel (sceneName).bronzeTime;
	}

	private static GameChapter GetChapterWithScene(string sceneName) {
		//TODO throw exception if not found
		return chapters.Find (c => c.ContainsScene (sceneName));
	}

	public static bool GetIsLastLevelInChapter(string sceneName) {
		var chapter = GetChapterWithScene (sceneName);
		return chapter.IsFinalLevel (sceneName);
	}

	public static string GetNextSceneName (string sceneName) {
		var chapter = GetChapterWithScene (sceneName);
		return chapter.GetNextSceneName(sceneName);
	}

	public static string GetSceneName (string chapterRef, int levelIndex) {
		var chapter = GetChapter (chapterRef);
		return chapter.levels [levelIndex].sceneName;
	}

	public static GameChapter GetChapter (string chapterRef) {
		return chapters.Find (c => c.reference == chapterRef);
	}

	public static string GetLevelDisplayName (string sceneName) {
		return GetLevel (sceneName).displayName;
	}
}

public class GameLevel {
	public GameLevel (string displayName,
	                  string sceneName,
	                  float goldTime,
	                  float silverTime,
	                  float bronzeTime) {
		this.displayName = displayName;
		this.sceneName = sceneName;
		this.bronzeTime = bronzeTime;
		this.silverTime = silverTime;
		this.goldTime = goldTime;
	}

	public string displayName;
	public float goldTime;
	public float silverTime;
	public float bronzeTime;
	public string sceneName;

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

public class GameChapter {
	public static GameChapter TheCastle { 
		get {
			var levels = new List<GameLevel> {
				new GameLevel("the stairs",
				              "the_stairs",
				              4f,
				              6f,
				              10f),
				new GameLevel("scaling the turret",
				              "level_1_2",
				              5.5f,
				              8f,
				              11f),
				new GameLevel("gate hopper",
				              "level_1_3",
				              11.1f,
				              12f,
				              16f),
				new GameLevel("climbing the walls",
				              "level_1_4",
				              14.5f,
				              18f,
				              23f),
				new GameLevel("the haunted platform",
				              "level_1_5",
				              20.8f,
				              22f,
				              30f),
				new GameLevel("the descent",
				              "level_1_6",
				              25f,
				              29f,
				              40f),		
				new GameLevel("donkey kong I",
				              "level_1_7",
				              26f,
				              33f,
				              45f),
				new GameLevel("donkey kong II",
				              "level_1_8",
				              19f,
				              25f,
				              40f),
				new GameLevel("donkey kong III",
				              "level_1_9",
				              40f,
				              50f,
				              70f),
				new GameLevel("donkey kong IV",
				              "level_1_10",
				              40f,
				              50f,
				              70f),
			};
			return new GameChapter("chapter1", "the castle", levels);
		}

	}
	
	public GameChapter (string reference, string displayName, List<GameLevel> levels) {
		this.reference = reference;
		this.displayName = displayName;
		this.levels = levels;
	}
	
	public string reference;
	public string displayName;
	public List<GameLevel> levels;

	public bool ContainsScene(string sceneName) {
		return GetLevel(sceneName) != null;
	}

	private GameLevel GetLevel(string sceneName) {
		return levels.Find (l => l.sceneName == sceneName);
	}

	public GameLevel FinalLevel {
		get {
			return levels.Last();
		}
	}

	public bool IsFinalLevel(string sceneName) {
		return FinalLevel.sceneName == sceneName;
	}

	public string GetNextSceneName(string sceneName) {
		//TODO throw if not found or if final level
		var index = levels.FindIndex (l => l.sceneName == sceneName);
		return levels[index + 1].sceneName;
	}
}
