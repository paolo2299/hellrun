using UnityEngine;
using System.Collections;

public class GrappleConstraint {

	//fields
	private Vector2 _anchor;
	private float _length;
	private float _maxLength;

	//properties
	public Vector2 anchor { get { return _anchor; } }
	public float length { get { return _length; } }
    
	//constructors
	public GrappleConstraint (Vector2 anchor, float startLength, float maxLength) {
		_anchor = anchor;
		_length = length;
		_maxLength = maxLength;
	}

	//public methods
	public void Extend (float amount) {
		_length += amount;
		_length = Mathf.Clamp (_length, 0, _maxLength);
	}

	public void Retract (float amount) {
		_length -= amount;
		_length = Mathf.Clamp (_length, 0, _maxLength);
	}
}
