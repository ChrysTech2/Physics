using System;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour{

	public Vector2Double position;
	public Vector2Double velocity;

	public double mass, radius;
	public Color color;

	private BodyController bodyController;
	private Settings settings;

	public Action ForceOnce;
	public Action<Body> ForceEachBody;

	public List<Body> bodiesAlreadyCollided = new List<Body>();

	int nCollisions = 0;

	public void Initialize(){

		bodyController = FindObjectOfType<BodyController>();
		settings = bodyController.settingsController.settings;

		GetComponent<Renderer>().material.color = color;

		transform.parent = bodyController.transform;
		transform.localScale = Vector2.one * (float)radius * 2 * 0.78f;
		
		ApplyPosition();
	}

	public static void CopyData(Body from, Body to){

		to.position = from.position;
		to.velocity = from.velocity;
		
		to.mass = from.mass;
		to.radius = from.radius;

		to.color = from.color;
		to.name = from.name;
	}

	// Movement
	private Vector2Double totalForces;

	public void UpdateVelocity(){

		totalForces = Vector2Double.zero;

		for (int i = 0; i < bodyController.bodies.Count; i++){

			Body body = bodyController.bodies[i];

			if (body != this)
				ForceEachBody(body);
		}
		
		ForceOnce();

		Vector2Double acceleration = totalForces / mass;
		velocity += acceleration * settings.secondsPerFrame;
	}

	public void UpdatePosition(){
		position += velocity * settings.secondsPerFrame;
	}

	public void ApplyPosition(){
		transform.localPosition = position.ToVector2();
	}

	// Forces
	public void DirectionalGravity(){
		totalForces += mass * settings.globalGravityVector;
	}

	public void Buoyancy(){
		totalForces -= Volume * settings.airDensity * settings.globalGravityVector;
	}

	public void AirDrag(){
		totalForces += velocity.direction.opposite * AirDragForce();
	}

	public void AttractionGravity(Body body){
		totalForces += Direction(body) * AttractionGravityForce(body);
	}

	public void Collision(Body body){

		double distance = DistanceFromSurface(body);
		
		if (distance > 0)
			return;

		if (bodiesAlreadyCollided.Contains(body))
			return;

		// Collision Happens
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

		// Collision Happens
		velocity = (mass * velocity + body.mass * body.velocity) / (mass + body.mass);

		double areaTot = Area + body.Area;
		float percentage1 = (float)(Area / areaTot);
		float percentage2 = (float)(body.Area / areaTot);

		color = color * percentage1 + body.color * percentage2;

		mass += body.mass;
		radius = Math.Cbrt(Math.Pow(radius, 3) + Math.Pow(body.radius, 3));

		GetComponent<Renderer>().material.color = color;
		transform.localScale = Vector2.one * 2 * (float)radius;

		nCollisions ++;
		body.nCollisions ++;

		position = position *  percentage1 + body.position * percentage2;
		bodyController.DeleteBody(body);
	}
	
	// Formulas
	private double AttractionGravityForce(Body body){
		return settings.gravityConstant * body.mass * mass / Math.Pow(Distance(body), 2);
	}
	private double AirDragForce(){
		return Math.Pow(velocity.magnitude,2) * settings.dragCoefficient * settings.airDensity * Area / 2;
	}
	
	// Other

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

	private double DistanceFromSurface(Body body){
		return Vector2Double.Distance(body.position, position) - body.radius - radius;
	}

	private double Distance(Body body){
		return Vector2Double.Distance(body.position, position);
	}

	private Vector2Double Direction(Body body){
		return (body.position - position).direction;
	}
}
