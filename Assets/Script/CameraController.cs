using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour{

	
	[SerializeField] private BodyController bodyController;
	[SerializeField] private TMP_Text focusModeText;
	[SerializeField] private double sensibiliy = 6;

	private SettingsController settingsController;

	private int focus = 0;
	private int index = -1;

	private Vector2Double bodyPosition = Vector2Double.zero;
	private Vector2Double offset = Vector2Double.zero;

	public Vector2Double position = Vector2Double.zero;

	private void Start(){
		transform.position = -Vector3.forward;
		settingsController = bodyController.settingsController;
	}
	
	private void Update(){

		CalculateOffset();

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

	private void CalculateOffset(){

		if (!Input.GetKey(KeyCode.Mouse0))
			return;

		if (settingsController.gameObject.activeSelf && Input.mousePosition.y / Screen.height > 0.2)
			return;

		float x = -Input.GetAxis("Mouse X");
		float y = -Input.GetAxis("Mouse Y");

		offset += new Vector2Double(x, y) / (sensibiliy * bodyController.scale);
	}

	private void Center(){

		if (Focus != FocusMode.Disabled)
			offset = Vector2Double.zero;

	}

	public void NextBody(){

		if (Index == -1)
			return;

		if (Index == bodyController.bodies.Count - 1)
			Index = 0;
		else
			Index ++;
	}

	public void PreviousBody(){

		if (Index == -1)
			return;

		if (Index == 0)
			index = bodyController.bodies.Count - 1;
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
		
}
