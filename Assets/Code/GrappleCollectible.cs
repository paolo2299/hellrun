using UnityEngine;
using System.Collections;

public class GrappleCollectible : MonoBehaviour {

	public Animator animator;

	private BoxCollider2D _boxCollider;
	private Renderer _renderer;

	void Awake () {
		_boxCollider = GetComponent<BoxCollider2D> ();
		_renderer = GetComponent<Renderer> ();
		if (LevelManagerSingleton.Instance.PlayerHasPermanentGrapple ()) {
			Debug.Log ("disabling grapple collectible");
			Disable ();
		}
	}

	void Disable () {
		enabled = false;
		_boxCollider.enabled = false;
		_renderer.enabled = false;
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
