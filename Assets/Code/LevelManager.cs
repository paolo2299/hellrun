using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance { get; private set; }

	public Player player;
	public CameraController mainCamera;
	public GUIText elapsedTimeText;
	public GUIText levelNameText;
	public string nextLevel;
	private float elapsedTimeThisTry;
	private float totalElapsedTime;
	private LevelProgress levelProgress;

	public void Awake() {
		LevelManager.Instance = this;
		player = GameObject.FindObjectOfType<Player> ();
		mainCamera = GameObject.FindObjectOfType<CameraController> ();
		elapsedTimeThisTry = 0f;
		totalElapsedTime = 0f;
		levelProgress = GameData.Instance.GetLevelProgress (Application.loadedLevelName);
	}

	public void LevelComplete() {
		player.Disable ();
		SaveLevelData ();
		
		Application.LoadLevelAdditive ("level_complete");
	}

	private void SaveLevelData() {
		if (!levelProgress.complete || levelProgress.bestTime > elapsedTimeThisTry) {
			levelProgress.bestTime = elapsedTimeThisTry;
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
		elapsedTimeThisTry = 0f;
		player.Respawn ();
	}

	public void Update() {
		elapsedTimeThisTry += Time.deltaTime;
		totalElapsedTime += Time.deltaTime;
		elapsedTimeText.text = ElapsedTimeThisTryString ();
		if (totalElapsedTime > 5f) {
			levelNameText.enabled = false;
		}
	}

	public string ElapsedTimeThisTryString() {
		var minutes = (int)(elapsedTimeThisTry / 60);
		var seconds = (int)(elapsedTimeThisTry % 60);
		var fraction = (int)((elapsedTimeThisTry * 100) % 100);
		return string.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);
	}
}
