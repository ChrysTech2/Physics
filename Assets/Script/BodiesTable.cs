using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyPreset{

	public double mass;
	public double radius;
	public string name;
	public Color color;

	public BodyPreset(string name, double mass, double radius, Color color){
		this.name = name;
		this.mass = mass;
		this.radius = radius;
		this.color = color;
	}
}

public class BodiesTable : MonoBehaviour{

	[SerializeField] private SettingsController settingsController;

	[SerializeField] private List<BodyPreset> bodies = new List<BodyPreset>() {

		new BodyPreset(
			"Earth", 5.972 * Math.Pow(10,24), 6371000,
			new Color(0, 122, 204, 255) / 255
		),

		new BodyPreset(
			"Mars", 5.972 * Math.Pow(10,24), 6371000,
			new Color(105, 105, 105, 255) / 255
		),
		
	};

	public void SelectMaterial(int materialIndex){

		BodyPreset body = bodies[materialIndex];

		settingsController.mass.text = Utils.FormatText(((float)body.mass).ToString());
		settingsController.radius.text = Utils.FormatText(((float)body.radius).ToString());

		settingsController.r.value = body.color.r;
		settingsController.g.value = body.color.g;
		settingsController.b.value = body.color.b;
		settingsController.a.value = body.color.a;

		transform.parent.gameObject.SetActive(false);
	}
}
