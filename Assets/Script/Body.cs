using System;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour{

	public Vector2Double position;
	public Vector2Double velocity;

	public double mass, radius;
	public Color color;

	// Functions
	public Action ForceOnce, ForceAfterPosition;
	public Action<Body> ForceEachBody;
	public List<Body> bodiesAlreadyCollided = new List<Body>();
	public List<Body> bodiesAlreadyGravited = new List<Body>();

	// Other Stuff
	private BodyController bodyController;
	private Settings settings;

	public int nCollisions = 0;

	public static void CopyData(Body from, Body to){

		to.position = from.position;
		to.velocity = from.velocity;
		
		to.mass = from.mass;
		to.radius = from.radius;

		to.color = from.color;
		to.name = from.name;
	}

	public void Initialize(BodyController bodyController){

		this.bodyController = bodyController;
		settings = bodyController.settingsController.settings;
		
		transform.parent = bodyController.transform;
		
		SetRadius(radius);
		SetColor(color);
		
		ApplyPosition();
	}

	public static int n = 0;

	// Movement
	private Vector2Double acceleration = Vector2Double.zero;
	public void UpdateVelocity(){

		

		for (int i = 0; i < bodyController.bodies.Count; i++){

			Body body = bodyController.bodies[i];

			if (body != this)
				ForceEachBody(body);
		}
		
		ForceOnce();

		velocity += acceleration * settings.secondsPerFrame;

		acceleration = Vector2Double.zero;
	}

	public void UpdatePosition(){
		position += velocity * settings.secondsPerFrame;
		ForceAfterPosition();
	}

	public void ApplyPosition(){
		transform.localPosition = (position - bodyController.cameraController.position).ToVector2();
	}

	// Gravity Forces
	public void DirectionalGravity(){
		acceleration += settings.gravity;
	}

	public void DirectionalGravityBuoyancy(){
		acceleration += settings.gravity * (1 - settings.fluidDensity/Density);
	}

	public void CenteredGravity(){
		acceleration = position.direction.opposite * settings.gravityAcceleration;
	}
	
	public void CenteredGravityBuoyancy(){
		acceleration += position.direction.opposite * settings.gravityAcceleration * (1 - settings.fluidDensity/Density);
	}

	public void VelocityGravity(){
		acceleration += velocity.direction.SumVectorAsAngle(settings.gravityDirection) * settings.gravityAcceleration;
	}

	public void VelocityGravityBuoyancy(){
		acceleration += velocity.direction.SumVectorAsAngle(settings.gravityDirection) * settings.gravityAcceleration * (1 - settings.fluidDensity/Density);
	}

	// Other Forces
	public void AirDrag(){
		acceleration += velocity.direction.opposite * settings.AirDrag(this);
	}

	public void AttractionGravity(Body body){
		if (bodiesAlreadyGravited.Contains(body))
			return;

		Vector2Double force = Direction(body) * settings.AttractionGravity(this, body);
		acceleration += force;
		body.acceleration -= force;
		body.bodiesAlreadyGravited.Add(this);
	}

	// Collisions
	public void Collision(Body body){

		double distance = DistanceFromSurface(body);
		
		if (distance > 0)
			return;

		if (bodiesAlreadyCollided.Contains(body))
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

		body.bodiesAlreadyCollided.Add(this);
	}

	public void CollisionMerge(Body body){

		double distance = DistanceFromSurface(body);
		
		if (distance > 0)
			return;

		velocity = (mass * velocity + body.mass * body.velocity) / (mass + body.mass);
		radius = Math.Cbrt(Math.Pow(radius, 3) + Math.Pow(body.radius, 3));
		mass += body.mass;

		double areaTot = Area + body.Area;
		float percentage1 = (float)(Area / areaTot);
		float percentage2 = (float)(body.Area / areaTot);

		SetColor(color * percentage1 + body.color * percentage2);
		SetRadius(radius);

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
 
	public double Area{
		get{
			return Math.PI * Math.Pow(radius, 2);
		}
	}

	public double Volume{
		get{
			return 4 * Math.PI / 3 * Math.Pow(radius, 3);
		}
	}

	public double Density{
		get{
			return mass / Volume;
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

	public void SetRadius(double radius){
		this.radius = radius;
		transform.localScale = Vector2.one * (float)radius;
	}

	public void SetColor(Color color){
		this.color = color;
		GetComponent<Renderer>().material.color = color;
	}

	public int Index(){
		return bodyController.bodies.IndexOf(this);
	}
}
