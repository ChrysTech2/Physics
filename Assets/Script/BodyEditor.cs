using UnityEngine;
using TMPro;

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

	private Body bodyToEdit;

	public void ShowMenu(){
		if (bodiesDropdown.options.Count > 0){
			gameObject.SetActive(true);	
			bodiesDropdown.value = bodyController.cameraController.Index;
			AssignBody();
		}
	}

	public void AssignBody(){
		bodyToEdit = bodyController.bodies[bodiesDropdown.value];
		Debug.Log("cambiato");
	}

	/*private void Update(){
		
		outputX.text = ((float)bodyToEdit.position.x).ToString();
		outputX.text = ((float)bodyToEdit.position.x).ToString();

		outputVelocityX.text = ((float)bodyToEdit.velocity.x).ToString();
		outputVelocityY.text = ((float)bodyToEdit.velocity.x).ToString();

		outputMass.text = ((float)bodyToEdit.mass).ToString();
		outputRadius.text = ((float)bodyToEdit.radius).ToString();
	}*/

	public void FocusBody(){

		bodyController.cameraController.Focus = FocusMode.Enabled;
		bodyController.cameraController.Index = bodiesDropdown.value;
	}

	public void DeleteBody(){

		bodyController.DeleteBody(bodyToEdit);
	}
}
