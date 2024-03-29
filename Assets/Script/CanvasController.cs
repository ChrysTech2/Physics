using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerController : MonoBehaviour{
	
	[SerializeField] private CanvasScaler canvasScaler;
	[SerializeField] private GameObject startup, ui, cameraLeft, cameraRight;
	[SerializeField] private float ratio;
	[SerializeField] private BodyController bodyController;
	private float lastWidth, lastHeight;
	
	private void Start(){

		lastWidth = Screen.width;
		lastHeight = Screen.height;

		AdjustScaler();
	}
	
	private void Update(){

		if (lastWidth != Screen.width || lastHeight != Screen.height){

			AdjustScaler();
			
			lastWidth = Screen.width;
			lastHeight = Screen.height;
		}

		if (Input.GetKeyDown(KeyCode.F11))
			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, !Screen.fullScreen);
		
		if (Input.GetKeyDown(KeyCode.F2))
			ToggleUI();
		
		if (Input.GetKeyDown(KeyCode.Escape)){
			SetUIActive(true);
		}
		
	}

	public void ToggleUI(){

		if (startup.activeSelf)
			return;	

		SetUIActive(!ui.activeSelf);
	}

	private void SetUIActive(bool value){

		ui.SetActive(value);
		cameraLeft.SetActive(value);
		cameraRight.SetActive(value);

		bodyController.settingsController.gameObject.SetActive(false);
		bodyController.bodyEditor.gameObject.SetActive(false);
		bodyController.touchControl.addOnTouch.isOn = false;
	}

	private void AdjustScaler(){

		if ((float)Screen.width / Screen.height <= ratio)
			canvasScaler.matchWidthOrHeight = 0;
		else
			canvasScaler.matchWidthOrHeight = 1;
	}
}
