using System.Collections.Generic;
using UnityEngine;

public class WorldLineController : MonoBehaviour{

	[SerializeField] private GameObject lineToCreate;
	[SerializeField] private BodyController bodyController;
	public GameObject centerOfGravity;

	public List<WorldLine> lines = new List<WorldLine>();

	private void Update(){

		for (int i = 0; i < lines.Count; i++)
			lines[i].ApplyPosition();
		
		if (bodyController.settings.showCenterOfGravity)
			ShowCenterOfGravity();
	}

	public void CreateLine(Vector2Double start, Vector2Double end, Color color, bool delete, double deleteAfter = 0, string tag = "Line", double thickness = 0.1f, bool deleteIfZero = true){

		if (start == end)
			return;

		if (start == Vector2Double.zero && deleteIfZero)
			return;
		
		if ((delete && deleteAfter <= 0) || thickness <= 0)
			return;

		if (tag == "Line" && (bodyController.speedMultiplier == 0 || bodyController.settings.secondsPerFrame == 0))
			return;

		WorldLine line = Instantiate(lineToCreate).GetComponent<WorldLine>();

		line.position = start;
		line.tag = tag;
		
		line.bodyController = bodyController;
		line.gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = color;
		line.transform.parent = bodyController.transform;
		line.lineTransform = line.transform;

		Vector2Double difference = end - start;

		double length = difference.magnitude;
		double angle = difference.ToDegrees();

		line.height = thickness;

		line.transform.localScale = new Vector2((float)length, (float)thickness);
		line.transform.localEulerAngles = new Vector3(0, 0, (float)angle);

		line.ApplyPosition();

		if (delete)
			Destroy(line.gameObject, (float)deleteAfter);

		lines.Add(line);
	}

	public void DeleteAllLines(){

		foreach (WorldLine line in lines)
			Destroy(line.gameObject);
	}

	public void DeleteAllLines(string tag){

		foreach (WorldLine line in lines)
			if (line.tag == tag)
				Destroy(line.gameObject);
	}

	public void DrawPolygon(double radius, int nPieces, string tag = "Border"){

		double circonference = 2 * radius * Mathf.PI;

		double oneCirconferencePiece = circonference / nPieces;
		double degreesBetweenPieces = 2 * Mathf.PI / nPieces;

		Vector2Double point1 = new Vector2Double(-oneCirconferencePiece/2, -radius);

		double angle = 0;

		for (int i = 0; i < nPieces; i++){

			Vector2Double direction = Vector2Double.ToVector2Double(angle);
			Vector2Double point2 = point1 + direction * oneCirconferencePiece;
			
			CreateLine(point1, point2, Color.gray, false, 0, tag, 0.1f);
			
			angle += degreesBetweenPieces;
			point1 = point2;
		}
	}

	public void DrawRectangle(Vector2Double border, string tag = "Border"){

		CreateLine(new Vector2Double(-border.x, border.y), new Vector2Double(border.x, border.y), Color.gray, false, 0, tag, 0.1f);
		CreateLine(new Vector2Double(-border.x, -border.y), new Vector2Double(border.x, -border.y), Color.gray, false, 0, tag, 0.1f);
		CreateLine(new Vector2Double(border.x, border.y), new Vector2Double(border.x, -border.y), Color.gray, false, 0, tag, 0.1f);
		CreateLine(new Vector2Double(-border.x, border.y), new Vector2Double(-border.x, -border.y), Color.gray, false, 0, tag, 0.1f);

	}

	private void ShowCenterOfGravity(){

		Vector2Double position = Vector2Double.zero;
		double totalMass = 0;

		foreach (Body body in bodyController.bodies){

			position += body.position * body.mass;
			totalMass += body.mass;
		}

		if (totalMass != 0)
			position /= totalMass;

		centerOfGravity.transform.localPosition = (position - bodyController.cameraController.position).ToVector2() * bodyController.scale;
	}
}
