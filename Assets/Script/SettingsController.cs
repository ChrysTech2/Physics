using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.Mathematics;

public class SettingsController : MonoBehaviour{

	// Global Settings Input
	[SerializeField] private TMP_InputField secondsPerFrame;
	[SerializeField] private TMP_InputField globalGravity, globalGravityAngle;
	[SerializeField] private TMP_InputField airDensity, dragCoefficient;
	[SerializeField] private TMP_InputField gravityCostant;
	[SerializeField] private Toggle calculateCollisions, mergeBodiesInCollisions;
	[SerializeField] private Toggle calculateBuoyancy;
	[SerializeField] private TMP_InputField coefOfRestitution;
	
	// Body Settings Input
	[SerializeField] private TMP_InputField x, y;
	[SerializeField] private TMP_InputField velocityX, velocityY;
	[SerializeField] private TMP_InputField mass, radius, bodyName;
	[SerializeField] private Slider r, g, b, a;

	// Other Stuff
	[SerializeField] private Image colorPreview;
	[SerializeField] private TMP_Text errorMessage;

	public Settings settings;
	private BodyController bodyController;
	public Body bodyToCreate;
	
	private void Start(){
		bodyController = FindObjectOfType<BodyController>();
		errorMessage.SetText("");	
	}

	public void AddBody(){

		ApplyDataToBody();
		
		CheckForErrors();

		if (errorMessage.text == ""){
			bodyController.AddBody();
		}

		BodyFunctionCompiler();
		
	}
	private void ApplyDataToBody(){

		ExpressionEvaluator.Evaluate(x.text, out bodyToCreate.position.x);
		ExpressionEvaluator.Evaluate(y.text, out bodyToCreate.position.y);

		ExpressionEvaluator.Evaluate(velocityX.text, out bodyToCreate.velocity.x);
		ExpressionEvaluator.Evaluate(velocityY.text, out bodyToCreate.velocity.y);

		ExpressionEvaluator.Evaluate(mass.text, out bodyToCreate.mass);
		ExpressionEvaluator.Evaluate(radius.text, out bodyToCreate.radius);

		bodyToCreate.name = bodyName.text;

		bodyToCreate.color = new Color(r.value, g.value, b.value, a.value);

		ApplyDataToSettings();
	}

	public void ApplyDataToSettings(){

		ExpressionEvaluator.Evaluate(secondsPerFrame.text, out settings.secondsPerFrame);

		ExpressionEvaluator.Evaluate(globalGravity.text, out settings.globalGravity);
		ExpressionEvaluator.Evaluate(globalGravityAngle.text, out settings.globalGravityAngle);

		ExpressionEvaluator.Evaluate(airDensity.text, out settings.airDensity);
		ExpressionEvaluator.Evaluate(dragCoefficient.text, out settings.dragCoefficient);

		ExpressionEvaluator.Evaluate(gravityCostant.text, out settings.gravityConstant);

		ExpressionEvaluator.Evaluate(coefOfRestitution.text, out settings.coefOfRestitution);

		settings.calculateBuoyancy = calculateBuoyancy.isOn;
		settings.calculateCollisions = calculateCollisions.isOn;
		settings.mergeBodiesInCollisions = mergeBodiesInCollisions.isOn;
	}

	private void BodyFunctionCompiler(){


		foreach (Body body in bodyController.bodies){

			body.ForceOnce = new Action(() => {});
			body.ForceEachBody = new Action<Body>((body2) => {});
			
			if (settings.airDensity != 0 && settings.dragCoefficient != 0)
				body.ForceOnce += () => body.AirDrag();
			
			if (settings.globalGravity != 0){

				body.ForceOnce += () => body.DirectionalGravity();

				if (settings.airDensity != 0 && settings.calculateBuoyancy)
					body.ForceOnce += () => body.Buoyancy();
			}

			if (settings.gravityConstant != 0)
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

	private void CheckForErrors(){

		errorMessage.SetText("");

		if (bodyToCreate.mass == 0){

			errorMessage.SetText("The mass cannot be zero Kg.");
			return;
		}

		if (bodyToCreate.radius <= 0){

			errorMessage.SetText("The radius cannot be zero m.");
			return;
		}

		if(!Utils.IsValidName(bodyToCreate.name)){

			errorMessage.SetText("That name is not valid.");
			return;
		}

		foreach (Body body in bodyController.bodies){

			if (body.position == bodyToCreate.position){
				errorMessage.SetText("There is already a body in that position.");
				return;
			}

			if (body.name == bodyToCreate.name){
				errorMessage.SetText("There is already a body with that name.");
				return;
			}
		}
	}

	public void OnColorChange(){
		colorPreview.color = new Color(r.value, g.value, b.value, a.value);
	}

	// Events
	public void OnCollisionsSettingsChange(){

		mergeBodiesInCollisions.gameObject.SetActive(calculateCollisions.isOn);
		coefOfRestitution.gameObject.transform.parent.gameObject.SetActive(calculateCollisions.isOn);
	
	}
}
