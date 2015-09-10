using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Transform Player;
	public Vector2 Smoothing = new Vector2(5f, 5f);
	public BoxCollider2D Bounds;
	public bool IsFollowing = true;

	private Vector3 
		_min,
		_max;

	void Start () {;
		_min = Bounds.bounds.min;
		_max = Bounds.bounds.max;
	}

	void LateUpdate () {
		var x = transform.position.x;
		var y = transform.position.y;

		if (IsFollowing) {
			x = Player.position.x;

			y = Player.position.y;
		}

		var cameraHalfWidth = Camera.main.orthographicSize * ((float) Screen.width / Screen.height);

		x = Mathf.Clamp (x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
		y = Mathf.Clamp (y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);

		transform.position = new Vector3 (x, y, transform.position.z);
	}
}
