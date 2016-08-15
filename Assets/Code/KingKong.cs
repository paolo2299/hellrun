using UnityEngine;
using System.Collections;

public class KingKong : MonoBehaviour {
	private float fallSpeed = 10.0f;
	private bool isDead = false;

	// Update is called once per frame
	void Update () {
		if (isDead) {
			var fallDistance = fallSpeed * Time.deltaTime;
			transform.position = new Vector2(transform.position.x, transform.position.y - fallDistance);
		}
	}

	public void Die() {
		FlipUpsideDown ();
		isDead = true;
	}

	private void FlipUpsideDown() {
		transform.localScale = new Vector3 (transform.localScale.x, -transform.localScale.y, transform.localScale.z);
	}
}
