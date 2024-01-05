using UnityEngine;
using TMPro;

public class GlobalDataViewer : MonoBehaviour{

	[SerializeField] private BodyController bodyController;
	[SerializeField] private TMP_InputField totalMass, totalVolume, totalArea;
	[SerializeField] private TMP_InputField nCollisions, nBodies, averageDensity;

	private Body bodyToView;

	private void FixedUpdate(){

		bodyToView = bodyController.bodyEditor.bodyToEdit;

		nBodies.text = bodyController.bodies.Count.ToString();
		nCollisions.text = bodyToView.nCollisions.ToString();

		double totMass = 0, totVolume = 0, totArea = 0, avgDensity = 0;

		for (int i = 0; i < bodyController.bodies.Count; i++){
			Body body = bodyController.bodies[i];
			totArea += body.area;
			totMass = body.mass;
			totVolume += body.volume;
			avgDensity += body.density;
		}

		avgDensity /= bodyController.bodies.Count;

		totalMass.text = ((float)totMass).ToString() + " Kg";
		totalVolume.text = ((float)totVolume).ToString() + " m^3";
		totalArea.text = ((float)totArea).ToString() + " m^2";
		averageDensity.text = ((float)avgDensity).ToString() + " Kg/m^3";

	}
}
