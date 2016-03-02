using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance { get; private set; }

	public Player player;
	public GUIText elapsedTimeText;
	public GUIText levelNameText;
	public string nextLevel;
	private StopWatch stopWatchThisTry;
	private StopWatch stopWatchSinceLevelStart;
	private GameProgress gameProgress;
	private string levelName;

	private bool _resetting;
	private int _resetFrames = 1;
	private bool _paused = false;

	public void Awake() {
		Time.timeScale = 1; //In case we arrive here from a paused state
		LevelManager.Instance = this;
		player = GameObject.FindObjectOfType<Player> ();
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

	private void SaveLevelData() {
		var alreadyComplete = gameProgress.GetLevelComplete (Application.loadedLevelName);
		var bestTime = gameProgress.GetLevelBestTime (Application.loadedLevelName);
		if (!alreadyComplete || bestTime > stopWatchThisTry.time) {
			gameProgress.SetLevelBestTime(Application.loadedLevelName, stopWatchThisTry.time);
		}
		gameProgress.SetLevelComplete (Application.loadedLevelName, true);
	}

	public void Reset() {
		Application.LoadLevel(Application.loadedLevelName);
	}

	public void KillPlayer() {
		StartCoroutine (KillPlayerCo());
	}

	private IEnumerator KillPlayerCo() {
		player.Die ();
		yield return new WaitForSeconds (0.5f);
		Reset ();
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
		if (_resetting) {
			if (_resetFrames > 0) {
				_resetFrames -= 1;
			} else {
				_resetting = false;
			} 
		}
		if (Input.GetKeyDown (KeyCode.R) && Paused()) {
			Reset ();
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
			if (!_paused) {
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
