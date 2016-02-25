using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public PlayerParameters2D DefaultParameters;
	public PlayerParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }
	private float SpeedAccelerationRunning = 4f;
	private float SpeedAccelerationWalking = 3.5f;
	private float MaxSpeedRunning = 8f;
	private float MaxSpeedWalking = 6f;
	private float jumpForce = 8.5f;
	private float jumpCutoff = 2.2f;
	private float grappleDirectionChangeSpeed = 360.0f;
	private float wallStickTime = 0.1f;
	private Vector2 wallJumpForceRunning = new Vector2 (8f, 10f);
	private Vector2 wallJumpForceWalking = new Vector2 (5f, 9f);
	public Grapple grapple;

	private float _grappleMaxLength = 5f;
	private float _grappleRetractSpeed = 1f;
	private float _grappleExtendSpeed = 1f;
	private float _grappleAngleDegrees = 45f;

	private bool _alive = true;
	private CharacterController2D _controller;
	private PlayerParameters2D _overrideParameters;
	private bool _isFacingRight;
	private bool _isRunning;
	private int _normalizedHorizontalSpeed;
	private float _jumpIn;
	private bool _grappleInPosession = false;
	private float _grappleAngle;
	private bool _canJump
	{
		get
		{
			if (Parameters.JumpRestrictions == PlayerParameters2D.JumpBehavior.CanJumpAnywhere)
				return _jumpIn <= 0;
			
			if (Parameters.JumpRestrictions == PlayerParameters2D.JumpBehavior.CanJumpOnGround)
				return _controller.State.IsGrounded || _controller.State.IsHuggingWall || _controller.State.IsGrappling;
			
			return false;
		}
	}

	private float _leaveWallIn;
	private bool _canLeaveWall
	{
		get
		{
			if (!_controller.State.IsHuggingWall) {
				//should never end up here as not on wall!
				return true;
			}

			var keyCode = _controller.State.IsHuggingWallRight ? KeyCode.LeftArrow : KeyCode.RightArrow;

			if (_leaveWallIn <= 0) {
				return true;
			}
			if (Input.GetKey(keyCode)) {
				_leaveWallIn -= Time.deltaTime;
			} else {
				_leaveWallIn = wallStickTime;
			}
			return false;
		}
	}

	void Start () {
		_controller = GetComponent<CharacterController2D> ();
		_isFacingRight = transform.localScale.x > 0;
		_leaveWallIn = wallStickTime;
		if (LevelManager.Instance && LevelManager.Instance.PlayerHasPermanentGrapple ()) {
			_grappleInPosession = true;
		}
	}

	void Update () {
		if (LevelManager.Instance && LevelManager.Instance.ResetInProgress ()) {
			Debug.Log ("player resetting due to level reset");
			Reset ();
		}
		if (_alive && !Paused()) {
			_jumpIn -= Time.deltaTime;
			HandleInput ();

			if (_normalizedHorizontalSpeed != 0) {
				if (!_controller.State.IsHuggingWall || _canLeaveWall) {
					var acceleration = _isRunning ? SpeedAccelerationRunning : SpeedAccelerationWalking;
					var maxSpeed = _isRunning ? MaxSpeedRunning : MaxSpeedWalking;
					_controller.SetHorizontalVelocity (Mathf.Lerp (_controller.Velocity.x, _normalizedHorizontalSpeed * maxSpeed, Time.deltaTime * acceleration));
					_leaveWallIn = wallStickTime;
				}
			} else if (_controller.State.IsGrounded) {
				_controller.SetHorizontalVelocity (0);
			}
		}
	}

	bool Paused() {
		if (!LevelManager.Instance) {
			return false;
		}
		if (LevelManager.Instance.Paused ()) {
			return true;
		}
		return false;
	}

	void LateUpdate () {
		UpdateGrapple ();
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

	public void Die (){
		if (_alive) {
			Debug.Log ("You dead!!!!");
			_controller.ReleaseGrapple ();
			_alive = false;
			_controller.Die();
		}
	}

	private void UpdateGrapple () {
		if (!_alive) {
			grapple.isActive = false;
			return;
		}
		if (_grappleInPosession && _controller.State.IsGrappling) {
			grapple.isActive = true;
			grapple.SetEnds (_controller.grappleConstraint.anchor, transform.position);
		} else if (_grappleInPosession) {
			grapple.isActive = true;
			grapple.SetByAngleDegreesAndLength (_grappleAngle, (Vector2) transform.position, 0.3f);
		} else {
			grapple.isActive = false;
		}
	}

	public void CollectGrapple() {
		_grappleInPosession = true;
	}

	public void Respawn () {
		Debug.Log ("Respawning");
		_alive = true;
		_controller.Respawn ();
		_controller.Live ();
		_controller.SetVelocity(new Vector2 (0f, 0f));
	}

	public void Reset () {
		_controller.ReleaseGrapple ();
		_grappleInPosession = false;
		UpdateGrapple ();
		Disable ();
		Respawn ();
	}

	public void Disable() {
		_controller.Disable ();
	}

	private void HandleInput() {
		if (Input.GetKey (KeyCode.RightArrow)) {
			_normalizedHorizontalSpeed = 1;
			if (!_isFacingRight && !_controller.State.IsHuggingWallLeft) {
				Flip ();
			}
			if (_grappleInPosession) {
				ShiftGrappleRight();
			}
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			_normalizedHorizontalSpeed = -1;
			if (_isFacingRight && !_controller.State.IsHuggingWallRight) {
				Flip ();
			}
			if (_grappleInPosession) {
				ShiftGrappleLeft();
			}
		} else {
			_normalizedHorizontalSpeed = 0;
			if (_grappleInPosession) {
				ShiftGrappleTowardsCentre();
			}
		}

		_isRunning = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);

		if (_canJump && Input.GetKeyDown (KeyCode.Space)) {
			var wallJumpForce = _isRunning ? wallJumpForceRunning : wallJumpForceWalking;
			Debug.Log (wallJumpForce);
			if (_controller.State.IsHuggingWallRight && !_controller.State.IsGrounded)
				_controller.Jump(new Vector2(-wallJumpForce.x, wallJumpForce.y));
			else if (_controller.State.IsHuggingWallLeft && !_controller.State.IsGrounded)
				_controller.Jump(wallJumpForce);
			else
				_controller.Jump(jumpForce);
		}

		if (!Input.GetKey (KeyCode.Space) && _controller.Velocity.y > jumpCutoff)
			_controller.SetVerticalVelocity (jumpCutoff);

		if (_controller.State.IsGrappling) {
			if (Input.GetKey (KeyCode.UpArrow))
				_controller.RetractGrapple(_grappleRetractSpeed);
			else if (Input.GetKey (KeyCode.DownArrow))
			    _controller.ExtendGrapple(_grappleExtendSpeed);
		}

		if (_grappleInPosession && !_controller.State.IsGrappling && Input.GetKey (KeyCode.UpArrow)){
			var missPoint = _controller.FireGrapple (_grappleAngle, _grappleMaxLength);
			if (missPoint.x != -1 || missPoint.y != -1) {
				grapple.RegisterMiss(missPoint);
			}
		}
	}

	private void ShiftGrappleRight() {
		_grappleAngle += grappleDirectionChangeSpeed * Time.deltaTime;
		_grappleAngle = Mathf.Clamp (_grappleAngle, -1 * _grappleAngleDegrees, _grappleAngleDegrees);
	}

	private void ShiftGrappleLeft() {
		_grappleAngle -= grappleDirectionChangeSpeed * Time.deltaTime;
		_grappleAngle = Mathf.Clamp (_grappleAngle, -_grappleAngleDegrees, _grappleAngleDegrees);
	}

	private void ShiftGrappleTowardsCentre() {
		_grappleAngle = 0;
	}

	private void Flip() {
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}
}
