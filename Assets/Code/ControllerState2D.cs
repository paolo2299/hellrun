using UnityEngine;
using System.Collections;

public class ControllerState2D
{
	public bool IsCollidingRight { get; set; }
	public bool IsCollidingLeft { get; set; }
	public bool IsCollidingAbove { get; set; }
	public bool IsCollidingBelow { get; set; }
	public bool IsMovingDownSlope { get; set; }
	public bool IsMovingUpSlope { get; set; }
	public bool IsGrounded { get { return IsCollidingBelow; } }
	public bool IsHuggingWallRight { get; set; }
	public bool IsHuggingWallLeft { get; set; }
	public bool IsHuggingWall { get { return IsHuggingWallLeft || IsHuggingWallRight; } }
	public bool IsGrappling { get; set; }
	public float SlopeAngle { get; set; }
	public bool HasCollisions {
		get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; }
	}
	public Vector2 PreviousGrapplingPosition { get; set; }
	public Vector2 PreviousGrapplingVelocity { get; set; }

	public void Reset()
	{
		IsMovingUpSlope =
			IsMovingDownSlope =
			IsCollidingLeft = 
			IsCollidingRight =
			IsCollidingAbove =
			IsCollidingBelow =
			IsHuggingWallLeft = 
			IsHuggingWallRight = false;

		SlopeAngle = 0;
	}

	public override string ToString ()
	{
		return string.Format ("[ControllerState2D: r={0}, l={1}, a={2}, b={3}, down-slope={4}, up-slope={5}, angle={6}]", 
		                      IsCollidingRight, 
		                      IsCollidingLeft, 
		                      IsCollidingAbove, 
		                      IsCollidingBelow, 
		                      IsMovingDownSlope, 
		                      IsMovingUpSlope, 
		                      SlopeAngle);
	}
}
