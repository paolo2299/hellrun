using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public static LevelManager Instance { get; private set; }

	public Player player;
	public CameraController mainCamera;

	public void Awake() {
		LevelManager.Instance = this;
		Debug.Log (LevelManager.Instance);
		player = GameObject.FindObjectOfType<Player> ();
		mainCamera = GameObject.FindObjectOfType<CameraController> ();
	}

	public void GotoNextLevel(string levelName) {
		StartCoroutine(GotoNextLevelCo(levelName));
	}

	private IEnumerator GotoNextLevelCo(string levelName) {

		yield return new WaitForSeconds (0.1f);

		if (string.IsNullOrEmpty (levelName))
			Application.LoadLevel ("StartScreen");
		else
			Application.LoadLevel (levelName);
	}

	public void KillPlayer() {
		StartCoroutine (KillPlayerCo());
	}

	private IEnumerator KillPlayerCo() {
		player.Die ();

		yield return new WaitForSeconds (0.5f);

		player.Respawn ();
	}
}
