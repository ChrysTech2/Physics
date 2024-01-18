using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TouchControl : MonoBehaviour{

	[SerializeField] private BodyController bodyController;
	[SerializeField] private TMP_Text bodyInfo;

	private SettingsController settingsController;
	private BodyEditor bodyEditor;
	public Toggle addOnTouch;

	public bool canAddBody = false, bodyInstantiated = false;
	private Vector2Double worldPosition1 = Vector2Double.zero;
	private Vector2Double startVelocity = Vector2Double.zero;
	private Vector2Double velocity = Vector2Double.zero;

	void Start(){
		settingsController = bodyController.settingsController;
		bodyEditor = bodyController.bodyEditor;
	}

	public bool twoFingers = false, twoFingersOld = false;

	private void Update(){

		if (bodyController.startupMenu.activeSelf)
			return;

		twoFingers = Input.touchCount > 1;

		bodyController.cameraController.CalculateOffset();

		TouchZoom();

		TouchBodyCreation();

		twoFingersOld = Input.touchCount > 1;
	}

	private void TouchBodyCreation(){

		if (Input.GetKeyDown(KeyCode.Mouse0)){

			bool condition1 = !bodyEditor.gameObject.activeSelf && !settingsController.gameObject.activeSelf;
			bool condition2 = !UIHitboxController.MouseOverControls && !UIHitboxController.MouseOverAddOnTouchButton;
			bool condition3 = !twoFingers && !UIHitboxController.MouseOverCameraControls;
			bool condition4 = !UIHitboxController.MouseOverThrustControls;

			canAddBody = condition1 && condition2 && condition3 && condition4;

			worldPosition1 = mouseWorldPosition;

			if (canAddBody){

				if (!addOnTouch.isOn){

					bodyController.settings.lastMousePosition = worldPosition1;
					bodyController.settings.actualImpulseForce = bodyController.settings.impulseForce;

					canAddBody = false;
				}
				else{
					canAddBody = true;
				}
			}

			if (canAddBody)
				InstantiateBody();
		}

		if (Input.GetKeyUp(KeyCode.Mouse0) || twoFingers){

			bodyController.settings.actualImpulseForce = 0;

			if (bodyInstantiated){

				bodyController.lineController.DeleteAllLines("TouchLine");

				AddBody();

				bodyInfo.SetText("");

				canAddBody = false;
				bodyInstantiated = false;

			}
		}

		if (bodyInstantiated && Input.GetKey(KeyCode.Mouse0)){

			bodyController.lineController.DeleteAllLines("TouchLine");

			Vector2Double worldPosition2 = mouseWorldPosition;

			Vector2Double distance = worldPosition2 - worldPosition1;

			bodyController.lineController.CreateLine(worldPosition1, worldPosition2, Color.red, false, 0, "TouchLine");

			distance *= bodyController.scale;

			velocity.x = (Math.Log(1/bodyController.scale) + Math.Sqrt(1/bodyController.scale)) * distance.x;
			velocity.y = (Math.Log(1/bodyController.scale) + Math.Sqrt(1/bodyController.scale)) * distance.y;
			
			velocity = velocity * bodyController.settings.touchMultiplier + startVelocity;

			bodyInfo.SetText($"velocity : {(float)velocity.magnitude} m/s\n angle : {(float)velocity.ToDegrees()} °");
		}
	}

	private void InstantiateBody(){

		bodyInstantiated = true;

		ExpressionEvaluator.Evaluate(settingsController.velocityX.text, out startVelocity.x);
		ExpressionEvaluator.Evaluate(settingsController.velocityY.text, out startVelocity.y);

		if (!settingsController.randomMode.isOn)
			return;

		System.Random random = new System.Random();

		int maxValueVX = (int)Math.Abs(startVelocity.x);
		int maxValueVY = (int)Math.Abs(startVelocity.y);

		startVelocity.x = random.Next(-maxValueVX, maxValueVX + 1);
		startVelocity.y = random.Next(-maxValueVY, maxValueVY + 1);
	}

	private void AddBody(){

		Body body = bodyController.settingsController.bodyToCreate;

		body.position = worldPosition1;
		body.velocity = velocity;

		settingsController.AddBody(true);
	}

	private Vector2 mousePosition{
		get{
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}

	private Vector2Double ToWorldPosition(Vector2 mousePosition){
		return Utils.ToVector2Double(mousePosition) / bodyController.scale + bodyController.cameraController.position;
	}

	public Vector2Double mouseWorldPosition{
		get{
			return ToWorldPosition(mousePosition);
		}
	}

	private void TouchZoom(){

		if (!twoFingers)
			return;

		Touch finger1 = Input.GetTouch(0);
		Touch finger2 = Input.GetTouch(1);

		Vector2 old1 = finger1.position - finger1.deltaPosition; // delta = new - old // old = new - delta // old = new - (new - old)
		Vector2 old2 = finger2.position - finger2.deltaPosition; // old = new -new + old // old = old

		Vector2 new1 = finger1.position;
		Vector2 new2 = finger2.position;

		float deltaOld = (old2 - old1).magnitude;
		float deltaNew = (new2 - new1).magnitude;

		float delta = bodyController.fps * (deltaNew - deltaOld) / 36_000;

		bodyController.ZoomIn(1 + delta);
	}
}
