using System;
using UnityEngine;

public class Settings : MonoBehaviour{

	// Global Settings
	public double secondsPerFrame;
	public int gravityMode, borderMode;
	public double gravityAcceleration, gravityAngle;
	public double attractionGravityConstant;
	public double fluidDensity, dragCoefficient;
	public double coefOfRestitution;
	public bool calculateCollisions, mergeBodiesInCollisions;
	public Vector2Double border;

	public double touchMultiplier;
	public bool showCenterOfGravity;
	
	public Vector2Double gravity, gravityDirection;

	public double AttractionGravity(Body body1, Body body2){
		return attractionGravityConstant * body2.mass / Math.Pow(body1.Distance(body2), 2);
	}

	public double AirDrag(Body body){
		return Math.Pow(body.velocity.magnitude, 2) * dragCoefficient * fluidDensity * body.Area / (2 * body.mass);
	}
}
