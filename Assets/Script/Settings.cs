using System;
using UnityEngine;

public class Settings : MonoBehaviour{

	// Global Settings
	public double secondsPerFrame, lineDuration, lineThickness;
	public int gravityMode, borderMode;
	public double gravityAcceleration, gravityAngle;
	public double attractionGravityConstant, impulseForce;
	public double fluidDensity, dragCoefficient, liftCoefficient;
	public double coefOfRestitution, borderCoefOfRestitution, frictionCoefficient;
	public bool calculateCollisions, mergeBodiesInCollisions;
	public Vector2Double border;

	public Func<double, double> distanceFunction; // distance -> force
	public double aValue;

	public double touchMultiplier;
	public bool showCenterOfGravity;
	public bool calculateBuoyancy;
	
	public Vector2Double gravity, gravityDirection;

	public double AttractionGravity(Body body1, Body body2){
		return attractionGravityConstant * body1.mass * body2.mass * distanceFunction(body1.Distance(body2));
	}

	public double Drag(Body body){
		return Math.Pow(body.velocity.magnitude, 2) * dragCoefficient * fluidDensity * body.area / (2 * body.mass);
	}

	public double Lift(Body body){
		return Math.Pow(body.velocity.magnitude, 2) * liftCoefficient * fluidDensity * body.area / (2 * body.mass);
	}

	public double actualImpulseForce = 0;
	public double ImpulseForce(Body body){
		return actualImpulseForce / (body.mass * Math.Pow(Vector2Double.Distance(body.position, lastMousePosition),2));
	}

	public bool isApplyingThrustForward = false;
	public bool isApplyingThrustBackward = false;
	public bool isApplyingThrustRight = false;
	public bool isApplyingThrustLeft = false;

	public bool isRotatingForward = false;
	public bool isRotatingBackward = false;

	public bool isCameraRotatingLeft = false, isCameraRotatingRigtht = false;

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

	public void SetIsCameraRotatingRight(bool value){
		isCameraRotatingRigtht = value;
	}
	public void SetIsCameraRotatingLeft(bool value){
		isCameraRotatingLeft = value;
	}
	
	public Vector2Double thrustDirection = Vector2Double.up;
	public double thrustAcceleration;
	public double thrustDirectionSensibiliy;
	public Vector2Double currentInputAcceleration = Vector2Double.zero;
	public Vector2Double lastMousePosition = Vector2Double.zero;
}
