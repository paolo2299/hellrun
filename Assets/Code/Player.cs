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
	public float grappleDirectionChangeSpeed = 100.0f;
	public Vector2 wallJumpForce = new Vector2 (8f, 10f);
	public Grapple grapple;

	private bool _alive = true;
	private CharacterController2D _controller;
	private PlayerParameters2D _overrideParameters;
	private bool _isFacingRight;
	private bool _isRunning;
	private int _normalizedHorizontalSpeed;
	private float _jumpIn;
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
				_leaveWallIn = Parameters.WallStickTime;
			}
			return false;
		}
	}

	void Start () {
		_controller = GetComponent<CharacterController2D> ();
		_isFacingRight = transform.localScale.x > 0;
		_leaveWallIn = Parameters.WallStickTime;
	}

	void Update () {
		if (LevelManager.Instance && LevelManager.Instance.ResetInProgress ()) {
			Reset ();
		}
		if (_alive) {
			_jumpIn -= Time.deltaTime;
			HandleInput ();

			if (_normalizedHorizontalSpeed != 0) {
				if (!_controller.State.IsHuggingWall || _canLeaveWall) {
					var acceleration = _isRunning ? SpeedAccelerationRunning : SpeedAccelerationWalking;
					var maxSpeed = _isRunning ? MaxSpeedRunning : MaxSpeedWalking;
					_controller.SetHorizontalVelocity (Mathf.Lerp (_controller.Velocity.x, _normalizedHorizontalSpeed * maxSpeed, Time.deltaTime * acceleration));
					_leaveWallIn = Parameters.WallStickTime;
				}
			} else if (_controller.State.IsGrounded) {
				_controller.SetHorizontalVelocity (0);
			}
			
			if (!_controller.State.IsGrappling) {
				TargetGrapple();
			}
		}
	}

	void LateUpdate () {
		UpdateGrapple ();
	}
	
	private void TargetGrapple() {
		grapple.SetByAngleDegreesAndLength (_grappleAngle, (Vector2) transform.position, 0.3f);
		if (_normalizedHorizontalSpeed > 0)
			TargetGrappleRight ();
		else if (_normalizedHorizontalSpeed < 0)
			TargetGrappleLeft ();
		else
			TargetGrappleUp ();
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
		if (grapple && _controller.State.IsGrappling) {
			grapple.isActive = true;
			grapple.SetEnds (_controller.grappleConstraint.anchor, transform.position);
		} else if (grapple) {
			grapple.isActive = true;
			TargetGrapple();
		} else {
			grapple.isActive = false;
		}
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
			ShiftGrappleRight();
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			_normalizedHorizontalSpeed = -1;
			if (_isFacingRight && !_controller.State.IsHuggingWallRight) {
				Flip ();
			}
			ShiftGrappleLeft();
		} else {
			_normalizedHorizontalSpeed = 0;
			ShiftGrappleTowardsCentre();
		}

		_isRunning = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift);

		if (_canJump && Input.GetKeyDown (KeyCode.Space)) {
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
				_controller.RetractGrapple(Parameters.grappleRetractSpeed);
			else if (Input.GetKey (KeyCode.DownArrow))
			    _controller.ExtendGrapple(Parameters.grappleExtendSpeed);
		}

		if (!_controller.State.IsGrappling && Input.GetKey (KeyCode.UpArrow)){
			_controller.FireGrapple (_grappleAngle, Parameters.grappleMaxLength);
		}
	}

	private void ShiftGrappleRight() {
		_grappleAngle += grappleDirectionChangeSpeed * Time.deltaTime;
		_grappleAngle = Mathf.Clamp (_grappleAngle, -Parameters.grappleAngleDegrees, Parameters.grappleAngleDegrees);
	}

	private void ShiftGrappleLeft() {
		_grappleAngle -= grappleDirectionChangeSpeed * Time.deltaTime;
		_grappleAngle = Mathf.Clamp (_grappleAngle, -Parameters.grappleAngleDegrees, Parameters.grappleAngleDegrees);
	}

	private void ShiftGrappleTowardsCentre() {
		//if (_grappleAngle > 0) {
		//	ShiftGrappleLeft ();
		//	_grappleAngle = Mathf.Max (_grappleAngle, 0);
		//} else if (_grappleAngle < 0) {
		//	ShiftGrappleRight ();
		//	_grappleAngle = Mathf.Min (_grappleAngle, 0);
		//}
		//TODO settle on what to do here
		_grappleAngle = 0;
	}

	//TODO fix or kill target grapple functionality
	private void TargetGrappleRight() {
		var direction = new Vector2(Mathf.Cos (Parameters.grappleAngleRadians), Mathf.Sin (Parameters.grappleAngleRadians));
		_controller.TargetGrapple (direction, Parameters.grappleMaxLength);
	}
	
	private void TargetGrappleLeft() {
		var direction = new Vector2(-Mathf.Cos (Parameters.grappleAngleRadians), Mathf.Sin (Parameters.grappleAngleRadians));
		_controller.TargetGrapple (direction, Parameters.grappleMaxLength);
	}
	
	private void TargetGrappleUp() {
		_controller.TargetGrapple (Vector2.up, Parameters.grappleMaxLength);
	}

	private void Flip() {
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacingRight = transform.localScale.x > 0;
	}
}
