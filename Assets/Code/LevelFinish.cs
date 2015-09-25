using UnityEngine;
using System.Collections;

public class LevelFinish : MonoBehaviour {
	public string nextLevel;

	public void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.gameObject.GetComponent<Player> ();
		if (player != null) {
			player.FinishLevel(nextLevel);
		}
	}
}
