using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TouchControl : MonoBehaviour{

	[SerializeField] private BodyController bodyController;
	[SerializeField] private TMP_Text bodyInfo;

	private SettingsController settingsController;
	public Toggle addOnTouch;

	void Start(){
		settingsController = bodyController.settingsController;
	}

	public bool canAddBody = false;
	Vector2Double worldPosition1 = Vector2Double.zero;
	Vector2Double startVelocity = Vector2Double.zero;
	Vector2Double velocity = Vector2Double.zero;

	private void Update(){

		if (Input.GetKeyDown(KeyCode.Mouse0)){

			float percentageY = Input.mousePosition.y / Screen.height;
			float percentageX = Input.mousePosition.x / Screen.width;

			bool condition1 = addOnTouch.isOn && !settingsController.gameObject.activeSelf;
			bool condition2 =  percentageY > 0.2 && !(percentageY > 0.85 && percentageX > 0.75);

			canAddBody = condition1 && condition2;

			worldPosition1 = ToWorldPosition(cameraPosition);

			ExpressionEvaluator.Evaluate(settingsController.velocityX.text, out startVelocity.x);
			ExpressionEvaluator.Evaluate(settingsController.velocityY.text, out startVelocity.y);

			if (settingsController.randomMode.isOn){
				
				System.Random random = new System.Random();

				int maxValueVX = (int)Math.Abs(startVelocity.x);
				int maxValueVY = (int)Math.Abs(startVelocity.y);

				startVelocity.x = random.Next(-maxValueVX, maxValueVX + 1);
				startVelocity.y = random.Next(-maxValueVY, maxValueVY + 1);
			}
		}


		if (Input.GetKeyUp(KeyCode.Mouse0) && canAddBody){

			AddBody();

			canAddBody = false;
			bodyInfo.SetText("");
		}

		if (Input.GetKey(KeyCode.Mouse0) && canAddBody){

			Vector2Double worldPosition2 = ToWorldPosition(cameraPosition);

			Vector2Double distance = worldPosition2 - worldPosition1;

			distance *= bodyController.scale;

			velocity.x = (startVelocity.x + distance.x) * bodyController.settings.touchMultiplier;
			velocity.y = (startVelocity.y + distance.y) * bodyController.settings.touchMultiplier; 

			bodyInfo.SetText($"velocity : {(int)velocity.magnitude} m/s , angle : {(int)(Math.Atan2(velocity.y, velocity.x) * 180/Math.PI)} °");
		}
	}

	private void AddBody(){

		//Vector2Double worldPosition2 = ToWorldPosition(mousePosition2);

		Body body = bodyController.settingsController.bodyToCreate;

		body.position = worldPosition1;
		body.velocity = velocity;

		body.velocity += velocity;

		settingsController.AddBody(true);
	}

	private Vector2 cameraPosition{
		get{
			return Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}
	}

	private Vector2Double ToWorldPosition(Vector2 mousePosition){
		return Utils.ToVector2Double(mousePosition) / bodyController.scale + bodyController.cameraController.position;
	}
}
