using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BodyEditor : MonoBehaviour{

	[SerializeField] private BodyController bodyController;

	public TMP_Dropdown bodiesDropdown;

	// Input
	[SerializeField] private TMP_InputField x, y;
	[SerializeField] private TMP_InputField velocityX, velocityY;
	[SerializeField] private TMP_InputField mass, radius;

	// Output
	[SerializeField] private TMP_InputField outputX, outputY;
	[SerializeField] private TMP_InputField outputVelocityX, outputVelocityY;
	[SerializeField] private TMP_InputField outputMass, outputRadius;
	[SerializeField] private Toggle showColor, showAngle;
	[SerializeField] private Image backgroundImage;
	[SerializeField] private Button showEditorButton;

	public Body bodyToEdit;

	private void OnEnable(){
		bodyController.touchControl.addOnTouch.gameObject.SetActive(false);
		showEditorButton.transform.GetChild(0).GetComponent<TMP_Text>().text = ">";
	}

	private void OnDisable(){
		bodyController.touchControl.addOnTouch.gameObject.SetActive(true);
		showEditorButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "<";
	}

	public void ToggleMenu(){

		if (gameObject.activeSelf){
			gameObject.SetActive(false);
			return;
		}

		if (bodiesDropdown.options.Count > 0){
			gameObject.SetActive(true);	
			DropdownValue = bodyController.cameraController.Index;
		}
	}

	public void AssignBody(){
		bodyToEdit = bodyController.bodies[DropdownValue];
		OnShowColorChange();
	}

	private void Update(){

		if (!showAngle.isOn){
		
			outputX.text = ((float)bodyToEdit.position.x).ToString() + " m";
			outputY.text = ((float)bodyToEdit.position.y).ToString() + " m";

			outputVelocityX.text = ((float)bodyToEdit.velocity.x).ToString() + " m/s";
			outputVelocityY.text = ((float)bodyToEdit.velocity.y).ToString()+ " m/s";

			outputMass.text = ((float)bodyToEdit.mass).ToString()+ " Kg";
			outputRadius.text = ((float)bodyToEdit.radius).ToString()+ " m";
		}
		else{

			outputX.text = ((float)bodyToEdit.position.magnitude).ToString() + " m";
			outputY.text = ((float)bodyToEdit.position.ToDegrees()).ToString() + "°";

			outputVelocityX.text = ((float)bodyToEdit.velocity.magnitude).ToString() + " m/s";
			outputVelocityY.text = ((float)bodyToEdit.velocity.ToDegrees()).ToString() + "°";

			outputMass.text = ((float)bodyToEdit.Volume).ToString() + " m^3";
			outputRadius.text = ((float)bodyToEdit.Density).ToString() + " Kg/m^3";
		}
	}

	public void OnShowAngleChange(){

		if (!showAngle.isOn){

			Utils.SetTextChild(outputX, "X", 2);
			Utils.SetTextChild(outputY, "Y", 2);

			Utils.SetTextChild(outputVelocityX, "Vx", 2);
			Utils.SetTextChild(outputVelocityY, "Vy", 2);

			Utils.SetTextChild(outputMass, "Mass", 2);
			Utils.SetTextChild(outputRadius, "Radius", 2);
			
			return;
		}

		Utils.SetTextChild(outputX, "Dst", 2);
		Utils.SetTextChild(outputY, "Angle", 2);

		Utils.SetTextChild(outputVelocityX, "V", 2);
		Utils.SetTextChild(outputVelocityY, "Angle", 2);

		Utils.SetTextChild(outputMass, "Vlm", 2);
		Utils.SetTextChild(outputRadius, "Dst", 2);	
	}

	public void OnShowColorChange(){

		if (!showColor.isOn){
			backgroundImage.color = new Color(0.46f, 0.46f, 0.46f, 0.78f);
			return;
		}

		Color color = bodyToEdit.color;
		backgroundImage.color = new Color(color.r, color.g, color.b, 0.78f);		
	}

	public void FocusBody(){
		
		bodyController.cameraController.Focus = FocusMode.Enabled;
		bodyController.cameraController.Index = bodiesDropdown.value;
	}

	public void DeleteBody(){
		AssignBody();
		bodyController.DeleteBody(bodyToEdit);
	}

	public void BodyEliminated(int index){

		bodiesDropdown.options.RemoveAt(index);

		if (DropdownValue < index)
			return;

		if (bodiesDropdown.value > index){
			DropdownValue --;
			AssignBody();
			return;
		}

		if (bodiesDropdown.options.Count == 0){
			gameObject.SetActive(false);
			return;
		}

		DropdownValue = 0;
		AssignBody();
	}

	private int DropdownValue{

		get{
			return bodiesDropdown.value;
		}
		set{
			bodiesDropdown.value = value;
			bodiesDropdown.RefreshShownValue();
			AssignBody();
		}
	}

	public void ChangePositionX(bool sum = false){

		ExpressionEvaluator.Evaluate(x.text, out double value);

		if (sum)
			value += bodyToEdit.position.x;

		bodyToEdit.position.x = value;
	}

	public void ChangePositionY(bool sum = false){

		ExpressionEvaluator.Evaluate(y.text, out double value);

		if (sum)
			value += bodyToEdit.position.y;

		bodyToEdit.position.y = value;
	}

	public void ChangeVelocityX(bool sum = false){

		ExpressionEvaluator.Evaluate(velocityX.text, out double value);

		if (sum)
			value += bodyToEdit.velocity.x;

		bodyToEdit.velocity.x = value;
	}

	public void ChangeVelocityY(bool sum = false){

		ExpressionEvaluator.Evaluate(velocityY.text, out double value);

		if (sum)
			value += bodyToEdit.velocity.y;
		
		bodyToEdit.velocity.y = value;
	}

	public void ChangeRadius(bool sum = false){

		ExpressionEvaluator.Evaluate(radius.text, out double value);

		if (sum)
			value += bodyToEdit.radius;
		
		if (value > 0)
			bodyToEdit.SetRadius(value);
	}

	public void ChangeMass(bool sum = false){

		ExpressionEvaluator.Evaluate(mass.text, out double value);

		if (sum)
			value += bodyToEdit.mass;
		
		if (value != 0)
			bodyToEdit.mass = value;
	}
}
