using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GraphController : MonoBehaviour{

	[SerializeField] private BodyController bodyController;
	[SerializeField] private GameObject graphArea;
	[SerializeField] private GameObject lineToCreate;
	[SerializeField] private Toggle position, velocity, acceleration;

	private List<RectTransform> lines = new List<RectTransform>();
	private Vector2 lastPosition, lastVelocity, lastAcceleration;
	private Body bodyToGraph;


	private void DrawGraphLine(Vector2 start, Vector2 end, Color color){

		GameObject line = Instantiate(lineToCreate);

		line.transform.SetParent(graphArea.transform, true);

		line.GetComponent<Image>().color = color;

		Vector2 difference = end - start;

		float length = difference.magnitude;
		float angle = Mathf.Atan2(difference.y, difference.x) * 180/Mathf.PI;

		RectTransform rectTransform = line.GetComponent<RectTransform>();

		rectTransform.sizeDelta = new Vector2(length, 5);
		rectTransform.eulerAngles = new Vector3(0, 0, angle);

		rectTransform.localPosition = start;
		rectTransform.localScale = Vector2.one * 1f;

		lines.Add(rectTransform);
	}

	private float x = 0;

	private void FixedUpdate(){

		bodyToGraph = bodyController.bodyEditor.bodyToEdit;

		float max = lines.Max(gameObject => Mathf.Abs(gameObject.GetComponent<RectTransform>().localPosition.y));

		x++;

		if (x > 300){

			GameObject lineToDestroy = lines[0].gameObject;
			lines.RemoveAt(0);
			DestroyImmediate(lineToDestroy);
			x = 0;
		}


		PlotPosition(x);

	}

	private void PlotPosition(float x){
		DrawGraphLine(lastPosition, new Vector2(x,(float)bodyToGraph.position.x), Color.red);
	}

}
