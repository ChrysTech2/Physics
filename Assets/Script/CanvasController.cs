using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerController : MonoBehaviour{
	
	[SerializeField] private CanvasScaler canvasScaler;
	private float lastWidth, lastHeight;
	[SerializeField] private float ratio;

	private void Start(){
		lastWidth = Screen.width;
		lastHeight = Screen.height;

		AdjustScaler();
	}
	
	private void Update(){

		if (lastWidth != Screen.width || lastHeight != Screen.height){

			Debug.Log((float)Screen.width / Screen.height);
			AdjustScaler();
			
			lastWidth = Screen.width;
			lastHeight = Screen.height;
		}
	}

	private void AdjustScaler(){

		if ((float)Screen.width / Screen.height <= ratio)
			canvasScaler.matchWidthOrHeight = 0;
		else
			canvasScaler.matchWidthOrHeight = 1;
	}
}
