using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {

	public bool isActive { set; private get; }

	private Vector2 _anchor;
	private Vector2 _trailingEnd;
	private LineRenderer _lineRenderer;

	public void Awake () {
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.sortingLayerName = "grapple";
	}

	public void SetEnds (Vector2 anchor, Vector2 trailingEnd) {
		_anchor = anchor;
		_trailingEnd = trailingEnd;
	}

	public void LateUpdate () {
		if (isActive) {
			_lineRenderer.enabled = true;
			_lineRenderer.SetPosition (0, new Vector3(_anchor.x, _anchor.y, -1));
			_lineRenderer.SetPosition (1, new Vector3(_trailingEnd.x, _trailingEnd.y, -1));
		} else {
			_lineRenderer.enabled = false;
		}
	}
}
