﻿using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ControllerParameters2D
{	
	public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);
	
	[Range(0, 90)]
	public float SlopeLimit = 30;

	public float Gravity = -25f;
	public float MaxFallingVelocity = 25f;
	public float JumpFrequency = .25f;	
}
