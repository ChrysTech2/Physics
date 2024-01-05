using System;
using UnityEngine;

public class Settings : MonoBehaviour{

	// Global Settings
	public double secondsPerFrame, lineDuration, lineThickness;
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
		return attractionGravityConstant * body1.mass *body2.mass / Math.Pow(body1.Distance(body2), 2);
	}

	public double AirDrag(Body body){
		return Math.Pow(body.velocity.magnitude, 2) * dragCoefficient * fluidDensity * body.area / (2 * body.mass);
	}
 
	public bool isApplyingThrust = false;
	public bool isRotatingForward = false;
	public bool isRotatingBackward = false;

	public void SetIsApplyingThrust(bool value){
		isApplyingThrust = value;
	}

	public void SetIsRotatingForward(bool value){
		isRotatingForward = value;
	}

	public void SetIsRotatingBackward(bool value){
		isRotatingBackward = value;
	}
	
	public Vector2Double thrustDirection = Vector2Double.up;
	public double thrustAcceleration;
}
