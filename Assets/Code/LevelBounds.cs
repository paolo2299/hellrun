using UnityEngine;
using System.Collections;

public class LevelBounds : MonoBehaviour {
	public void OnTriggerExit2D(Collider2D other)
	{
		var player = other.gameObject.GetComponent<Player> ();
		if (player != null) {
			LevelManagerSingleton.Instance.KillPlayer();
			return;
		}

		//Not the player so destroy it
		Destroy(other.gameObject);
	}
}
