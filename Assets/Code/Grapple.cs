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

	public float AngleInDegrees () {
		var opposite = transform.position.x - _anchor.x;
		var adjacent = _anchor.y - transform.position.y;
		if (opposite == 0)
			return 0;
		var ratio = opposite / adjacent;
		return Mathf.Atan (ratio) * (360 / (2 * Mathf.PI));
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
