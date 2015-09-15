using UnityEngine;
using System.Collections;

public class BackgroundParallax : MonoBehaviour {

	public Transform background;
	public float parallaxScale;

	private Vector3 _lastPosition;

	void Start () {
		_lastPosition = transform.position;
	}

	void Update () {
		var parallax = (_lastPosition.x - transform.position.x) * parallaxScale;
		var backgroundTargetPosition = background.position.x - parallax;
		background.position = new Vector3 (backgroundTargetPosition, background.position.y, background.position.z);
		_lastPosition = transform.position;
	}
}
