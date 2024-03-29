using System;
using TMPro;
using UnityEngine;
public class CameraController : MonoBehaviour{

	[SerializeField] private BodyController bodyController;
	[SerializeField] private TouchControl touchControl;
	[SerializeField] private TMP_Text mouseOnBody;
	[SerializeField] private double sensibiliy = 6, rotationSensibiliy = 6;
	[SerializeField] private TMP_Text focusModeText;

	private SettingsController settingsController;
	private BodyEditor bodyEditor;

	public int focus = 0;
	public int index = -1;

	public Vector2Double bodyPosition = Vector2Double.zero;
	public Vector2Double offset = Vector2Double.zero;

	public Vector2Double position = Vector2Double.zero;

	private void Start(){
		transform.position = -Vector3.forward;
		settingsController = bodyController.settingsController;
		bodyEditor = bodyController.bodyEditor;
	}
	
	private void Update(){

		if (Index == -1)
			return;

		Body body = bodyController.bodies[index];
		
		switch(Focus){
			case FocusMode.Enabled:
				bodyPosition = body.position;
				break;
			case FocusMode.XAxis:
				bodyPosition.x = body.position.x;
				break;
			case FocusMode.YAxis:
				bodyPosition.y = body.position.y;
				break;
		}

		position = bodyPosition + offset;
	}

	private void CheckRotationInput(){

		if (settingsController.gameObject.activeSelf)
			return;

		if (Input.GetKeyDown(KeyCode.R))
			bodyController.settings.SetIsCameraRotatingRight(true);
		if (Input.GetKeyDown(KeyCode.F))
			bodyController.settings.SetIsCameraRotatingLeft(true);

		if (Input.GetKeyUp(KeyCode.R))
			bodyController.settings.SetIsCameraRotatingRight(false);
		if (Input.GetKeyUp(KeyCode.F))
			bodyController.settings.SetIsCameraRotatingLeft(false);

		if (bodyController.settings.isCameraRotatingLeft)
			RotateLeft();
		if (bodyController.settings.isCameraRotatingRigtht)
			RotateRight();
	}

	private void RotateLeft(){
		transform.eulerAngles = Vector3.forward * (transform.eulerAngles.z - (float)rotationSensibiliy * Time.deltaTime);
	}
	private void RotateRight(){
		transform.eulerAngles = Vector3.forward * (transform.eulerAngles.z + (float)rotationSensibiliy * Time.deltaTime);
	}

	public bool canCalculateOffset = false;
	public void CalculateOffset(){

		CheckRotationInput();

		if (Input.GetKeyDown(KeyCode.Mouse0)){

			bool condition1 = !settingsController.gameObject.activeSelf;
			bool condition2 = bodyEditor.gameObject.activeSelf && !UIHitboxController.MouseOverBodyEditor;
			bool condition3 = !bodyEditor.gameObject.activeSelf && !touchControl.addOnTouch.isOn;
			bool condition4 = UIHitboxController.MouseOverControls || UIHitboxController.MouseOverAddOnTouchButton;
			bool condition5 = !UIHitboxController.MouseOverThrustControls && !UIHitboxController.MouseOverCameraControls;

			canCalculateOffset = Index != -1 && condition5 && ((condition1 && (condition2 || condition3)) || condition4);
		}

		if (Input.GetKeyUp(KeyCode.Mouse0)){
			canCalculateOffset = false;
			mouseOnBody.SetText("");
		}

		if (!canCalculateOffset)
			return;

		if (touchControl.twoFingers)
			return;

		float mouseX = -Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");

		if (touchControl.twoFingersOld){
			mouseX = 0;
			mouseY = 0;
		}

		if (Math.Abs(mouseX) < 0.05 && Math.Abs(mouseY) < 0.05)
			CheckBodiesAtMousePosition();
		
		Vector2Double offsetVector = new Vector2Double(mouseX, mouseY);
		Vector2Double direction = Vector2Double.ToVector2Double(transform.eulerAngles.z * Math.PI / 180);
		offset += offsetVector.magnitude * offsetVector.direction.SumVectorAsAngle(direction) / (sensibiliy * bodyController.scale);
	}

	private void Center(){

		if (Focus != FocusMode.Disabled)
			offset = Vector2Double.zero;
	}

	public void NextBody(){

		if (Index == -1)
			return;

		Focus = FocusMode.Enabled;

		if (Index == bodyController.bodies.Count - 1)
			Index = 0;
		else
			Index ++;
	}

	public void PreviousBody(){

		if (Index == -1)
			return;

		Focus = FocusMode.Enabled;

		if (Index == 0)
			Index = bodyController.bodies.Count - 1;
		else
			Index --;		
	}

	public void ChangeFocusMode(){

		if (Focus == FocusMode.YAxis)
			Focus = FocusMode.Disabled;
		else
			Focus ++;
	}

	public string FocusModeToString(){

		switch(focus){

			case FocusMode.Disabled:
				return "Off";
			case FocusMode.Enabled:
				return "On";
			case FocusMode.XAxis:
				return "X";
			case FocusMode.YAxis:
				return "Y";
		}

		return "Error";
	}

	public int Index{
		get{
			return index;
		}
		set{
			index = value;
			Center();
			if (index == -1){
				offset = Vector2Double.zero;
				bodyPosition = Vector2Double.zero;
				position = Vector2Double.zero;
			}
		}
	}

	public int Focus{
		get{
			return focus;
		}
		set{
			focus = value;
			SetFocusModeText();
			Center();
		}
	}

	public void SetFocusModeText(){
		focusModeText.SetText($"F : {FocusModeToString()}");
	}

	public void BodyEliminated(int index){

		if (bodyController.bodies.Count == 1)
			Index = -1;
		else if (Index == index)
			Index = 0;
		else if (Index > index)
			Index --;
	}

	private void CheckBodiesAtMousePosition(){

		foreach (Body body in bodyController.bodies){

			Vector2Double mousePosition = touchControl.mouseWorldPosition;

			if (Vector2Double.Distance(mousePosition, body.position) - body.radius < 0){

				//ChangeIndexWithFixedOffset(body.Index());
				mouseOnBody.SetText($"Body : {body.name}");
				return;
			}
		}
	}	

	public void ChangeIndexWithFixedOffset(int newIndex){

		Vector2Double oldOffset = offset;
		int oldIndex = index;
		index = newIndex;

		switch(focus){

			case FocusMode.Enabled:
				offset = oldOffset + (bodyController.bodies[oldIndex].position - bodyController.bodies[index].position); 
				break;
			case FocusMode.XAxis:
				offset = oldOffset + Vector2Double.right * (bodyController.bodies[oldIndex].position.x - bodyController.bodies[index].position.x); 
				break;
			case FocusMode.YAxis:
				offset = oldOffset + Vector2Double.up * (bodyController.bodies[oldIndex].position.y - bodyController.bodies[index].position.y); 
				break;
		}
	}

	public Vector2Double rotationDirection{
		get{
			return Vector2Double.ToVector2Double(transform.eulerAngles.z * Math.PI / 180);
		}
	}
}
