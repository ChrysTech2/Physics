using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPreset{

	public double density;
	public string name;
	public Color color;

	public MaterialPreset(string name, double density, Color color){
		this.name = name;
		this.density = density;
		this.color = color;
	}

	public double MassFromRadius(double radius){
		return density * Math.Pow(radius, 3) * 4 * Math.PI / 3;
	}
}

public class MaterialsTable : MonoBehaviour{

	[SerializeField] private SettingsController settingsController;

	[SerializeField] private List<MaterialPreset> materials = new List<MaterialPreset>() {

		new MaterialPreset("Gold", 		19300, 	new Color(1f, 0.824f, 0.035f, 1f)),
		new MaterialPreset("Iron", 		7874,  	new Color(0.388f, 0.388f, 0.388f, 1f)),
		new MaterialPreset("Copper", 	8960,  	new Color(0.729f, 0.467f, 0.224f, 1f)),
		new MaterialPreset("Mercury", 	13500,  new Color(0.29f, 0.271f, 0.247f, 1f)),
		new MaterialPreset("Titanium", 	4506,   new Color(0.318f, 0.353f, 0.439f, 1f)),

		new MaterialPreset("Water", 	1000,  	new Color(0.031f, 0.62f, 0.949f, 0.5f)),
		new MaterialPreset("Air",		1.293,  new Color(1f, 1f, 1f, 0.12f)),
		new MaterialPreset("Rock", 		2500,  	new Color(0.251f, 0.22f, 0.22f, 1f)),
		new MaterialPreset("Null", 		0,  	new Color(0.1f, 0.1f, 0.1f, 1f)),
		new MaterialPreset("Wood", 		650,  	new Color(0.38f, 0.282f, 0.239f, 1f)),
	
	};
	
	private bool massMode = false; // if false sets fluid density

	public void SetMassMode(bool massMode){
		this.massMode = massMode;
	}

	public void SelectMaterial(int materialIndex){
		
		if (massMode){

			ExpressionEvaluator.Evaluate(settingsController.radius.text, out double radius);

			if (radius == 0){
				settingsController.radius.text = "1";
				radius = 1;
			}

			MaterialPreset material = materials[materialIndex];

			settingsController.mass.text = Utils.FormatText(((float)material.MassFromRadius(radius)).ToString());

			settingsController.r.value = material.color.r;
			settingsController.g.value = material.color.g;
			settingsController.b.value = material.color.b;
			settingsController.a.value = material.color.a;
		}
		else{
			
			settingsController.fluidDensity.text = Utils.FormatText(materials[materialIndex].density.ToString());
		}

		transform.parent.gameObject.SetActive(false);
	}
}
