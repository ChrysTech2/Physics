using System;
using UnityEngine;
using System.Collections.Generic;

public class Body : MonoBehaviour{

	public Vector2Double position;
	public Vector2Double velocity;

	public double mass, radius;
	public Color color;
	public bool controllable;

	// Functions
	public Action ForceOnce, ForceAfterPosition;
	public Action<Body> ForceEachBody;

	// Other Stuff
	private BodyController bodyController;
	private Settings settings;
	private Transform bodyTransform;

	public int nCollisions = 0;
	public double area, density, volume;

	public static void CopyData(Body from, Body to){

		to.position = from.position;
		to.velocity = from.velocity;
		
		to.mass = from.mass;
		to.radius = from.radius;

		to.color = from.color;
		to.name = from.name;

		to.nCollisions = from.nCollisions;
	}

	public void Initialize(BodyController bodyController){

		this.bodyController = bodyController;
		settings = bodyController.settingsController.settings;
		
		transform.parent = bodyController.transform;
		bodyTransform = transform;
		
		Radius = radius;
		Color = color;
		
		ApplyPosition();
	}

	public static int n = 0;

	// Movement
	private Vector2Double acceleration = Vector2Double.zero;
	public Vector2Double accelerationBeforeReset;
	public void UpdateVelocity(){

		ForceOnce();

		for (int i = Index() + 1; i < bodyController.bodies.Count; i++)
			if (bodyController.bodies[i] != this)
				ForceEachBody(bodyController.bodies[i]);
		
		velocity += acceleration * settings.secondsPerFrame;

		accelerationBeforeReset = acceleration;

		acceleration = Vector2Double.zero;
	}

	private Vector2Double lastPosition = Vector2Double.zero;

	public void UpdatePosition(){
		position += velocity * settings.secondsPerFrame;
		ForceAfterPosition();
	}

	public void DrawLine(){
		bodyController.lineController.CreateLine(lastPosition, position, color, true, settings.lineDuration, "Line", settings.lineThickness);	
	}

	public void ApplyPosition(){;
		bodyTransform.localPosition = (position - bodyController.cameraController.position).ToVector2();
		lastPosition = position;
	}

	// Gravity Forces //
	// Directional
	public void DirectionalGravity(){
		acceleration += settings.gravity;
	}
	public void DirectionalGravityBuoyancy(){
		acceleration += settings.gravity * (1 - settings.fluidDensity/density);
	}

	// Centered
	public void CenteredGravity(){
		acceleration += position.direction.opposite * settings.gravityAcceleration;
	}
	public void CenteredGravityBuoyancy(){
		acceleration += position.direction.opposite * settings.gravityAcceleration * (1 - settings.fluidDensity/density);
	}

	// Velocity
	public void VelocityGravity(){
		acceleration += velocity.direction.SumVectorAsAngle(settings.gravityDirection) * settings.gravityAcceleration;
	}
	public void VelocityGravityBuoyancy(){
		acceleration += velocity.direction.SumVectorAsAngle(settings.gravityDirection) * settings.gravityAcceleration * (1 - settings.fluidDensity/density);
	}

	// Camera
	public void CameraGravity(){
		acceleration += bodyController.cameraController.rotationDirection.SubtractVectorAsAngle(Vector2Double.up) * settings.gravityAcceleration;
	}
	public void CameraGravityBuoyancy(){
		acceleration += bodyController.cameraController.rotationDirection.SubtractVectorAsAngle(Vector2Double.up) * settings.gravityAcceleration * (1 - settings.fluidDensity/density);
	}

	// Last Click
	public void LastClickGravity(){
		acceleration += (settings.lastMousePosition - position).direction * settings.gravityAcceleration;
	}
	public void LastClickGravityBuoyancy(){
		acceleration += (settings.lastMousePosition - position).direction * settings.gravityAcceleration * (1 - settings.fluidDensity/density);
	}

	// Last Click Direction
	public void LastClickDirectionGravity(){
		acceleration += settings.lastMousePosition.direction * settings.gravityAcceleration;
	}
	public void LastClickDirectionGravityBuoyancy(){
		acceleration += settings.lastMousePosition.direction * settings.gravityAcceleration * (1 - settings.fluidDensity/density);
	}

