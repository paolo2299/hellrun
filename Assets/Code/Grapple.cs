using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {

	public bool isActive { set; private get; }

	private Vector2 _anchor;
	private Vector2 _trailingEnd;
	private SpriteRenderer _spriteRenderer;
	private float _normalisedLength;

	public void Awake () {
		_spriteRenderer = GetComponent<SpriteRenderer>();
		var spriteBounds = _spriteRenderer.bounds;
		_normalisedLength = spriteBounds.max.y - spriteBounds.min.y;
	}

	public void SetEnds (Vector2 anchor, Vector2 trailingEnd) {
		_anchor = anchor;
		_trailingEnd = trailingEnd;
	}

	public void SetAnchor (Vector2 anchor) {
		_anchor = anchor;
	}

	public float AngleInDegrees () {
		var direction = (Vector2) transform.position - _anchor;
		var anglePositivity = Mathf.Sign (direction.x);

		return anglePositivity * Vector2.Angle (Vector2.down, direction);
	}

	public float LengthMultiplier () {
		var length = (_anchor - _trailingEnd).magnitude;
		return 1.56f * length / _normalisedLength; //TODO why 1.56? figure out what's going on here...
	}

	public void LateUpdate () {
		if (isActive) {
			_spriteRenderer.enabled = true;
			transform.position = _trailingEnd;
			transform.eulerAngles = new Vector3(0, 0, AngleInDegrees());
			transform.localScale = new Vector3(transform.localScale.x, _normalisedLength * LengthMultiplier(), transform.localScale.z);
		} else {
			_spriteRenderer.enabled = false;
		}
	}
}
