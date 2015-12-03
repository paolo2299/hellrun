using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {

	public bool isActive { set; private get; }

	public GameObject rope;
	public GameObject claw;
	private Vector2 _anchor;
	private Vector2 _trailingEnd;
	private float _normalisedLength;
	private SpriteRenderer _ropeRenderer;
	private SpriteRenderer _clawRenderer;

	public void Awake () {
		_ropeRenderer = rope.GetComponent<SpriteRenderer> ();
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

	public float AngleInDegrees () {
		var direction = (Vector2) _trailingEnd - _anchor;
		var anglePositivity = Mathf.Sign (direction.x);

		return anglePositivity * Vector2.Angle (Vector2.down, direction);
	}

	public float LengthMultiplier () {
		var length = (_anchor - _trailingEnd).magnitude;
		return 1.56f * length / _normalisedLength; //TODO why 1.56? figure out what's going on here...
	}

	public void LateUpdate () {
		if (isActive) {
			_ropeRenderer.enabled = true;
			_clawRenderer.enabled = true;
			rope.transform.position = _trailingEnd;
			claw.transform.position = _anchor;
			var angle = AngleInDegrees();
			rope.transform.eulerAngles = new Vector3(0, 0, AngleInDegrees());
			claw.transform.eulerAngles = new Vector3(0, 0, AngleInDegrees());
			rope.transform.localScale = new Vector3(transform.localScale.x, _normalisedLength * LengthMultiplier(), transform.localScale.z);
		} else {
			_ropeRenderer.enabled = false;
			_clawRenderer.enabled = false;
		}
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
