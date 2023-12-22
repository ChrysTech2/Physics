using System;
using UnityEngine;

public class Settings : MonoBehaviour{

	// Global Settings
	public double secondsPerFrame;
	public int gravityMode;
	public double gravityAcceleration, gravityAngle;
	public double attractionGravityConstant;
	public double fluidDensity, dragCoefficient;
	public double coefOfRestitution;
	public bool calculateCollisions, mergeBodiesInCollisions;

	public Vector2Double gravityDirection;

	public double AttractionGravityForce(Body body1, Body body2){
		return attractionGravityConstant * body1.mass * body2.mass / Math.Pow(body1.Distance(body2), 2);
	}

	public double AirDragForce(Body body){
		return Math.Pow(body.velocity.magnitude, 2) * dragCoefficient * fluidDensity * body.Area / 2;
	}

	public double BuoyancyForce(Body body){
		return fluidDensity * body.Volume * gravityAcceleration;
	}

	public double GravityForce(Body body){
		return body.mass * gravityAcceleration;
	}
}
