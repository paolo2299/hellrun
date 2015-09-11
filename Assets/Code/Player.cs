using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public PlayerParameters2D DefaultParameters;
	public PlayerParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }
	public float SpeedAccelerationRunning = 4f;
	public float SpeedAccelerationWalking = 3f;
	public float MaxSpeedRunning = 8f;
	public float MaxSpeedWalking = 5f;
	public float jumpForce = 8.5f;
	public float jumpCutoff = 2.2f;
	public Vector2 wallJumpForce = new Vector2 (8f, 10f);

	private CharacterController2D _controller;
	private PlayerParameters2D _overrideParameters;
	private bool _isFacingRight;
	private bool _isRunning;
	private int _normalizedHorizontalSpeed;
	private float _jumpIn;
	private bool _canJump
	{
		get
		{
			if (Parameters.JumpRestrictions == PlayerParameters2D.JumpBehavior.CanJumpAnywhere)
				return _jumpIn <= 0;
			
			if (Parameters.JumpRestrictions == PlayerParameters2D.JumpBehavior.CanJumpOnGround)
				return _controller.State.IsGrounded || _controller.State.IsHuggingWall;
			
			return false;
		}
	}

	void Start () {
		_controller = GetComponent<CharacterController2D> ();
		_isFacingRight = transform.localScale.x > 0;
	}

	void Update () {
		_jumpIn -= Time.deltaTime;
		HandleInput ();

		var acceleration = _isRunning ? SpeedAccelerationRunning : SpeedAccelerationWalking; //TODO - moving opposite direction?
		var maxSpeed = _isRunning ? MaxSpeedRunning : MaxSpeedWalking;
		if (_normalizedHorizontalSpeed != 0)
			_controller.SetHorizontalVelocity (Mathf.Lerp (_controller.Velocity.x, _normalizedHorizontalSpeed * maxSpeed, Time.deltaTime * acceleration));
		else if (_controller.State.IsGrounded)
			_controller.SetHorizontalVelocity(0);
	}

	public void Jump(float force)
	{
		_controller.Jump (force);
		_jumpIn = Parameters.JumpFrequency;
	}
	
	public void Jump(Vector2 force)
	{
		_controller.Jump (force);
		_jumpIn = Parameters.JumpFrequency;
	}

	private void HandleInput() {
		if (Input.GetKey (KeyCode.RightArrow)) {
			_normalizedHorizontalSpeed = 1;
			if (!_isFacingRight)
				Flip ();
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			_normalizedHorizontalSpeed = -1;
			if (_isFacingRight)
				Flip ();
		} else {
			_normalizedHorizontalSpeed = 0;
		}

		_isRunning = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);

		if (_canJump && Input.GetKeyDown (KeyCode.Space)) {
			if (_controller.State.IsGrounded)
				_controller.Jump(jumpForce);
			else if (_controller.State.IsHuggingWallRight)
				_controller.Jump(new Vector2(-wallJumpForce.x, wallJumpForce.y));
			else if (_controller.State.IsHuggingWallLeft)
				_controller.Jump(wallJumpForce);
		}

		if (Input.GetKey (KeyCode.G))
			_controller.FireGrapple (Parameters.grappleAngleRadians, Parameters.grappleMaxLength);

		if (!Input.GetKey (KeyCode.Space) && _controller.Velocity.y > jumpCutoff)
			_controller.SetVerticalVelocity (jumpCutoff);
	}

	private void Flip() {
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}
}
