using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BodyController : MonoBehaviour{

	public GameObject startupMenu;

	public TMP_Text informations;
	public TouchControl touchControl;
	public GameObject centerOfGravity;
	public GameObject thrustControls;

	public CameraController cameraController;
	public SettingsController settingsController;
	public WorldLineController lineController;
	public Settings settings;
	public BodyEditor bodyEditor;
	public GraphController graphController;

	public List<Body> bodies = new List<Body>();
	public int speedMultiplier = 0;
	public float t = 0, fps = 60;

	public bool isZoomingOut = false, isZoomingIn = false;
	private void Start(){
		settings = settingsController.settings;
		Application.targetFrameRate = 240;
	}

	public void SetIsZoomingIn(bool value){
		isZoomingIn = value;
	}

	public void SetIsZoomingOut(bool value){
		isZoomingOut = value;
	}

	private void Update(){

		if (isZoomingIn)
			ZoomIn(1.025f);
			
		else if (isZoomingOut)
			ZoomOut(1.025f);

		foreach (Body body in bodies)
			body.ApplyPosition();

		if (settings.showCenterOfGravity)
			ShowCenterOfGravity();

		if (settingsController.gameObject.activeSelf)
			return;

		CheckInput();
		DebugInformation();
	}

	private void ShowCenterOfGravity(){

		Vector2Double position = Vector2Double.zero;
		double totalMass = 0;

		foreach (Body body in bodies){

			position += body.position * body.mass;
			totalMass += body.mass;
		}

		if (totalMass != 0)
			position /= totalMass;

		centerOfGravity.transform.localPosition = (position - cameraController.position).ToVector2() * scale;
	}

	private void CheckInput(){

		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

		if (mouseWheel > 0)
			ZoomIn(1.1f);
		else if (mouseWheel < 0)
			ZoomOut(1.1f);

		if (Input.GetKeyDown(KeyCode.Comma))
			SlowDown();

		if (Input.GetKeyDown(KeyCode.Period))
			SpeedUp();

		if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
			ZoomOut(1.025f);

		if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
			ZoomIn(1.025f);

		if (Input.GetKeyDown(KeyCode.LeftBracket))
			cameraController.PreviousBody();

		if (Input.GetKeyDown(KeyCode.RightBracket))
			cameraController.NextBody();

		CheckThrustInput();
	}

	private void CheckThrustInput(){

		if (Input.GetKeyDown(KeyCode.W))
			settings.SetIsApplyingThrust(true);

		if (Input.GetKeyDown(KeyCode.D))
			settings.SetIsRotatingBackward(true);

		if (Input.GetKeyDown(KeyCode.A))
			settings.SetIsRotatingForward(true);

		
		if (Input.GetKeyUp(KeyCode.W))
			settings.SetIsApplyingThrust(false);

		if (Input.GetKeyUp(KeyCode.D))
			settings.SetIsRotatingBackward(false);

		if (Input.GetKeyUp(KeyCode.A))
			settings.SetIsRotatingForward(false);
	}

	private void DebugInformation(){

		float time; string timeUnit;
		fps = 1/Time.deltaTime;

		if (t < Times.SecondsPerMinute){
			time = t;
			timeUnit = "seconds";
		}
		else if (t < Times.SecondsPerHour){
			time = t / Times.SecondsPerMinute;
			timeUnit = "minutes";
		}
		else if (t < Times.SecondsPerDay){
			time = t / Times.SecondsPerHour;
			timeUnit = "hours";
		}
		else if (t < Times.SecondsPerWeek){
			time = t / Times.SecondsPerDay;
			timeUnit = "days";
		}
		else if (t < Times.SecondsPerMonth){
			time = t / Times.SecondsPerWeek;
			timeUnit = "weeks";
		}
		else if (t < Times.SecondsPerYear){
			time = t / Times.SecondsPerMonth;
			timeUnit = "months";
		}
		else{
			time = t / Times.SecondsPerYear;
			timeUnit = "years";
		}

		informations.SetText(
			$"Speed : {(float)(speedMultiplier * settings.secondsPerFrame * 50)}x  |  Scale : {scale}\nTime ({timeUnit}): {time}\nFPS : {fps}"
		);
	}

	private void FixedUpdate(){

		for (int i = 0; i < speedMultiplier; i++)
			CalculateOneFrame();

		foreach(Body body in bodies)
			body.DrawLine();

		if (settings.isRotatingForward)
			settings.thrustDirection = settings.thrustDirection.SumVectorAsAngle(Vector2Double.ToVector2Double(0.08));

		if (settings.isRotatingBackward)
			settings.thrustDirection = settings.thrustDirection.SubtractVectorAsAngle(Vector2Double.ToVector2Double(0.08));
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

	public void StopTime(){
		speedMultiplier = 0;
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
		settingsController.bodyToCreate.nCollisions = 0;

		createdBody.Initialize(this);
		CompileFunctions(createdBody);
		
		bodies.Add(createdBody);

		List<string> nameToList = new List<string>() {createdBody.name};
		settingsController.parent.AddOptions(nameToList);
		bodyEditor.bodiesDropdown.AddOptions(nameToList);

		cameraController.Index = bodies.Count - 1;

		if (createdBody.controllable)
			thrustControls.gameObject.SetActive(true);
	}

	public void DeleteBody(Body body){

		bool showControls = false;

		foreach (Body body1 in bodies)
			if (body1.controllable && body1 != body)
				showControls = true;

		thrustControls.gameObject.SetActive(showControls);
		
		int index = body.Index();

		settingsController.BodyEliminated(index);
		bodyEditor.BodyEliminated(index);
		cameraController.BodyEliminated(index);

		bodies.Remove(body);

		DestroyImmediate(body.gameObject);
	}

	public void DeleteAllBodies(){
		while(bodies.Count > 0)
			DeleteBody(bodies[0]);
	}

	public void CompileFunctions(Body body){

		body.ForceOnce = new Action(() => {});
		body.ForceAfterPosition = new Action(() => {}); // add friction
		body.ForceEachBody = new Action<Body>((body2) => {});
		
		if (settings.fluidDensity != 0 && settings.dragCoefficient != 0)
			body.ForceOnce += () => body.Drag();
		
		if (settings.gravityAcceleration != 0){

			switch(settings.gravityMode){

				case GravityMode.Directional:

					if (settings.fluidDensity != 0)
						body.ForceOnce += () => body.DirectionalGravityBuoyancy();
					else
						body.ForceOnce += () => body.DirectionalGravity();

					break;

				case GravityMode.Centered:

					if (settings.fluidDensity != 0)
						body.ForceOnce += () => body.CenteredGravityBuoyancy();
					else
						body.ForceOnce += () => body.CenteredGravity();

					break;

				case GravityMode.Velocity:

					if (settings.fluidDensity != 0)
						body.ForceOnce += () => body.VelocityGravityBuoyancy();
					else
						body.ForceOnce += () => body.VelocityGravity();

					break;
			}	
		}

		if (body.controllable)
			body.ForceOnce += () => body.Thrust();

		if (settings.attractionGravityConstant != 0){

			if (settings.fluidDensity == 0)
				body.ForceEachBody += (body2) => body.AttractionGravity(body2);
			else
				body.ForceEachBody += (body2) => body.AttractionGravityBuoyancy(body2);
		}

		if (settings.calculateCollisions){

			if (settings.mergeBodiesInCollisions)
				body.ForceEachBody += (body2) => body.CollisionMerge(body2);
			else
				body.ForceEachBody += (body2) => body.Collision(body2);
		}

		if (settings.borderMode == BorderMode.Rectangle)
			body.ForceAfterPosition += () => body.CheckRectangleCollision();

		else if (settings.borderMode == BorderMode.Circle)
			body.ForceAfterPosition += () => body.CheckCircleCollision();
	}
}
