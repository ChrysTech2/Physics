using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BodyController : MonoBehaviour{

	[SerializeField] private TMP_Text informations;

	public CameraController cameraController;
	public SettingsController settingsController;
	private Settings settings;
	public List<Body> bodies = new List<Body>();
	public int speedMultiplier = 0;
	public float t = 0, fps = 60;

	private void Start(){
		settings = settingsController.settings;
		Application.targetFrameRate = 240;
	}

	

	private void Update(){

		if (settingsController.gameObject.activeSelf){
			informations.SetText("");
			return;
		}

		CheckInputs();

		DebugInformation();
	}

	private void CheckInputs(){

		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

		if (mouseWheel > 0)
			ZoomIn(1.1f);
		else if (mouseWheel < 0)
			ZoomOut(1.1f);

		if (Input.GetKeyDown(KeyCode.Comma))
			SlowDown();

		if (Input.GetKeyDown(KeyCode.Period))
			SpeedUp();

		if (Input.GetKey(KeyCode.Minus))
			ZoomOut(1.025f);

		if (Input.GetKey(KeyCode.Equals))
			ZoomIn(1.025f);
		
	}

	private void DebugInformation(){

		float time = 0; string timeUnit;
		fps = 1/Time.deltaTime;

		if (t < Times.SECONDS_PER_MINUTE){
			time = t;
			timeUnit = "seconds";
		}
		else if (t < Times.SECONDS_PER_HOUR){
			time = t / Times.SECONDS_PER_MINUTE;
			timeUnit = "minutes";
		}
		else if (t < Times.SECONDS_PER_DAY){
			time = t / Times.SECONDS_PER_HOUR;
			timeUnit = "hours";
		}
		else if (t < Times.SECONDS_PER_WEEK){
			time = t / Times.SECONDS_PER_DAY;
			timeUnit = "days";
		}
		else if (t < Times.SECONDS_PER_MONTH){
			time = t / Times.SECONDS_PER_WEEK;
			timeUnit = "weeks";
		}
		else if (t < Times.SECONDS_PER_YEAR){
			time = t / Times.SECONDS_PER_MONTH;
			timeUnit = "months";
		}
		else{
			time = t / Times.SECONDS_PER_YEAR;
			timeUnit = "years";
		}

		informations.SetText(
			$"Speed : {(float)(speedMultiplier * settings.secondsPerFrame * 50)}x  |  Scale : {scale}\nTime ({timeUnit}): {time}\nFPS : {fps}\n{cameraController.Index}"
		);
	}

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

		t += (float)settingsController.settings.secondsPerFrame;
	}

	public void SpeedUp(){

		if (speedMultiplier == 0)
			speedMultiplier = 1;
		else
			if (fps > 20)
				speedMultiplier *= 2;
	}

	public void SlowDown(){
		
		if (speedMultiplier == 1)
			speedMultiplier = 0;
		else
			speedMultiplier /= 2;
	}

	public void ZoomOut(float zoomFactor = 1.25f){
		transform.localScale /= zoomFactor;
	}

	public void ZoomIn(float zoomFactor = 1.25f){
		transform.localScale *= zoomFactor;
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

		createdBody.Initialize(this);
		CompileFunctions(createdBody);
		
		bodies.Add(createdBody);

		List<string> nameToList = new List<string>() {createdBody.name};
		settingsController.parent.AddOptions(nameToList);

		cameraController.Index = bodies.Count - 1;
	}

	public void DeleteBody(Body body){

		settingsController.parent.options.RemoveAt(body.Index());
		
		bodies.Remove(body);
		DestroyImmediate(body.gameObject);
	}

	public void CompileFunctions(Body body){

		body.ForceOnce = new Action(() => {});
		body.ForceEachBody = new Action<Body>((body2) => {});
		
		if (settings.fluidDensity != 0 && settings.dragCoefficient != 0)
			body.ForceOnce += () => body.AirDrag();
		
		if (settings.gravityAcceleration != 0){

			switch(settings.gravityMode){

				case GravityMode.DIRECTIONAl:
					//body.ForceOnce += () => body.DirectionalGravity();

					if (settings.fluidDensity != 0)
						body.ForceOnce += () => body.DirectionalGravityBuoyancy();
					else
						body.ForceOnce += () => body.DirectionalGravity();

					break;

				case GravityMode.CENTERED:
					//body.ForceOnce += () => body.CenteredGravity();

					if (settings.fluidDensity != 0)
						body.ForceOnce += () => body.CenteredGravityBuoyancy();
					else
						body.ForceOnce += () => body.CenteredGravity();

					break;
			}	
		}

		if (settings.attractionGravityConstant != 0)
			body.ForceEachBody += (body2) => body.AttractionGravity(body2);

		if (settings.calculateCollisions){

			if (settings.mergeBodiesInCollisions)
				body.ForceEachBody += (body2) => body.CollisionMerge(body2);
			else{
				body.ForceEachBody += (body2) => body.Collision(body2);
				body.ForceOnce += () => body.bodiesAlreadyCollided.Clear();
			}
		}
	}

}
