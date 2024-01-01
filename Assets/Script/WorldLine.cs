using UnityEngine;

public class WorldLine : MonoBehaviour{

	public Vector2Double position = Vector2Double.zero;
	public BodyController bodyController;
	public double height;
	public Transform lineTransform;

	public void ApplyPosition(){

		lineTransform.localPosition = (position - bodyController.cameraController.position).ToVector2();
		lineTransform.localScale = new Vector2(lineTransform.localScale.x, (float)height / bodyController.scale);
	}
	
	private void OnDestroy(){
		bodyController.lineController.lines.Remove(this);
	}
}

