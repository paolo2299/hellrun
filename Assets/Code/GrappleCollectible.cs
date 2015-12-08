using UnityEngine;
using System.Collections;

public class GrappleCollectible : MonoBehaviour {

	public Animator animator;

	public void OnTriggerEnter2D(Collider2D other)
	{
		var player = other.gameObject.GetComponent<Player> ();
		if (player != null) {
			player.CollectGrapple();
			animator.SetBool("IsCollected", true);
		}
	}
}
