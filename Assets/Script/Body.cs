using UnityEngine;

public class Body : MonoBehaviour{

	public Vector2Double position;
	public Vector2Double velocity;

	public double mass, radius;
	public Color color;

	private BodyController bodyController;
	private Settings settings;

	public void Initialize(){

		// to do in the other file -> body.name = that_name

		bodyController = FindObjectOfType<BodyController>();
		settings = FindObjectOfType<Settings>();

		GetComponent<Renderer>().material.color = color;

		transform.parent = bodyController.transform;
		transform.localScale = Vector2.one * (float)radius * 2;
		
		ApplyPosition();
	}

	private Vector2Double totalForces;

	public void UpdateVelocity(){

		totalForces = Vector2Double.zero;

		// action with events to update totalForces

		Vector2Double acceleration = totalForces / mass;
		velocity += acceleration * bodyController.settings.secondsPerFrame;
	}

	public void UpdatePosition(){
		position += velocity * bodyController.settings.secondsPerFrame;
	}

	public void ApplyPosition(){
		transform.localPosition = position.ToVector2();
	} 
	

	
}
