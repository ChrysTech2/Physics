using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour{

	
	[SerializeField] private BodyController bodyController;
	[SerializeField] private TMP_Text focusModeText;

	private bool focus = true;
	private int index = -1;

	private Vector2Double offset = Vector2Double.zero;
	private Vector2Double bodyPosition = Vector2Double.zero;

	public Vector2Double position = Vector2Double.zero;

	private void Start(){
		 transform.position = -Vector3.forward;
	}
	
	private void Update(){


		if (index == -1)
			return;

		Body body = bodyController.bodies[index];
		
		if (focus)
			bodyPosition = body.position;


		CalculateOffset();

		position = bodyPosition + offset;
		
	  
	}

	private void CalculateOffset(){

		if (!Input.GetKey(KeyCode.Mouse0))
			return;
		
		float x = -Input.GetAxis("Mouse X");
		float y = -Input.GetAxis("Mouse Y");

		offset += new Vector2Double(x, y) / 6;
	}

	private void Center(){
		if (focus)
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

	public int Index{
		get{
			return index;
		}
		set{
			if (bodyController.bodies.Count == 1)
				focus = true;

			index = value;
			Center();
		}
	}

	public void ChangeFocusMode(){

		focus = !focus;
		focusModeText.SetText($"F: {focus}");
		Center();
	}
		
}
