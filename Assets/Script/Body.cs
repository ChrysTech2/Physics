using System;
using UnityEngine;

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
	public static BodyController bodyController;
	public static Settings settings;
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

	public void Initialize(){

		settings = bodyController.settingsController.settings;
		
		transform.parent = bodyController.transform;
		bodyTransform = transform;
		
		Radius = radius;
		Color = color;
		
		ApplyPosition();
	}

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
		acceleration += velocity.direction.SumVectorAsAngle(Vector2Double.up) * settings.Lift(this);
	}

	public void Thrust(){
		acceleration += settings.currentInputAcceleration;
	}

	public void ImpulseForce(){
		acceleration += (position - settings.lastMousePosition).direction * settings.ImpulseForce(this);
	}

	// Collisions
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

		if (settings.frictionCoefficient != 0){

			double normalAcceleration1 = (finalVelocity1.x - velocity1.x) / settings.secondsPerFrame;
			double normalAcceleration2 = (finalVelocity2.x - velocity2.x) / settings.secondsPerFrame;

			double relativeVelocity = velocity1.y - velocity2.y; // velocity of second body

			double friction1 = Math.Abs(normalAcceleration1) * Math.Sign(relativeVelocity) * settings.frictionCoefficient;
			double friction2 = Math.Abs(normalAcceleration2) * -Math.Sign(relativeVelocity) * settings.frictionCoefficient;
			
			if (Math.Sign(relativeVelocity) != Math.Sign(relativeVelocity + friction2 * settings.secondsPerFrame)){
				//velocity2.y = velocity1.y;
				//body.velocity = velocity2.magnitude * velocity2.direction.SumVectorAsAngle(direction);
			}
			else{
				body.acceleration += friction2 * direction.SubtractVectorAsAngle(Vector2Double.up);
			}

			if (Math.Sign(-relativeVelocity) != Math.Sign(-relativeVelocity + friction1 * settings.secondsPerFrame)){
				//velocity1.y = velocity2.y;
				//velocity = velocity1.magnitude * velocity1.direction.SubtractVectorAsAngle(direction);
			}
			else{
				acceleration += friction1 * direction.SubtractVectorAsAngle(Vector2Double.up);
			}
		}
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

	public void CheckRectangleCollision(){

		double limitDown = -settings.border.y + radius;
		double limitUp = settings.border.y - radius;
		double limitLeft = -settings.border.x + radius;
		double limitRight = settings.border.x - radius;

		if (position.y < limitDown){

			position.y = limitDown;
			double oldVelocity = velocity.y;
			velocity.y *= -settings.borderCoefOfRestitution;
			nCollisions ++;

			if (settings.frictionCoefficient != 0){

				double normalAcceleration = (velocity.y - oldVelocity) / settings.secondsPerFrame;
				double friction = Math.Abs(normalAcceleration) * -Math.Sign(velocity.x) * settings.frictionCoefficient;
				
				if (Math.Sign(velocity.x) != Math.Sign(velocity.x + friction * settings.secondsPerFrame))
					velocity.x = 0;
				else
					acceleration.x += friction;
			}
		}

		else if (position.y > limitUp){

			position.y = limitUp;
			double oldVelocity = velocity.y;
			velocity.y *= -settings.borderCoefOfRestitution;
			nCollisions ++;

			if (settings.frictionCoefficient != 0){

				double normalAcceleration = (velocity.y - oldVelocity) / settings.secondsPerFrame;
				double friction = Math.Abs(normalAcceleration) * -Math.Sign(velocity.x) * settings.frictionCoefficient;

				if (Math.Sign(velocity.x) != Math.Sign(velocity.x + friction * settings.secondsPerFrame))
					velocity.x = 0;
				else
					acceleration.x += friction;
			}
		}

		if (position.x < limitLeft){

			position.x = limitLeft;
			double oldVelocity = velocity.x;
			velocity.x *= -settings.borderCoefOfRestitution;
			nCollisions ++;

			if (settings.frictionCoefficient != 0){
				
				double normalAcceleration = (velocity.x - oldVelocity) / settings.secondsPerFrame;
				double friction = Math.Abs(normalAcceleration) * -Math.Sign(velocity.y) * settings.frictionCoefficient;

				if (Math.Sign(velocity.y) != Math.Sign(velocity.y + friction * settings.secondsPerFrame))
					velocity.y = 0;
				else
					acceleration.y += friction;
			}
		}

		else if (position.x > limitRight){
			
			position.x = limitRight;
			double oldVelocity = velocity.x;
			velocity.x *= -settings.borderCoefOfRestitution;
			nCollisions ++;

			if (settings.frictionCoefficient != 0){

				double normalAcceleration = (velocity.x - oldVelocity) / settings.secondsPerFrame;
				double friction = Math.Abs(normalAcceleration) * -Math.Sign(velocity.y) * settings.frictionCoefficient;

				if (Math.Sign(velocity.y) != Math.Sign(velocity.y + friction * settings.secondsPerFrame))
					velocity.y = 0;
				else
					acceleration.y += friction;
			}
		}
	}

	public void CheckCircleCollision(){

		if (settings.border.x - position.magnitude - radius > 0) // border.x is the radius here
			return;

		Vector2Double direction = -1 * position.direction;
		Vector2Double velocityOnDirection = velocity.magnitude * velocity.direction.SubtractVectorAsAngle(direction);

		position = -direction * (settings.border.x - radius);
		double oldVelocity = velocityOnDirection.x;
		velocityOnDirection.x *= -settings.borderCoefOfRestitution;
		nCollisions ++;

		if (settings.frictionCoefficient != 0){

			double normalAcceleration = (velocityOnDirection.x - oldVelocity) / settings.secondsPerFrame;
			double friction = Math.Abs(normalAcceleration) * -Math.Sign(velocityOnDirection.y) * settings.frictionCoefficient;

			if (Math.Sign(velocityOnDirection.y) != Math.Sign(velocityOnDirection.y + friction * settings.secondsPerFrame)){
				velocityOnDirection.y = 0;
			}
			else{
				acceleration += friction * direction.SumVectorAsAngle(Vector2Double.up);
			}
		}

		velocity = velocityOnDirection.magnitude * velocityOnDirection.direction.SumVectorAsAngle(direction);	
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
