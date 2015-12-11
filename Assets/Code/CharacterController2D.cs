using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
	private const float SkinWidth = .02f;
	private const int TotalHorizontalRays = 8;
	private const int TotalVerticalRays = 4;
	
	private static readonly float SlopeLimitTangant = Mathf.Tan(75f * Mathf.Deg2Rad);
	
	public LayerMask PlatformMask;
	public Animator animator;
	public ControllerParameters2D DefaultParameters;
	
	public ControllerState2D State { get; private set; }
	public Vector2 Velocity { get { return _velocity; } }
	public ControllerParameters2D Parameters { get { return _overrideParameters ?? DefaultParameters; } }
	public GameObject StandingOn { get; private set; }
	public GameObject GrapplingOn { get; private set; }
	public GrappleConstraint grappleConstraint { get; private set; }

	private Vector3 _originalPos;
	private Vector2 _velocity;
	private Transform _transform;
	private Vector3 _localScale;
	private BoxCollider2D _boxCollider;
	private ControllerParameters2D _overrideParameters;
	private GameObject _lastStandingOn;
	private bool _alive = true;

	private Vector3
		_activeGlobalPlatformPoint,
		_activeLocalPlatformPoint,
		_activeGlobalGrapplePoint,
		_activeLocalGrapplePoint;
	
	private Vector3
		_raycastTopLeft,
		_raycastBottomRight,
		_raycastBottomLeft;
	
	private float
		_verticalDistanceBetweenRays,
		_horizontalDistanceBetweenRays;
	
	public void Awake()
	{
		State = new ControllerState2D();
		_transform = transform;
		_originalPos = transform.position;
		_localScale = transform.localScale;
		_boxCollider = GetComponent<BoxCollider2D>();
		
		var colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2 * SkinWidth);
		_horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);
		
		var colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2 * SkinWidth);
		_verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);
	}

	public void LateUpdate()
	{
		if (State.IsGrappling) {
			UpdateForGrapple ();
		} else {
			_velocity.y += Parameters.Gravity * Time.deltaTime;
			_velocity.y = Mathf.Clamp(_velocity.y, -Parameters.MaxFallingVelocity, float.MaxValue);
			var deltaMovement = Velocity * Time.deltaTime;
			Move (deltaMovement);
		}
		UpdateAnimator ();
	}

	private void UpdateAnimator () 
	{
		if (animator) {
			if(_alive) {
				animator.SetBool("isDead", false);
				animator.SetFloat("playerSpeed", Mathf.Abs(Velocity.x));
				animator.SetBool("isGrounded", State.IsGrounded);
				animator.SetBool("isHuggingWall", State.IsHuggingWall);
				var isRising = false;
				var isFalling = false;
				if (Velocity.y > 0) {
					isRising = true;
				} else {
					isFalling = true;
				}
				animator.SetBool("isRising", isRising);
				animator.SetBool("isFalling", isFalling);
			} else {
				animator.SetBool("isDead", true);
			}
		}
	}

	private void UpdateForGrapple()
	{
		if (!State.IsGrappling)
			return;

		if (GrapplingOn == null) {
			ReleaseGrapple();
			return;
		}

		AdjustGrappleForPlatforms ();
		AdjustPositionForGrappleLength ();
		
		var grappleDirectionStart = (Vector2) _transform.position - grappleConstraint.anchor;

		var grappleAngleDegrees = 0f;
		if (grappleDirectionStart.y < 0)
			grappleAngleDegrees = Vector2.Angle (Vector2.down, grappleDirectionStart);
		else
			grappleAngleDegrees = Vector2.Angle (Vector2.up, grappleDirectionStart);
		var grappleAngleRadians = grappleAngleDegrees * 2 * Mathf.PI / 360f;
		
		var gravityForce = -Parameters.Gravity * Vector2.down;
		var tensionForceMagnitude = -Parameters.Gravity * Mathf.Cos (grappleAngleRadians) + (Velocity.magnitude * Velocity.magnitude) / grappleConstraint.length;
		var tensionForceDirection = -grappleDirectionStart.normalized;
		var tensionForce = tensionForceMagnitude * tensionForceDirection;
		
		var resultantForce = gravityForce + tensionForce;
		
		AddForce (resultantForce * Time.deltaTime);

		var deltaMovement = Velocity * Time.deltaTime;
		Move (deltaMovement);
	
		//Adjust Velocity for the fact that we have moved in an arc, not a straight line
		var grappleDirectionEnd = (Vector2) _transform.position - grappleConstraint.anchor;
		var angleTraversedRadians = Vector2.Angle (grappleDirectionStart, grappleDirectionEnd) * 2 * Mathf.PI / 360f;
		var distanceTraversed = angleTraversedRadians * grappleConstraint.length;

		var grapplePerp = new Vector2 (-grappleDirectionEnd.y, grappleDirectionEnd.x).normalized;
		var currentSpeed = distanceTraversed / Time.deltaTime;
		
		var grappleDirectionTowards = (Vector2)_transform.position + Velocity * Time.deltaTime - grappleConstraint.anchor;
		var clockwiseness = Mathf.Sign (grappleDirectionEnd.y * grappleDirectionTowards.x - grappleDirectionEnd.x * grappleDirectionTowards.y);
		var tangentialVelocity = -currentSpeed * clockwiseness * grapplePerp;
		
		SetVelocity (tangentialVelocity);
	}

	private void AdjustPositionForGrappleLength()
	{
		var grappleVector = grappleConstraint.anchor - (Vector2) _transform.position;
		var grappleNormal = grappleVector.normalized;
		var adjustmentSize = grappleVector.magnitude - grappleConstraint.length;
		var adjustment = grappleNormal * adjustmentSize;
		Move (adjustment, false);
		var newGrappleVector = grappleConstraint.anchor - (Vector2) _transform.position;
		var newGrappleLength = newGrappleVector.magnitude;

		if (Mathf.Abs(newGrappleLength - grappleConstraint.length) > 0.001f) {
			//Must have hit something so adjust the grapple instead
			grappleConstraint.Extend(newGrappleLength - grappleConstraint.length);
			//TODO if grapple exceeds maximum length then release
		}
	}
	
	public void AddForce(Vector2 force)
	{
		_velocity += force;
	}

	public void SetVelocity(Vector2 v)
	{
		_velocity = v;
	}

	public void SetHorizontalVelocity(float x)
	{
		_velocity.x = x;
	}
	
	public void SetVerticalVelocity(float y)
	{
		_velocity.y = y;
	}
	
	public void Jump(float force)
	{
		if (State.IsGrappling)
			ReleaseGrapple ();

		SetVerticalVelocity(force);
	}

	public void Jump(Vector2 force)
	{
		if (State.IsGrappling)
			ReleaseGrapple ();

		SetVelocity(force);
	}

	public void Respawn() {
		Live ();
		transform.position = _originalPos;
	}

	public void Die() {
		Disable ();
		_alive = false;
		UpdateAnimator ();
	}
	
	public void Live() {
		Enable ();
		_alive = true;
	}

	public void Disable() {
		_boxCollider.enabled = false;
		enabled = false;
	}

	public void Enable() {
		_boxCollider.enabled = true;
		enabled = true;
	}

	public Vector2 FireGrapple(Vector2 direction, float maxLength) {
		Debug.Log (maxLength);
		var origin = _transform.position;
		//Debug.DrawRay(origin, direction * maxLength, Color.cyan);
		
		var raycastHit = Physics2D.Raycast(origin, direction, maxLength, PlatformMask);
		if (!raycastHit) {
			return (Vector2) origin + (direction * maxLength);
		}
		
		var anchor = raycastHit.point;
		var length = raycastHit.distance;
		grappleConstraint = new GrappleConstraint (anchor, length, maxLength);
		State.IsGrappling = true;
		GrapplingOn = raycastHit.collider.gameObject;
		SetGrapplePoints();
		return new Vector2(-1, -1);
	}

	public Vector2 FireGrapple(float angleInDegrees, float maxLength) {
		var origin = _transform.position;
		//Debug.DrawRay(origin, direction * maxLength, Color.cyan);
		var angleInRadians = angleInDegrees * 2 * Mathf.PI / 360f;
		var direction = new Vector2 (Mathf.Sin (angleInRadians), Mathf.Cos (angleInRadians));

		var raycastHit = Physics2D.Raycast(origin, direction, maxLength, PlatformMask);
		if (!raycastHit)
			return (Vector2) origin + (direction * maxLength);
		
		var anchor = raycastHit.point;
		var length = raycastHit.distance;
		grappleConstraint = new GrappleConstraint (anchor, length, maxLength);
		State.IsGrappling = true;
		GrapplingOn = raycastHit.collider.gameObject;
		SetGrapplePoints();
		return new Vector2(-1, -1);
	}

	public void ReleaseGrapple () {
		grappleConstraint = null;
		State.IsGrappling = false;
		GrapplingOn = null;
		State.PreviousGrapplingPosition = Vector2.zero;
		State.PreviousGrapplingVelocity = Vector2.zero;
	}

	public void RetractGrapple(float speed)
	{
		if (!State.IsGrappling)
			return;

		grappleConstraint.Retract(speed * Time.deltaTime);
	}

	public void ExtendGrapple(float speed)
	{
		if (!State.IsGrappling)
			return;
		
		grappleConstraint.Extend(speed * Time.deltaTime);
	}
	
	private void Move(Vector2 deltaMovement, bool updateVelocity = true)
	{
		var wasGrounded = State.IsCollidingBelow;
		State.Reset();

		HandlePlatforms();
		CalculateRayOrigins();
		
		if (deltaMovement.y < 0 && wasGrounded)
			HandleVerticalSlope(ref deltaMovement);
		
		if (Mathf.Abs(deltaMovement.x) > .001f)
			MoveHorizontally(ref deltaMovement);
		
		MoveVertically(ref deltaMovement);

		DetectWalls();

		//If moving platforms have moved into the character correct accordingly
		CorrectHorizontalPlacement(ref deltaMovement, true);
		CorrectHorizontalPlacement(ref deltaMovement, false);

		_transform.Translate(deltaMovement, Space.World);
		
		if (Time.deltaTime > 0 && updateVelocity) {
			_velocity = deltaMovement / Time.deltaTime;
		}
		
		_velocity.x = Mathf.Clamp(_velocity.x, -Parameters.MaxVelocity.x, Parameters.MaxVelocity.x);
		_velocity.y = Mathf.Clamp(_velocity.y, -Parameters.MaxVelocity.y, Parameters.MaxVelocity.y);

		if (State.IsMovingUpSlope)
			_velocity.y = 0;
		
		if (StandingOn != null)
		{
			_activeGlobalPlatformPoint = transform.position;
			_activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);
			
			if (_lastStandingOn != StandingOn)
			{
				if (_lastStandingOn != null)
					_lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
				
				StandingOn.SendMessage("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
				_lastStandingOn = StandingOn;
			}
			else if (StandingOn != null)
				StandingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);
		}
		else if (_lastStandingOn != null)
		{
			_lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);
			_lastStandingOn = null;
		}

		if (GrapplingOn != null)
		{
			SetGrapplePoints();
		}
	}

	private void SetGrapplePoints()
	{
		_activeGlobalGrapplePoint = grappleConstraint.anchor;
		_activeLocalGrapplePoint = GrapplingOn.transform.InverseTransformPoint (grappleConstraint.anchor);
	}
	
	private void HandlePlatforms()
	{
		if (StandingOn != null)
		{
			var newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
			var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
			
			if (moveDistance != Vector3.zero)
				transform.Translate(moveDistance, Space.World);
		}
		
		StandingOn = null;
	}

	private void AdjustGrappleForPlatforms()
	{	
		if (GrapplingOn != null && _activeGlobalGrapplePoint != Vector3.zero) 
		{
			var newGlobalGrapplePoint = GrapplingOn.transform.TransformPoint(_activeLocalGrapplePoint);
			var moveDistance = newGlobalGrapplePoint - _activeGlobalGrapplePoint;
			
			if (moveDistance != Vector3.zero)
			{
				grappleConstraint.SetAnchor(grappleConstraint.anchor + (Vector2) moveDistance);
			}
		}
	}
	
	private void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight)
	{
		var halfWidth = (_boxCollider.size.x * _localScale.x) / 2f;
		var rayOrigin = isRight ? _raycastBottomRight : _raycastBottomLeft;
		
		if (isRight)
			rayOrigin.x -= halfWidth;
		else
			rayOrigin.x += halfWidth;
		
		var rayDirection = isRight ? Vector2.right : -Vector2.right;
		var offset = 0f;
		
		for (var i = 1; i < TotalHorizontalRays - 1; i++)
		{
			var rayVector = new Vector2(deltaMovement.x + rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
			//Debug.DrawRay(rayVector, rayDirection * halfWidth, isRight ? Color.cyan : Color.magenta);
			
			var raycastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatformMask);
			if (!raycastHit)
				continue;
			
			offset = isRight ? ((raycastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - raycastHit.point.x));
		}
		
		deltaMovement.x += offset;
	}
	
	private void CalculateRayOrigins()
	{
		var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y)) / 2;
		var center = new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
		
		_raycastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
		_raycastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
		_raycastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);
	}
	
	private void MoveHorizontally(ref Vector2 deltaMovement)
	{
		var isGoingRight = deltaMovement.x > 0;
		var rayDistance = Mathf.Abs(deltaMovement.x) + SkinWidth;
		var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		var rayOrigin = isGoingRight ? _raycastBottomRight : _raycastBottomLeft;
		
		for (var i = 0; i < TotalHorizontalRays; i++)
		{
			var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
			//Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);
			
			var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
			if (!rayCastHit)
				continue;
			
			if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
				break;
			
			deltaMovement.x = rayCastHit.point.x - rayVector.x;
			rayDistance = Mathf.Abs(deltaMovement.x);
			
			if (isGoingRight)
			{
				deltaMovement.x -= SkinWidth;
				State.IsCollidingRight = true;
			}
			else
			{
				deltaMovement.x += SkinWidth;
				State.IsCollidingLeft = true;
			}
			
			if (rayDistance < SkinWidth + .0001f)
				break;
		}
	}

	private void DetectWalls()
	{
		DetectWalls (true);
		DetectWalls (false);
	}

	private void DetectWalls(bool onRight) 
	{
		var rayDistance = SkinWidth * 1.5f;
		var rayDirection = onRight ? Vector2.right : Vector2.left;
		var rayOrigin = onRight ? _raycastBottomRight : _raycastBottomLeft;
		
		for (var i = 0; i < TotalHorizontalRays; i++)
		{
			var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
			//Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.green);
			
			var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
			if (rayCastHit) {
				if (onRight)
					State.IsHuggingWallRight = true;
				else
					State.IsHuggingWallLeft = true;
				break;
			}
		}
	}

	private void MoveVertically(ref Vector2 deltaMovement)
	{
		var isGoingUp = deltaMovement.y > 0;
		var rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;
		var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		var rayOrigin = isGoingUp ? _raycastTopLeft : _raycastBottomLeft;
		
		rayOrigin.x += deltaMovement.x;
		
		var standingOnDistance = float.MaxValue;
		for (var i = 0; i < TotalVerticalRays; i++)
		{
			var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
			//Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);
			
			var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
			if (!raycastHit)
				continue;
			
			if (!isGoingUp)
			{
				var verticalDistanceToHit = _transform.position.y - raycastHit.point.y;
				if (verticalDistanceToHit < standingOnDistance)
				{
					standingOnDistance = verticalDistanceToHit;
					StandingOn = raycastHit.collider.gameObject;
				}
			}
			
			deltaMovement.y = raycastHit.point.y - rayVector.y;
			rayDistance = Mathf.Abs(deltaMovement.y);
			
			if (isGoingUp)
			{
				deltaMovement.y -= SkinWidth;
				State.IsCollidingAbove = true;
			}
			else
			{
				deltaMovement.y += SkinWidth;
				State.IsCollidingBelow = true;
			}
			
			if (!isGoingUp && deltaMovement.y > .0001f)
				State.IsMovingUpSlope = true;
			
			if (rayDistance < SkinWidth + .0001f)
				break;
		}
	}
	
	private void HandleVerticalSlope(ref Vector2 deltaMovement)
	{
		var center = (_raycastBottomLeft.x + _raycastBottomRight.x) / 2;
		var direction = -Vector2.up;
		
		var slopeDistance = SlopeLimitTangant * (_raycastBottomRight.x - center);
		var slopeRayVector = new Vector2(center, _raycastBottomLeft.y);
		
		//Debug.DrawRay(slopeRayVector, direction * slopeDistance, Color.yellow);
		
		var raycastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, PlatformMask);
		if (!raycastHit)
			return;
		
		// ReSharper disable CompareOfFloatsByEqualityOperator
		
		var isMovingDownSlope = Mathf.Sign(raycastHit.normal.x) == Mathf.Sign(deltaMovement.x);
		if (!isMovingDownSlope)
			return;
		
		var angle = Vector2.Angle(raycastHit.normal, Vector2.up);
		if (Mathf.Abs(angle) < .0001f)
			return;
		
		State.IsMovingDownSlope = true;
		State.SlopeAngle = angle;
		deltaMovement.y = raycastHit.point.y - slopeRayVector.y;
	}
	
	private bool HandleHorizontalSlope(ref Vector2 deltaMovement, float angle, bool isGoingRight)
	{
		if (Mathf.RoundToInt(angle) == 90)
			return false;
		
		if (angle > Parameters.SlopeLimit)
		{
			deltaMovement.x = 0;
			return true;
		}
		
		if (deltaMovement.y > .07f)
			return true;
		
		deltaMovement.x += isGoingRight ? -SkinWidth : SkinWidth;
		deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
		State.IsMovingUpSlope = true;
		State.IsCollidingBelow = true;
		return true;
	}
	
	public void OnTriggerEnter2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhsyicsVolume2D>();
		if (parameters == null)
			return;
		
		_overrideParameters = parameters.Parameters;
	}
	
	public void OnTriggerExit2D(Collider2D other)
	{
		var parameters = other.gameObject.GetComponent<ControllerPhsyicsVolume2D>();
		if (parameters == null)
			return;
		
		_overrideParameters = null;
	}
}