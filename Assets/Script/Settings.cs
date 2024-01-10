using System;
using UnityEngine;

public class Settings : MonoBehaviour{

	// Global Settings
	public double secondsPerFrame, lineDuration, lineThickness;
	public int gravityMode, borderMode;
	public double gravityAcceleration, gravityAngle;
	public double attractionGravityConstant;
	public double fluidDensity, dragCoefficient;
	public double coefOfRestitution, borderCoefOfRestitution, frictionCoefficient;
	public bool calculateCollisions, mergeBodiesInCollisions;
	public Vector2Double border;

	public double touchMultiplier;
	public bool showCenterOfGravity;
	public bool calculateBuoyancy;
	
	public Vector2Double gravity, gravityDirection;

	public double AttractionGravity(Body body1, Body body2){
		return attractionGravityConstant * body1.mass *body2.mass / Math.Pow(body1.Distance(body2), 2);
	}

	public double AirDrag(Body body){
		return Math.Pow(body.velocity.magnitude, 2) * dragCoefficient * fluidDensity * body.area / (2 * body.mass);
	}
 
	public bool isApplyingThrustForward = false;
	public bool isApplyingThrustBackward = false;
	public bool isApplyingThrustRight = false;
	public bool isApplyingThrustLeft = false;

	public bool isRotatingForward = false;
	public bool isRotatingBackward = false;

	public bool isZoomingIn = false, isZoomingOut = false;

	public void SetIsApplyingThrustForward(bool value){
		isApplyingThrustForward = value;
	}
	public void SetIsApplyingThrustBackward(bool value){
		isApplyingThrustBackward = value;
	}
	public void SetIsApplyingThrustRight(bool value){
		isApplyingThrustRight = value;
	}
	public void SetIsApplyingThrustLeft(bool value){
		isApplyingThrustLeft = value;
	}

	public void SetIsRotatingForward(bool value){
		isRotatingForward = value;
	}

	public void SetIsRotatingBackward(bool value){
		isRotatingBackward = value;
	}


	public void SetIsZoomingIn(bool value){
		isZoomingIn = value;
	}

	public void SetIsZoomingOut(bool value){
		isZoomingOut = value;
	}
	
	public Vector2Double thrustDirection = Vector2Double.up;
	public double thrustAcceleration;
	public double thrustDirectionSensibiliy;
	public Vector2Double currentInputAcceleration = Vector2Double.zero;
}
