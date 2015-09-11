﻿using System;
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

	public int grappleAngleDegrees = 45;
	public float grappleAngleRadians {
		get { return (float) (grappleAngleDegrees * (Math.PI / 180)); }
	}
	public float grappleMaxLength = 20f;
}