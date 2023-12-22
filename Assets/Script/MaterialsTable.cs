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

		new MaterialPreset("Gold", 	19300, 	new Color(245, 201, 22, 255) / 255),
		new MaterialPreset("Iron", 	7874,  	new Color(100, 100, 100,255) / 255),
		new MaterialPreset("Copper", 	8960,  	new Color(186, 119, 57, 255) / 255),
		new MaterialPreset("Mercury", 13500,  new Color(79, 73, 57, 255) / 255),

		new MaterialPreset("Water", 	1000,  	new Color(8, 172, 243, 128) / 255),
		new MaterialPreset("Air",		1.293,  new Color(255, 255, 255, 64) / 255),
		new MaterialPreset("Rock", 	2500,  	new Color(52, 51, 51, 255) / 255),
		new MaterialPreset("Null", 	0,  	new Color(0, 0, 0, 255) / 255),
	
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
