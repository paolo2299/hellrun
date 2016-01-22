using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class PlayerParameters2D
{
	public enum JumpBehavior
	{
		CanJumpOnGround,
		CanJumpAnywhere
	}
	
	public JumpBehavior JumpRestrictions = JumpBehavior.CanJumpOnGround;
	public float JumpFrequency = .25f;
	public float WallStickTime = .25f;
}