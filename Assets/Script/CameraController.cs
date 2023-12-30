using TMPro;
using UnityEngine;
public class CameraController : MonoBehaviour{

	[SerializeField] private BodyController bodyController;
	[SerializeField] private TMP_Text focusModeText;
	[SerializeField] private double sensibiliy = 6;
	[SerializeField] private TouchControl touchControl;

	private SettingsController settingsController;
	private BodyEditor bodyEditor;

	private int focus = 0;
	private int index = -1;

	private Vector2Double bodyPosition = Vector2Double.zero;
	private Vector2Double offset = Vector2Double.zero;

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

	private bool canCalculateOffset = false;

	public void CalculateOffset(){

		if (Input.GetKeyDown(KeyCode.Mouse0)){

			bool condition1 = !settingsController.gameObject.activeSelf;
			bool condition2 = bodyEditor.gameObject.activeSelf && !UIHitboxController.MouseOverBodyEditor;
			bool condition3 = !bodyEditor.gameObject.activeSelf && !touchControl.addOnTouch.isOn;
			bool condition4 = UIHitboxController.MouseOverControls || UIHitboxController.MouseOverAddOnTouchButton;
			bool condition5 = Input.touchCount < 2;

			canCalculateOffset = Index != -1 && condition5 && ((condition1 && (condition2 || condition3)) || condition4);

			if (canCalculateOffset)
				CheckBodiesAtMousePosition();
		}

		if (Input.GetKeyUp(KeyCode.Mouse0))
			canCalculateOffset = false;

		if (!canCalculateOffset)
			return;

		float mouseX = -Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");

		offset += new Vector2Double(mouseX, mouseY) / (sensibiliy * bodyController.scale);
	}

	private void Center(){

		if (Focus != FocusMode.Disabled){
			offset = Vector2Double.zero;
		}
	}

	public void NextBody(){

		if (Index == -1)
			return;

		focus = FocusMode.Enabled;

		if (Index == bodyController.bodies.Count - 1)
			Index = 0;
		else
			Index ++;
	}

	public void PreviousBody(){

		if (Index == -1)
			return;

		focus = FocusMode.Enabled;

		if (Index == 0)
			index = bodyController.bodies.Count - 1;
		else
			Index --;		
	}

	public void ChangeFocusMode(){

		/*if (Focus == FocusMode.YAxis)
			Focus = FocusMode.Disabled;
		else
			Focus ++;*/

		if (Focus == FocusMode.Enabled)
			Focus = FocusMode.Disabled;
		else	
			Focus = FocusMode.Enabled;
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
			focusModeText.SetText($"F : {FocusModeToString()}");
			Center();
		}
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
				Index = body.Index();
				return;
			}
		}
	}	
}
