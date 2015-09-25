using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {
	public void GotoNextLevel(string levelName) {
		StartCoroutine(GotoNextLevelCo(levelName));
	}

	private IEnumerator GotoNextLevelCo(string levelName) {

		yield return new WaitForSeconds (2f);

		if (string.IsNullOrEmpty (levelName))
			Application.LoadLevel ("StartScreen");
		else
			Application.LoadLevel (levelName);
	}

	public void KillPlayer() {
		StartCoroutine (KillPlayerCo());
	}

	private IEnumerator KillPlayerCo() {
		yield return new WaitForSeconds (2f);
	}
}
