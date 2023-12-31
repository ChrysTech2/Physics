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

		for (int i = Index() + 1; i < bodyController.bodies.Count; i++)
			ForceEachBody(bodyController.bodies[i]);
		
		ForceOnce();

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

	public void ApplyPosition(){
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
		acceleration += velocity.direction.opposite * settings.AirDrag(this);
	}

	public void Thrust(){
		acceleration += settings.currentInputAcceleration;
	}

	// Collisions
	public void Collision(Body body){

		double distance = DistanceFromSurface(body);
		
		if (distance > 0)
			return;

		Vector2Double direction = Direction(body);
		
		Vector2Double velocity1 = velocity.magnitude * velocity.direction.SubtractVectorAsAngle(direction);
		Vector2Double velocity2 = body.velocity.magnitude * body.velocity.direction.SubtractVectorAsAngle(direction);

		double totalMass = mass + body.mass;
		double percentage1 = body.mass / totalMass;
		double percentage2 = mass / totalMass;

		// Calculate
		Vector2Double velocityTemp1, velocityTemp2;

		velocityTemp1.x = (mass * velocity1.x + body.mass * (velocity2.x * (1 + settings.coefOfRestitution) - settings.coefOfRestitution * velocity1.x)) / totalMass;
		velocityTemp2.x = (body.mass * velocity2.x + mass * (velocity1.x * (1 + settings.coefOfRestitution) - settings.coefOfRestitution * velocity2.x)) / totalMass;
		
		velocityTemp1.y = velocity1.y;
		velocityTemp2.y = velocity2.y;

		// Apply
		velocity = velocityTemp1.magnitude * velocityTemp1.direction.SumVectorAsAngle(direction);
		body.velocity = velocityTemp2.magnitude * velocityTemp2.direction.SumVectorAsAngle(direction);

		position -= Math.Abs(distance * percentage1) * direction;
		body.position += Math.Abs(distance * percentage2) * direction;

		// End
		nCollisions ++;
		body.nCollisions ++;
	}

	public void CollisionMerge(Body body){

		double distance = DistanceFromSurface(body);
		
		if (distance > 0)
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

		if (position.y - radius < -settings.border.y){

			position.y = -settings.border.y + radius;
			velocity.y = -velocity.y * settings.coefOfRestitution;
			nCollisions ++;
		}

		else if (position.y + radius > settings.border.y){

			position.y = settings.border.y - radius;
			velocity.y = -velocity.y * settings.coefOfRestitution;
			nCollisions ++;
		}

		if (position.x - radius < -settings.border.x){

			position.x = -settings.border.x + radius;
			velocity.x = -velocity.x * settings.coefOfRestitution;
			nCollisions ++;
		}

		else if (position.x + radius > settings.border.x){
			
			position.x = settings.border.x - radius;
			velocity.x = -velocity.x * settings.coefOfRestitution;
			nCollisions ++;
		}
	}

	public void CheckCircleCollision(){

		if (settings.border.x - position.magnitude - radius > 0) // border.x is the radius here
			return;
		
		Vector2Double direction = -1 * position.direction;

		Vector2Double velocity1 = velocity.magnitude * velocity.direction.SubtractVectorAsAngle(direction);
		Vector2Double velocityTemp1;

		velocityTemp1.x = -1 * velocity1.x * settings.coefOfRestitution;
		velocityTemp1.y = velocity1.y;

		velocity = velocityTemp1.magnitude * velocityTemp1.direction.SumVectorAsAngle(direction);

		position = position.direction * (settings.border.x - radius);

		nCollisions ++;
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