	// Attraction
	public void AttractionGravity(Body body){

		Vector2Double force = Direction(body) * settings.AttractionGravity(this, body);
		acceleration += force / mass;
		body.acceleration -= force / body.mass;
	}
	public void AttractionGravityBuoyancy(Body body){
		
		Vector2Double force = Direction(body) * settings.AttractionGravity(this, body);
		acceleration += force / mass * (1 - settings.fluidDensity / density);
		body.acceleration -= force / body.mass * (1 - settings.fluidDensity / body.density);
	}

	// Other Forces
	public void Drag(){
		acceleration += velocity.direction.opposite * settings.Drag(this);
	}

	public void Lift(){
		
		Vector2Double direction = velocity.direction;

		if (Math.Abs(direction.ToDegrees()) <= 90)
			acceleration += velocity.direction.SumVectorAsAngle(Vector2Double.up) * settings.Lift(this);
		else
			acceleration += velocity.direction.SubtractVectorAsAngle(Vector2Double.up) * settings.Lift(this);
	}

	public void Thrust(){
		acceleration += settings.currentInputAcceleration;
	}

	// Collisions

	public List<Body> collidedBodies = new List<Body>();
	public void Collision(Body body){

		double distance = DistanceFromSurface(body);
		
		if (distance >= 0)
			return;

		Vector2Double direction = Direction(body);
		
		Vector2Double velocity1 = velocity.magnitude * velocity.direction.SubtractVectorAsAngle(direction);
		Vector2Double velocity2 = body.velocity.magnitude * body.velocity.direction.SubtractVectorAsAngle(direction);

		double totalMass = mass + body.mass;
		double percentage1 = body.mass / totalMass;
		double percentage2 = mass / totalMass;

		// Calculate
		Vector2Double finalVelocity1, finalVelocity2;

		finalVelocity1.x = (mass * velocity1.x + body.mass * (velocity2.x * (1 + settings.coefOfRestitution) - settings.coefOfRestitution * velocity1.x)) / totalMass;
		finalVelocity2.x = (body.mass * velocity2.x + mass * (velocity1.x * (1 + settings.coefOfRestitution) - settings.coefOfRestitution * velocity2.x)) / totalMass;
		
		finalVelocity1.y = velocity1.y;
		finalVelocity2.y = velocity2.y;

		// Apply
		velocity = finalVelocity1.magnitude * finalVelocity1.direction.SumVectorAsAngle(direction);
		body.velocity = finalVelocity2.magnitude * finalVelocity2.direction.SumVectorAsAngle(direction);

		position -= Math.Abs(distance * percentage1) * direction;
		body.position += Math.Abs(distance * percentage2) * direction;

		// End
		nCollisions ++;
		body.nCollisions ++;

		collidedBodies.Add(body);
		body.collidedBodies.Add(this);
	}

	public void CollisionMerge(Body body){

		double distance = DistanceFromSurface(body);
		
		if (distance >= 0)
			return;

		double areaTot = area + body.area;
		float percentage1 = (float)(area / areaTot);
		float percentage2 = (float)(body.area / areaTot);

		velocity = (mass * velocity + body.mass * body.velocity) / (mass + body.mass);

		Radius = Math.Cbrt(Math.Pow(radius, 3) + Math.Pow(body.radius, 3));
		Color = color * percentage1 + body.color * percentage2;

		Mass = mass + body.mass;

		nCollisions ++;

		position = position *  percentage1 + body.position * percentage2;
		bodyController.DeleteBody(body);
	}

	public void Friction(){

		foreach (Body body in collidedBodies){

			Vector2Double direction = Direction(body);
			Vector2Double velocityOnDirection = velocity.magnitude * velocity.direction.SubtractVectorAsAngle(direction);
			
			double acceleration1 = (accelerationBeforeReset.magnitude * accelerationBeforeReset.direction.SubtractVectorAsAngle(direction)).x;
			double acceleration2 = (body.accelerationBeforeReset.magnitude * body.accelerationBeforeReset.direction.SubtractVectorAsAngle(direction)).x;
			double relativeAcceleration = Math.Abs(-acceleration1 + acceleration2);

			double finalAcceleration = -relativeAcceleration * settings.frictionCoefficient * Math.Sign(velocityOnDirection.y);
			acceleration += finalAcceleration * direction.SumVectorAsAngle(Vector2Double.up);
		}

		collidedBodies.Clear();
	}

