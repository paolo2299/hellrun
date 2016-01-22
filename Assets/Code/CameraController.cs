using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Transform Player;
	public Vector2 Smoothing = new Vector2(5f, 5f);
	public BoxCollider2D Bounds;
	public bool IsFollowing = true;
	public bool FixX = false;
	public float FixedX;

	private float targetaspectX = 16.0f;
	private float targetaspectY = 9.0f;
	private float targetaspect;

	private Camera camera;

	private Vector3 
		_min,
		_max;

	void Start () {
		camera = GetComponent<Camera> ();

		//set the desired aspect ratio
		targetaspect = targetaspectX / targetaspectY;

		//determine the game window's current aspect ratio
		float windowaspect = (float)Screen.width / (float)Screen.height;

		float scaleHeight = windowaspect / targetaspect;;

		if (scaleHeight < 1.0f) {
			Rect rect = camera.rect;

			rect.width = 1.0f;
			rect.height = scaleHeight;
			rect.x = 0;
			rect.y = (1.0f - scaleHeight) / 2.0f;

			camera.rect = rect;
		} else {
			float scaleWidth = 1.0f / scaleHeight;

			Rect rect = camera.rect;
			rect.width = scaleWidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scaleWidth) / 2.0f;
			rect.y = 0;

			camera.rect = rect;
		}

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

		if (FixX) {
			x = FixedX;
		}

		var cameraHalfWidth = Camera.main.orthographicSize * targetaspect;

		x = Mathf.Clamp (x, _min.x + cameraHalfWidth, _max.x - cameraHalfWidth);
		y = Mathf.Clamp (y, _min.y + Camera.main.orthographicSize, _max.y - Camera.main.orthographicSize);

		transform.position = new Vector3 (x, y, transform.position.z);
	}
}
