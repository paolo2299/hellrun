using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {

	public bool isActive { set; private get; }

	public GameObject rope;
	public GameObject ropeMiss;
	public GameObject claw;
	private Vector2 _anchor;
	private Vector2 _trailingEnd;
	private Vector2 _anchorMiss;
	private float _normalisedLength;
	private SpriteRenderer _ropeRenderer;
	private SpriteRenderer _ropeMissRenderer;
	private SpriteRenderer _clawRenderer;
	private int _ropeMissDisplayFrames = 3;
	private int _ropeMissDisplayFramesRemaining;

	public void Awake () {
		_ropeRenderer = rope.GetComponent<SpriteRenderer> ();
		_ropeMissRenderer = ropeMiss.GetComponent<SpriteRenderer> ();
		_clawRenderer = claw.GetComponent<SpriteRenderer> ();

		var ropeBounds = _ropeRenderer.bounds;
		_normalisedLength = ropeBounds.max.y - ropeBounds.min.y;
	}

	public void SetEnds (Vector2 anchor, Vector2 trailingEnd) {
		_anchor = anchor;
		_trailingEnd = trailingEnd;
	}

	public void SetAnchor (Vector2 anchor) {
		_anchor = anchor;
	}

	public void RegisterMiss (Vector2 missPoint) {
		_anchorMiss = missPoint;
		_ropeMissDisplayFramesRemaining = _ropeMissDisplayFrames;
	}

	public float AngleInDegrees (bool missed) {
		var anchor = missed ? _anchorMiss : _anchor;
		var direction = (Vector2) _trailingEnd - anchor;
		var anglePositivity = Mathf.Sign (direction.x);

		return anglePositivity * Vector2.Angle (Vector2.down, direction);
	}

	public float LengthMultiplier (bool missed) {
		var anchor = missed ? _anchorMiss : _anchor;
		var length = (anchor - _trailingEnd).magnitude;
		return 1.56f * length / _normalisedLength; //TODO why 1.56? figure out what's going on here...
	}

	public void LateUpdate () {
		if (LevelManagerSingleton.Instance && LevelManagerSingleton.Instance.Paused ()) {
			return;
		}

		if (isActive) {
			var missed = false;
			var anchor = _anchor;
			var ropeToUse = rope; //TODO put both rope and rope_miss sprite renderers on rope and figure out how to target the correct one
			_ropeRenderer.enabled = true;
			_ropeMissRenderer.enabled = false;
			if (_ropeMissDisplayFramesRemaining > 0) {
				missed = true;
				anchor = _anchorMiss;
				ropeToUse = ropeMiss;
				_ropeMissRenderer.enabled = true;
				_ropeRenderer.enabled = false;
				_ropeMissDisplayFramesRemaining -= 1;
			}
			_clawRenderer.enabled = true;
			ropeToUse.transform.position = _trailingEnd;
			claw.transform.position = anchor;
			var angle = AngleInDegrees(missed);
			ropeToUse.transform.eulerAngles = new Vector3(0, 0, angle);
			claw.transform.eulerAngles = new Vector3(0, 0, angle);
			ropeToUse.transform.localScale = new Vector3(transform.localScale.x, _normalisedLength * LengthMultiplier(missed), transform.localScale.z);
		} else {
			_ropeRenderer.enabled = false;
			_ropeMissRenderer.enabled = false;
			_clawRenderer.enabled = false;
		}
	}

	public void Render (SpriteRenderer ropeRenderer, SpriteRenderer clawRenderer) {
	}

	public void SetByAngleDegreesAndLength(float angleInDegrees, Vector2 trailingEnd, float length) {
		//assumes angle is between -90 degress and 90 degrees, where 0 degrees is straight up
		var angleInRadians = angleInDegrees * 2 * Mathf.PI / 360f;
		_trailingEnd = trailingEnd;
		var anchorY = _trailingEnd.y + length * Mathf.Cos (angleInRadians);
		var anchorX = _trailingEnd.x + length * Mathf.Sin (angleInRadians);
		_anchor = new Vector2 (anchorX, anchorY);
	}
}
