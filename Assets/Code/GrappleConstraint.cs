using UnityEngine;
using System.Collections;

public class GrappleConstraint {

	//fields
	private Vector2 _anchor;
	private float _length;

	//properties
	public Vector2 anchor { get { return _anchor; } }
	public float length { get { return _length; } }
    
	//constructors
	public GrappleConstraint (Vector2 anchor, Vector2 trailingEnd) {
		_anchor = anchor;
		_length = (trailingEnd - anchor).magnitude;
	}
}
