using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerController : MonoBehaviour{
	
	[SerializeField] private CanvasScaler canvasScaler;
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

		if (Input.GetKeyDown(KeyCode.F11)){

			Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, !Screen.fullScreen);
			
		}
	}

	private void AdjustScaler(){

		if ((float)Screen.width / Screen.height <= ratio)
			canvasScaler.matchWidthOrHeight = 0;
		else
			canvasScaler.matchWidthOrHeight = 1;
	}
}
