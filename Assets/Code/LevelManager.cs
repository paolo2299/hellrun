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
	private GameProgress gameProgress;
	private string levelName;

	private bool _resetting;

	public void Awake() {
		LevelManager.Instance = this;
		player = GameObject.FindObjectOfType<Player> ();
		mainCamera = GameObject.FindObjectOfType<CameraController> ();
		stopWatchThisTry = new StopWatch ();
		stopWatchSinceLevelStart = new StopWatch ();
		gameProgress = GameProgress.Load();
		levelName = gameProgress.GetLevelName(Application.loadedLevelName);
	}

	public void LevelComplete() {
		stopWatchThisTry.Stop ();
		player.Disable ();
		SaveLevelData ();
		
		Application.LoadLevelAdditive ("level_complete");
	}

	private void SaveLevelData() {
		var alreadyComplete = gameProgress.GetLevelComplete (Application.loadedLevelName);
		var bestTime = gameProgress.GetLevelBestTime (Application.loadedLevelName);
		if (!alreadyComplete || bestTime > stopWatchThisTry.time) {
			gameProgress.SetLevelBestTime(Application.loadedLevelName, stopWatchThisTry.time);
		}
		gameProgress.SetLevelComplete (Application.loadedLevelName, true);
	}

	public bool ResetInProgress() {
		return _resetting;
	}

	public void Reset() {
		stopWatchThisTry.Start ();
		_resetting = true;
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
		} else {
			levelNameText.text = levelName;
		}
		if (_resetting) {
			//Only allow the resetting flag to be true for one frame
			_resetting = false;
		}
		if (Input.GetKey (KeyCode.R)) {
			Reset ();
		}
	}
}
