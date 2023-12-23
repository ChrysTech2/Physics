using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SettingsController : MonoBehaviour{

	// Settings Input
	public TMP_InputField secondsPerFrame;
	public TMP_Dropdown gravityMode, parent;
	public TMP_InputField gravityAcceleration, gravityAngle;
	public TMP_InputField attractionGravityConstant;
	public TMP_InputField fluidDensity, dragCoefficient;
	public Toggle calculateCollisions, mergeBodiesInCollisions;
	public TMP_InputField coefOfRestitution;
	public Toggle useParent, sumParentRadius, sumBodyRadius, sumAutoVelocity;
	public Toggle randomMode;
	
	// Body Input
	public TMP_InputField x, y;
	public TMP_InputField velocityX, velocityY;
	public TMP_InputField mass, radius, bodyName;
	public Slider r, g, b, a;

	// Other Stuff
	[SerializeField] private Image colorPreview;
	[SerializeField] private TMP_Text errorMessage;

	public Settings settings;
	[SerializeField] private BodyController bodyController;
	public Body bodyToCreate;

	private int n = 1;
	
	private void Start(){
		errorMessage.SetText("");	
	}

	public void AddBody(){

		ApplyDataToBody();
		ApplyParent();
		ApplyDataToSettings();
		CheckForErrors();

		if (errorMessage.text == ""){
			bodyController.AddBody();
			n++;
			bodyName.text = "Body " + n;
		}
	}

	private void ApplyDataToBody(){

		ExpressionEvaluator.Evaluate(x.text, out bodyToCreate.position.x);
		ExpressionEvaluator.Evaluate(y.text, out bodyToCreate.position.y);

		ExpressionEvaluator.Evaluate(velocityX.text, out bodyToCreate.velocity.x);
		ExpressionEvaluator.Evaluate(velocityY.text, out bodyToCreate.velocity.y);

		ExpressionEvaluator.Evaluate(mass.text, out bodyToCreate.mass);
		ExpressionEvaluator.Evaluate(radius.text, out bodyToCreate.radius);

		bodyToCreate.name = bodyName.text;

		if (randomMode.isOn)
			ApplyRandomDataToBody();
		else
			bodyToCreate.color = new Color(r.value, g.value, b.value, a.value);	
	}

	private void ApplyRandomDataToBody(){

		System.Random random = new System.Random();

		int maxValueX = (int)Math.Abs(bodyToCreate.position.x);
		int maxValueY = (int)Math.Abs(bodyToCreate.position.y);

		int maxValueVX = (int)Math.Abs(bodyToCreate.position.x);
		int maxValueVY = (int)Math.Abs(bodyToCreate.velocity.y);
		
		bodyToCreate.position.x = random.Next(-maxValueX, maxValueX + 1);
		bodyToCreate.position.y = random.Next(-maxValueY, maxValueY + 1);

		bodyToCreate.velocity.x = random.Next(-maxValueVX, maxValueVX + 1);
		bodyToCreate.velocity.y = random.Next(-maxValueVY, maxValueVY + 1);

		bodyToCreate.color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());	
	}

	public void ApplyDataToSettings(){

		ExpressionEvaluator.Evaluate(secondsPerFrame.text, out settings.secondsPerFrame);

		ExpressionEvaluator.Evaluate(gravityAcceleration.text, out settings.gravityAcceleration);
		ExpressionEvaluator.Evaluate(gravityAngle.text, out settings.gravityAngle);

		ExpressionEvaluator.Evaluate(fluidDensity.text, out settings.fluidDensity);
		ExpressionEvaluator.Evaluate(dragCoefficient.text, out settings.dragCoefficient);

		ExpressionEvaluator.Evaluate(attractionGravityConstant.text, out settings.attractionGravityConstant);

		ExpressionEvaluator.Evaluate(coefOfRestitution.text, out settings.coefOfRestitution);

		settings.calculateCollisions = calculateCollisions.isOn;
		settings.mergeBodiesInCollisions = mergeBodiesInCollisions.isOn;

		settings.gravityMode = gravityMode.value;
		
		Vector2Double gravityDirection = Vector2Double.ToVector2Double(settings.gravityAngle * Math.PI/180);
		settings.gravity = gravityDirection * settings.gravityAcceleration;

		foreach (Body body in bodyController.bodies)
			bodyController.CompileFunctions(body);
	}

	private void ApplyParent(){

		if (!useParent.isOn)
			return;

		Vector2Double parentPosition = Vector2Double.zero;
		Vector2Double parentVelocity = Vector2Double.zero;
		double parentMass = 0;

		if (parent.value != 0){
			Body parentBody = bodyController.bodies[parent.value - 1];

			parentPosition = parentBody.position;
			parentVelocity = parentBody.velocity;
			parentMass = parentBody.mass;
		}
		else{ 

			// Parent = Center of Gravity
			Vector2Double totalPositions = Vector2Double.zero;
			Vector2Double totalVelocity = Vector2Double.zero;

			foreach (Body body in bodyController.bodies){

				totalPositions += body.position * body.mass;
				totalVelocity += body.velocity * body.mass;
				parentMass += body.mass;
			}

			if (parentMass != 0){
				parentPosition = totalPositions / parentMass;
				parentVelocity = totalVelocity / parentMass;
			}

			sumParentRadius.isOn = false;
			sumBodyRadius.isOn = false;
		}

		bodyToCreate.position += parentPosition;
		bodyToCreate.velocity += parentVelocity;

		Vector2Double direction = (bodyToCreate.position - parentPosition).direction;

		if (direction == Vector2Double.zero)
			direction = Vector2Double.up;

		if (sumBodyRadius.isOn)
			bodyToCreate.position += direction * bodyToCreate.radius;
		
		if (sumParentRadius.isOn)
			bodyToCreate.position += direction * bodyToCreate.radius;

		if (sumAutoVelocity.isOn){
			double distance = Vector2Double.Distance(bodyToCreate.position, parentPosition);
			if (distance == 0)
				return;
			double autoVelocity = Math.Sqrt(settings.attractionGravityConstant * (parentMass + bodyToCreate.mass) / distance);
			bodyToCreate.velocity += direction.SumVectorAsAngle(Vector2Double.down) * autoVelocity;
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
		Utils.GetParent(coefOfRestitution).SetActive(calculateCollisions.isOn);
	}

	public void OnGravitySettingsChange(){

		Utils.GetParent(gravityAcceleration).SetActive(gravityMode.value != GravityMode.Disabled);
		Utils.GetParent(gravityAngle).SetActive(gravityMode.value != GravityMode.Disabled && gravityMode.value != GravityMode.Centered);
	}

	public void OnParentSettingsChange(){

		sumParentRadius.gameObject.SetActive(useParent.isOn && parent.value != 0);
		sumBodyRadius.gameObject.SetActive(useParent.isOn && parent.value != 0);
		sumAutoVelocity.gameObject.SetActive(useParent.isOn);
		Utils.GetParent(parent).SetActive(useParent.isOn);
	}

	public void OnForcesSettingsChange(){

		Utils.GetParent(dragCoefficient).SetActive(fluidDensity.text != "0");
	}

	public void OnRandomChange(){
		if (randomMode.isOn){
			Utils.GetTextChild(x).SetText("Max X");
			Utils.GetTextChild(y).SetText("Max Y");

			Utils.GetTextChild(velocityX).SetText("Max Velocity X");
			Utils.GetTextChild(velocityY).SetText("Max Velocity Y");

			if (x.text == "0") x.text = "20";
			if (y.text == "0") y.text = "20";

			if (velocityX.text == "0") velocityX.text = "20";
			if (velocityY.text == "0") velocityY.text = "20";
		}
		else{

			Utils.GetTextChild(x).SetText("X");
			Utils.GetTextChild(y).SetText("Y");

			Utils.GetTextChild(velocityX).SetText("Velocity X");
			Utils.GetTextChild(velocityY).SetText("Velocity Y");
		}
	}
}
