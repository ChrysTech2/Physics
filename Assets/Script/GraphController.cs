using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class GraphController : MonoBehaviour{

	[SerializeField] private BodyController bodyController;
	[SerializeField] private GameObject graphArea;
	[SerializeField] private GameObject pointToCreate;
	[SerializeField] private TMP_Text lowerBound, upperBound;
	[SerializeField] private int offsetX, maxX, maxY;

	public Toggle position, velocity, acceleration;
	public Toggle positionAngle, velocityAngle, accelerationAngle;
	public Toggle positionX, positionY, velocityX, velocityY, accelerationX, accelerationY;
	public Toggle totalKineticEnergy, numberOfCollisions;
	public Toggle totalMomentumX, totalMomentumY;

	private List<RectTransform> points = new List<RectTransform>();
	private Body bodyToGraph;

	private void Start(){
		x = offsetX;
	}

	private void DrawPoint(Vector2 position, Color color){

		GameObject point = Instantiate(pointToCreate);

		point.transform.SetParent(graphArea.transform, true);

		point.GetComponent<Image>().color = color;

		RectTransform rectTransform = point.GetComponent<RectTransform>();

		rectTransform.sizeDelta = new Vector2(10, 10);

		rectTransform.localPosition = position;
		rectTransform.localScale = Vector2.one * 1f;

		points.Add(rectTransform);
	}

	public void DestroyAllPoints(){

		while (points.Count > 0)
			DestroyPoint(0);
		
		x = offsetX;
	}

	private void ScaleAllPoints(float scale){

		for (int i = 0; i < points.Count; i++)
			ScalePoint(points[i], scale);
	}

	private void TranslateAllPoints(){

		for (int i = 0; i < points.Count; i++){

			points[i].localPosition = points[i].localPosition - 1 * Vector3.right;
		}

		DestroyPoint(0);
		x = maxX;
	}

	private int x;
	private float max = 1;

	private void PlotPoint(float y, Color color){

		float oldMax = max;

		Vector2 position = new Vector2(x, y * maxY/oldMax);
		DrawPoint(position, color);

		if (points.Count > 0)
			max = points.Max(point => Mathf.Abs(point.localPosition.y) * oldMax/maxY);

		if (max == 0)
			max = 1;

		if (oldMax != max)
			ScaleAllPoints(oldMax / max);

		upperBound.SetText(max.ToString());
		lowerBound.SetText((-max).ToString());

		x++;

		if (x > maxX)
			TranslateAllPoints();	
	}

	private Action DrawGraphs = new Action(() => {});

	private void FixedUpdate(){

		bodyToGraph = bodyController.bodyEditor.bodyToEdit;
		DrawGraphs();
	}

	private void DestroyPoint(int index){

		GameObject point = points[index].gameObject;
		points.RemoveAt(index);
		DestroyImmediate(point.gameObject);
	}

	private void ScalePoint(RectTransform point, float scale){

		Vector2 position = point.localPosition;
		point.localPosition = new Vector2(position.x, position.y * scale);
	}

	private Vector2Double TotalMomentum(){

		Vector2Double totalMomentum = Vector2Double.zero;

		for (int i = 0; i < bodyController.bodies.Count; i++){

			Body body = bodyController.bodies[i];

			totalMomentum += body.velocity * body.mass;
		}

		return totalMomentum;
	}

	private float TotalKineticEnergy(){

		float totalKineticEnergy = 0;

		for (int i = 0; i < bodyController.bodies.Count; i++){

			Body body = bodyController.bodies[i];

			totalKineticEnergy += (float)(Math.Pow(body.velocity.magnitude, 2) * body.mass * 0.5);
		}

		return totalKineticEnergy;
	}


	public void OnTogglesChange(){

		DrawGraphs = new Action(() => {});

		/// Magnitudes
		if (position.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.position.magnitude, Utils.ColorOfCheckmark(position));

		if (velocity.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.velocity.magnitude, Utils.ColorOfCheckmark(velocity));

		if (acceleration.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.accelerationBeforeReset.magnitude, Utils.ColorOfCheckmark(acceleration));

		// Angles
		if (positionAngle.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.position.ToDegrees(), Utils.ColorOfCheckmark(positionAngle));

		if (velocityAngle.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.velocity.ToDegrees(), Utils.ColorOfCheckmark(velocityAngle));
		
		if (accelerationAngle.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.accelerationBeforeReset.ToDegrees(), Utils.ColorOfCheckmark(accelerationAngle));

		// 2 Axis Position
		if (positionX.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.position.x, Utils.ColorOfCheckmark(positionX));

		if (positionY.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.position.y, Utils.ColorOfCheckmark(positionY));

		// 2 Axis Velocity
		if (velocityX.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.velocity.x, Utils.ColorOfCheckmark(velocityX));

		if (velocityY.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.velocity.y, Utils.ColorOfCheckmark(velocityY));

		// 2 Axis Acceleration
		if (accelerationX.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.accelerationBeforeReset.x, Utils.ColorOfCheckmark(accelerationX));

		if (accelerationY.isOn)
			DrawGraphs += () => PlotPoint((float)bodyToGraph.accelerationBeforeReset.y, Utils.ColorOfCheckmark(accelerationY));

		// 2 Axis Momentum
		if (totalMomentumX.isOn)
			DrawGraphs += () => PlotPoint((float)TotalMomentum().x, Utils.ColorOfCheckmark(totalMomentumX));

		if (totalMomentumY.isOn)
			DrawGraphs += () => PlotPoint((float)TotalMomentum().y, Utils.ColorOfCheckmark(totalMomentumY));

		// Other Stuff
		if (numberOfCollisions.isOn)
			DrawGraphs += () => PlotPoint(bodyToGraph.nCollisions, Utils.ColorOfCheckmark(numberOfCollisions));

		if (totalKineticEnergy.isOn)
			DrawGraphs += () => PlotPoint((float)TotalKineticEnergy(), Utils.ColorOfCheckmark(totalKineticEnergy));

		DestroyAllPoints();
	}

	private void OnEnable(){
		OnTogglesChange();
	}
}
