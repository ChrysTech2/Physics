using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerController : MonoBehaviour{
	
	[SerializeField] private CanvasScaler canvasScaler;
	[SerializeField] private GameObject startup, ui, info;
	[SerializeField] private float ratio;
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
		
		if (Input.GetKeyDown(KeyCode.F2)){

			if (startup.activeSelf)
				return;

			ui.SetActive(!ui.activeSelf);
			info.SetActive(!info.activeSelf);
		}

		if (Input.GetKeyDown(KeyCode.Escape)){
			ui.SetActive(true);
			info.SetActive(false);
		}
	}

	private void AdjustScaler(){

		if ((float)Screen.width / Screen.height <= ratio)
			canvasScaler.matchWidthOrHeight = 0;
		else
			canvasScaler.matchWidthOrHeight = 1;
	}
}
