using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance { get; private set; }

	public Player player;
	public CameraController mainCamera;
	public GUIText elapsedTimeText;
	public GUIText levelNameText;
	public string nextLevel;
	private StopWatch stopWatchThisTry;
	private StopWatch stopWatchSinceLevelStart;
	private LevelProgress levelProgress;

	public void Awake() {
		LevelManager.Instance = this;
		player = GameObject.FindObjectOfType<Player> ();
		mainCamera = GameObject.FindObjectOfType<CameraController> ();
		stopWatchThisTry = new StopWatch ();
		stopWatchSinceLevelStart = new StopWatch ();
		levelProgress = GameData.Instance.GetLevelProgress (Application.loadedLevelName);
	}

	public void LevelComplete() {
		stopWatchThisTry.Stop ();
		player.Disable ();
		SaveLevelData ();
		
		Application.LoadLevelAdditive ("level_complete");
	}

	private void SaveLevelData() {
		if (!levelProgress.complete || levelProgress.bestTime > stopWatchThisTry.time) {
			levelProgress.bestTime = stopWatchThisTry.time;
		}
		levelProgress.complete = true;
		GameData.Instance.SaveGameProgress ();
	}

	public void KillPlayer() {
		StartCoroutine (KillPlayerCo());
	}

	private IEnumerator KillPlayerCo() {
		player.Die ();
		yield return new WaitForSeconds (0.5f);
		stopWatchThisTry.Start ();
		player.Respawn ();
	}

	public void Update() {
		elapsedTimeText.text = stopWatchThisTry.formattedTime;
		if (stopWatchSinceLevelStart.time > 5f) {
			levelNameText.enabled = false;
		}
	}
}
