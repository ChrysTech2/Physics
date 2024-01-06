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

	public List<Body> bodies = new List<Body>();
	public int speedMultiplier = 0;
	public double t = 0;
	public float fps = 60;

	private int numberOfControllableBodies = 0;

	private int NumberOfControllableBodies{
		get{
			return numberOfControllableBodies;
		}
		set{
			numberOfControllableBodies = value;

			if (numberOfControllableBodies == 0)
				thrustControls.gameObject.SetActive(false);
			else
				thrustControls.gameObject.SetActive(true);
		}
	}

	private void Start(){
		settings = settingsController.settings;
		Application.targetFrameRate = 240;
	}

	private void Update(){

		foreach (Body body in bodies)
			body.ApplyPosition();

		if (settings.isZoomingIn)
			ZoomIn(1.0075f);
		else if (settings.isZoomingOut)
			ZoomOut(1.0075f);

		if (settings.showCenterOfGravity)
			ShowCenterOfGravity();

		if (settingsController.gameObject.activeSelf)
			return;

		CheckInput();
		DebugInformation();

		if (NumberOfControllableBodies == 0)
			return;
		
		CheckThrustInput();
		
		foreach (Body body in bodies)
			if (body.controllable)
		
		lineController.CreateLine(body.position, body.position + settings.thrustDirection * body.radius * 3, body.color, true, 0.02, "InfoLine");
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

	public void CalculateOneFrame(){

		for(int i = 0; i < bodies.Count; i++)
			bodies[i].UpdateVelocity();

		foreach (Body body in bodies)
			body.UpdatePosition();

		t += (float)settingsController.settings.secondsPerFrame;
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

		fps = 1/Time.deltaTime;

		informations.SetText(
			$"Speed : {(float)(speedMultiplier * settings.secondsPerFrame * 50)}x  |  Scale : {scale}\n{Utils.FormatTime(t)}\nFPS : {fps}"
		);
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
			NumberOfControllableBodies ++;		
	}

	public void DeleteBody(Body body){

		if (body.controllable)
			NumberOfControllableBodies --;

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
