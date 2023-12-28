using UnityEngine;

public class WorldLine : MonoBehaviour{

	public Vector2Double position = Vector2Double.zero;
	public BodyController bodyController;
	public double height;

	public void ApplyPosition(){

		transform.localPosition = (position - bodyController.cameraController.position).ToVector2();
		transform.localScale = new Vector2(transform.localScale.x, (float)height / bodyController.scale);
	}
	
	private void OnDestroy(){
		bodyController.lineController.lines.Remove(this);
	}
}

