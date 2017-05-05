using UnityEngine;
using System.Collections;

public class Instadeath : MonoBehaviour {
	public void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.gameObject.GetComponent<Player> ();
		if (player != null && player.InPlay()) {
			LevelManagerSingleton.Instance.KillPlayer();
		}
	}
}
