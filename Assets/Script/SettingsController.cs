using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SettingsController : MonoBehaviour{

	// Settings Input
	public TMP_InputField secondsPerFrame, lineDuration, lineThickness;
	public TMP_Dropdown gravityMode, parent, borderMode;
	public TMP_InputField gravityAcceleration, gravityAngle;
	public TMP_InputField attractionGravityConstant, liftCoefficient, impulseForce;
	public TMP_InputField fluidDensity, dragCoefficient, frictionCoefficient;
	public Toggle calculateCollisions, mergeBodiesInCollisions, calculateBuoyancy;
	public TMP_InputField coefOfRestitution, borderCoefOfRestitution, thrustAcceleration;
	public Toggle useParent, sumParentRadius, sumBodyRadius, sumAutoVelocity;
	public Toggle randomMode, showCenterOfGravity, controllable;
	public TMP_InputField borderX, borderY, touchMultiplier;
	public TMP_InputField thrustDirectionSensibiliy;
	
	// Body Input
	public TMP_InputField x, y;
	public TMP_InputField velocityX, velocityY;
	public TMP_InputField mass, radius, bodyName;
	public Slider r, g, b, a;

	// Other Stuff
	[SerializeField] private Toggle toggleEditor;
	public TMP_Text errorMessage;

	public Settings settings;
	public BodyController bodyController;
	public GameObject cameraControls;
	
	public Body bodyToCreate;
	public int n = 1;
	
	private void Start(){
		errorMessage.SetText("");
		bodyToCreate.nCollisions = 0;
	}

	private void OnEnable(){
		bodyController.informations.gameObject.SetActive(false);
		bodyController.touchControl.addOnTouch.gameObject.SetActive(false);
		bodyController.thrustControls.transform.GetChild(0).gameObject.SetActive(false);
		cameraControls.SetActive(false);
	}

	private void OnDisable(){

		bodyController.informations.gameObject.SetActive(true);
		cameraControls.SetActive(true);

		if (!bodyController.bodyEditor.gameObject.activeSelf){
			bodyController.touchControl.addOnTouch.gameObject.SetActive(true);
			bodyController.thrustControls.transform.GetChild(0).gameObject.SetActive(true);	
		}
	}

	public void AddBody(bool addedOnTouch = false){

		ApplyDataToBody(addedOnTouch);
		ApplyParent(addedOnTouch);
		ApplyDataToSettings();
		CheckForErrors();

		if (errorMessage.text == ""){
			bodyController.AddBody();
			n++;
			bodyName.text = "Body " + n;
		}
	}

	public void ApplyDataToBody(bool addedOnTouch){

		if (!addedOnTouch){
			ExpressionEvaluator.Evaluate(x.text, out bodyToCreate.position.x);
			ExpressionEvaluator.Evaluate(y.text, out bodyToCreate.position.y);

			ExpressionEvaluator.Evaluate(velocityX.text, out bodyToCreate.velocity.x);
			ExpressionEvaluator.Evaluate(velocityY.text, out bodyToCreate.velocity.y);
		}

		ExpressionEvaluator.Evaluate(mass.text, out bodyToCreate.mass);
		ExpressionEvaluator.Evaluate(radius.text, out bodyToCreate.radius);

		bodyToCreate.controllable = controllable.isOn;

		bodyToCreate.name = bodyName.text;

		if (randomMode.isOn)
			ApplyRandomDataToBody(addedOnTouch);
		else
			bodyToCreate.color = new Color(r.value, g.value, b.value, a.value);	
	}

	private void ApplyRandomDataToBody(bool addedOnTouch){

		System.Random random = new System.Random();

		float r = (float)random.NextDouble();
		float g = (float)random.NextDouble();
		float b = (float)random.NextDouble();

		bodyToCreate.color = new Color(r, g, b, 1f);	

		if (addedOnTouch)
			return;

		int maxValueX = (int)Math.Abs(bodyToCreate.position.x);
		int maxValueY = (int)Math.Abs(bodyToCreate.position.y);

		int maxValueVX = (int)Math.Abs(bodyToCreate.velocity.x);
		int maxValueVY = (int)Math.Abs(bodyToCreate.velocity.y);

		bodyToCreate.velocity.x = random.Next(-maxValueVX, maxValueVX + 1);
		bodyToCreate.velocity.y = random.Next(-maxValueVY, maxValueVY + 1);
		
		bodyToCreate.position.x = random.Next(-maxValueX, maxValueX + 1);
		bodyToCreate.position.y = random.Next(-maxValueY, maxValueY + 1);
	}

	public void ApplyDataToSettings(){

		ExpressionEvaluator.Evaluate(secondsPerFrame.text, out settings.secondsPerFrame);

		ExpressionEvaluator.Evaluate(gravityAcceleration.text, out settings.gravityAcceleration);
		ExpressionEvaluator.Evaluate(gravityAngle.text, out settings.gravityAngle);

		ExpressionEvaluator.Evaluate(fluidDensity.text, out settings.fluidDensity);
		ExpressionEvaluator.Evaluate(dragCoefficient.text, out settings.dragCoefficient);
		ExpressionEvaluator.Evaluate(frictionCoefficient.text, out settings.frictionCoefficient);

		ExpressionEvaluator.Evaluate(attractionGravityConstant.text, out settings.attractionGravityConstant);

		ExpressionEvaluator.Evaluate(coefOfRestitution.text, out settings.coefOfRestitution);
		ExpressionEvaluator.Evaluate(borderCoefOfRestitution.text, out settings.borderCoefOfRestitution);

		ExpressionEvaluator.Evaluate(borderX.text, out settings.border.x);
		ExpressionEvaluator.Evaluate(borderY.text, out settings.border.y);

		ExpressionEvaluator.Evaluate(lineDuration.text, out settings.lineDuration);
		ExpressionEvaluator.Evaluate(lineThickness.text, out settings.lineThickness);

		ExpressionEvaluator.Evaluate(touchMultiplier.text, out settings.touchMultiplier);

		ExpressionEvaluator.Evaluate(thrustAcceleration.text, out settings.thrustAcceleration);
		ExpressionEvaluator.Evaluate(thrustDirectionSensibiliy.text, out settings.thrustDirectionSensibiliy);

		ExpressionEvaluator.Evaluate(liftCoefficient.text, out settings.liftCoefficient);
		ExpressionEvaluator.Evaluate(impulseForce.text, out settings.impulseForce);

		settings.calculateBuoyancy = calculateBuoyancy.isOn;

		settings.calculateCollisions = calculateCollisions.isOn;
		settings.mergeBodiesInCollisions = mergeBodiesInCollisions.isOn;

		settings.gravityMode = gravityMode.value;
		settings.borderMode = borderMode.value;

		settings.showCenterOfGravity = showCenterOfGravity.isOn;
		bodyController.lineController.centerOfGravity.gameObject.SetActive(showCenterOfGravity.isOn);

		settings.gravityDirection = Vector2Double.ToVector2Double(settings.gravityAngle * Math.PI/180);
		settings.gravity = settings.gravityDirection * settings.gravityAcceleration;

		DrawBorder();

		foreach (Body body in bodyController.bodies){
			bodyController.CompileFunctions(body);
			body.collidedBodies.Clear();
		}
	}

	private void DrawBorder(){

		bodyController.lineController.DeleteAllLines("Border");

		if (borderMode.value == BorderMode.Rectangle)
			bodyController.lineController.DrawRectangle(settings.border);
			
		else if (borderMode.value == BorderMode.Circle)
			bodyController.lineController.DrawPolygon(settings.border.x, 100);
	}

	private void ApplyParent(bool addedOnTouch){

		if (!useParent.isOn)
			return;

		Vector2Double parentPosition = Vector2Double.zero;
		Vector2Double parentVelocity = Vector2Double.zero;

		double parentMass = 0;
		double parentRadius = 0;

		if (parent.value != 0){
			Body parentBody = bodyController.bodies[parent.value - 1];

			parentPosition = parentBody.position;
			parentVelocity = parentBody.velocity;
			parentMass = parentBody.mass;
			parentRadius = parentBody.radius;
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

		if (!addedOnTouch)
			bodyToCreate.position += parentPosition;

		bodyToCreate.velocity += parentVelocity;

		Vector2Double direction = (bodyToCreate.position - parentPosition).direction;

		if (direction == Vector2Double.zero)
			direction = Vector2Double.up;

		if (sumBodyRadius.isOn && !addedOnTouch)
			bodyToCreate.position += direction * bodyToCreate.radius;
		
		if (sumParentRadius.isOn && !addedOnTouch)
			bodyToCreate.position += direction * parentRadius;

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

		if (settings.border.x <= 0 || settings.border.y <= 0){
			errorMessage.SetText("The border/radius cannot be negative or 0.");
			return;
		}

		if (borderMode.value == BorderMode.Rectangle){
		
			if (bodyToCreate.position.y - bodyToCreate.radius < -settings.border.y 	||
				bodyToCreate.position.y + bodyToCreate.radius > settings.border.y 	||
				bodyToCreate.position.x - bodyToCreate.radius < -settings.border.x 	||
				bodyToCreate.position.x + bodyToCreate.radius > settings.border.x){

				errorMessage.SetText("The body cannot be outside the border.");
				return;
			}

			if (bodyToCreate.radius >= settings.border.x || bodyToCreate.radius >= settings.border.y){
				errorMessage.SetText("The body cannot fit in the border.");
				return;
			}
		}

		else if (borderMode.value == BorderMode.Circle){

			if (bodyToCreate.position.magnitude + bodyToCreate.radius > settings.border.x){
				errorMessage.SetText("The body cannot be outside the border.");
				return;
			}

			if (bodyToCreate.radius >= settings.border.x){
				errorMessage.SetText("The body cannot fit in the border.");
				return;
			}
		}

		foreach (Body body in bodyController.bodies){

			if (Vector2Double.Distance(bodyToCreate.position, body.position) < Math.Pow(10,-5)){
				errorMessage.SetText("There is already a body in that position.");
				return;
			}

			if (body.name == bodyToCreate.name){
				errorMessage.SetText("There is already a body with that name.");
				return;
			}
		}
	}

	public void BodyEliminated(int index){

		parent.options.RemoveAt(index + 1);

		if (parent.value == index + 1)
			parent.value = 0;
		else if (parent.value > index + 1)
			parent.value --;

		parent.RefreshShownValue();
	}

	public void SetGToRealValue(){
		attractionGravityConstant.text = "6.6743e-11";
	}

	public void ResetErrors(){
		errorMessage.SetText("");
	}
}
