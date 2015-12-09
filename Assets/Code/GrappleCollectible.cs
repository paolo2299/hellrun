using UnityEngine;
using System.Collections;

public class GrappleCollectible : MonoBehaviour {

	public Animator animator;

	void Update () {
		if (LevelManager.Instance && LevelManager.Instance.ResetInProgress ()) {
			Debug.Log ("player grapple collectible due to level reset");
			animator.SetBool("IsCollected", false);
		}
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.gameObject.GetComponent<Player> ();
		if (player != null) {
			player.CollectGrapple();
			animator.SetBool("IsCollected", true);
		}
	}
}
