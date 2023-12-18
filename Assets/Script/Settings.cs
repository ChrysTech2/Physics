
using System;
using UnityEngine;

public class Settings : MonoBehaviour{

	// Global Settings
	public double secondsPerFrame;
	public double globalGravity, globalGravityAngle;
	public double airDensity, dragCoefficient;
	public double gravityConstant;
	public double coefOfRestitution;
	public bool calculateCollisions, mergeBodiesInCollisions;
	public bool calculateBuoyancy;
	

	public Vector2Double globalGravityVector{
		get{
			return Vector2Double.ToVector2Double(globalGravity, globalGravityAngle * Math.PI/180);
		}
	}
	
}
