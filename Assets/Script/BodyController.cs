using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour{

	[SerializeField] public Settings settings;

	public List<Body> bodies = new List<Body>();
	public int speedMultiplier = 0;
	public double t = 0;

	private void FixedUpdate(){

		for (int i = 0; i < speedMultiplier; i++)
			CalculateOneFrame();
		
		foreach (Body body in bodies)
			body.ApplyPosition();
	}

	private void CalculateOneFrame(){

		foreach (Body body in bodies)
			body.UpdateVelocity();

		foreach (Body body in bodies)
			body.UpdatePosition();

		t += settings.secondsPerFrame;
	}

	public void SpeedUp(){

		if (speedMultiplier == 0)
			speedMultiplier = 1;
		else
			speedMultiplier *= 2;
	}

	public void SlowDown(){
		
		if (speedMultiplier == 1)
			speedMultiplier = 0;
		else
			speedMultiplier /= 2;
	}

	public void ZoomOut(){
		transform.localScale /= 1.25f;
	}

	public void ZoomIn(){
		transform.localScale *= 1.25f;
	}

	public float scale{
		get{
			return transform.localScale.x;
		}
	}

	public void AddBody(Body bodyToCreate){

		Body body = Instantiate(bodyToCreate);

		

		body.Initialize();
		bodies.Add(body);

		/*body.staticFrictionCoef = bodyToCopy.staticFrictionCoef;
		body.dynamicFrictionCoef = bodyToCopy.dynamicFrictionCoef;*/

		// uncomment when u add those

		

	}

	public void DeleteBody(Body body){
		bodies.Remove(body);
		DestroyImmediate(body);
	}

}
