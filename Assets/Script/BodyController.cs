using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour{

	public SettingsController settingsController;
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

		for(int i = 0; i < bodies.Count; i++)
			bodies[i].UpdateVelocity();

		foreach (Body body in bodies)
			body.UpdatePosition();

		t += settingsController.settings.secondsPerFrame;
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
		set{
			transform.localScale = Vector3.one * value;
		}
	}

	public void AddBody(){

		Body createdBody = Instantiate(settingsController.bodyToCreate);
		Body.CopyData(settingsController.bodyToCreate, createdBody);

		createdBody.Initialize();
		bodies.Add(createdBody);
	}

	public void DeleteBody(Body body){
		
		bodies.Remove(body);
		DestroyImmediate(body.gameObject);
	}

}