	public void CheckRectangleCollision(){

		// add impulse force in normal force -> Math.Abs(accelerationBeforeReset.y) + Math.Abs(velocity.x - oldVelocity)

		double limitDown = -settings.border.y + radius;
		double limitUp = settings.border.y - radius;
		double limitLeft = -settings.border.x + radius;
		double limitRight = settings.border.x - radius;

		if (position.y < limitDown){

			position.y = limitDown;
			velocity.y *= -settings.borderCoefOfRestitution;
			nCollisions ++;

			if (settings.frictionCoefficient != 0)
				acceleration.x = Math.Abs(accelerationBeforeReset.y) * -Math.Sign(velocity.x) * settings.frictionCoefficient;
		}

		else if (position.y > limitUp){

			position.y = limitUp;
			velocity.y *= -settings.borderCoefOfRestitution;
			nCollisions ++;

			if (settings.frictionCoefficient != 0)
				acceleration.x = Math.Abs(accelerationBeforeReset.y) * -Math.Sign(velocity.x) * settings.frictionCoefficient;
		}

		if (position.x < limitLeft){

			position.x = limitLeft;
			velocity.x *= -settings.borderCoefOfRestitution;
			nCollisions ++;

			if (settings.frictionCoefficient != 0)
				acceleration.y = Math.Abs(accelerationBeforeReset.x) * -Math.Sign(velocity.y) * settings.frictionCoefficient;
		}

		else if (position.x > limitRight){
			
			position.x = limitRight;
			velocity.x *= -settings.borderCoefOfRestitution;
			nCollisions ++;

			if (settings.frictionCoefficient != 0)
				acceleration.y = Math.Abs(accelerationBeforeReset.x) * -Math.Sign(velocity.y) * settings.frictionCoefficient;
		}
	}

	public void CheckCircleCollision(){

		if (settings.border.x - position.magnitude - radius > 0) // border.x is the radius here
			return;
		
		Vector2Double direction = -1 * position.direction;
		Vector2Double velocityOnDirection = velocity.magnitude * velocity.direction.SubtractVectorAsAngle(direction);
		Vector2Double finalVelocityOnDirection;

		finalVelocityOnDirection.x = -1 * velocityOnDirection.x * settings.borderCoefOfRestitution;
		finalVelocityOnDirection.y = velocityOnDirection.y;

		velocity = finalVelocityOnDirection.magnitude * finalVelocityOnDirection.direction.SumVectorAsAngle(direction);
		position = position.direction * (settings.border.x - radius);
		nCollisions ++;

		if (settings.frictionCoefficient != 0){
			
			double accelerationOnDirection = (accelerationBeforeReset.magnitude * accelerationBeforeReset.direction.SubtractVectorAsAngle(direction)).x;
			double finalAcceleration = -Math.Abs(accelerationOnDirection) * settings.frictionCoefficient * Math.Sign(velocityOnDirection.y);

			acceleration = finalAcceleration * direction.SumVectorAsAngle(Vector2Double.up);
		}
	}
	
	// Other Stuff
	public double Radius{
		set{
			radius = value;
			bodyTransform.localScale = Vector2.one * (float)radius;
			area = Math.PI * Math.Pow(radius, 2);
			volume = 4 * Math.PI / 3 * Math.Pow(radius, 3);
			density = mass / volume;
		}
	}

	public double Mass{
		set{
			mass = value;
			density = mass / volume;
		}
	}

	public Color Color{
		set{
			color = value;
			GetComponent<Renderer>().material.color = color;
		}
	}

	public double Distance(Body body){
		return Vector2Double.Distance(body.position, position);
	}

	private double DistanceFromSurface(Body body){
		return Vector2Double.Distance(body.position, position) - body.radius - radius;
	}

	private Vector2Double Direction(Body body){
		return (body.position - position).direction;
	}

	public int Index(){
		return bodyController.bodies.IndexOf(this);
	}
}
