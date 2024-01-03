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

	public MaterialPreset(){
		name = "Material 1";
		density = 0;
		color = Color.white;
	}

	public double MassFromRadius(double radius){
		return density * Math.Pow(radius, 3) * 4 * Math.PI / 3;
	}
}

public class MaterialsTable : MonoBehaviour{

	[SerializeField] private SettingsController settingsController;
	[SerializeField] private MaterialsTableDataController tableDataController;
	public List<MaterialPreset> customMaterials = new List<MaterialPreset>();

	[SerializeField] private List<MaterialPreset> materials = new List<MaterialPreset>(){

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

		new MaterialPreset("Oxygen", 	1.429,  new Color(1f, 1f, 1f, 0.12f)),
		new MaterialPreset("Hydrogen",	0.08375,new Color(1f, 1f, 1f, 0.12f)),
		new MaterialPreset("Helium", 	0.178,  new Color(1f, 1f, 1f, 0.12f)),
		new MaterialPreset("Nitrogen", 	1.19,  	new Color(1f, 1f, 1f, 0.12f)),
		new MaterialPreset("Sodium", 	970,  	new Color(0.471f, 0.471f, 0.467f, 1f)),

		new MaterialPreset("Carbon", 	2200, 	new Color(0.09f, 0.078f, 0.075f, 1f)),
		new MaterialPreset("Silicon", 	2329,  	new Color(0.361f, 0.42f, 0.518f, 1f)),
		new MaterialPreset("Platinum", 	21450,  new Color(0.518f, 0.522f, 0.494f, 1f)),
		new MaterialPreset("Osmium", 	22590,  new Color(0.573f, 0.69f, 0.714f, 1f)),
		new MaterialPreset("Ceramic", 	4250,   new Color(0.698f, 0.596f, 0.569f, 1f)),

		new MaterialPreset("Lead", 		11400, 	new Color(0.282f, 0.282f, 0.29f, 1f)),
		new MaterialPreset("Sulfur", 	2000,  	new Color(0.757f, 0.663f, 0.267f, 1f)),
		new MaterialPreset("Silver", 	10490 ,  new Color(0.592f, 0.596f, 0.58f, 1f)),
		new MaterialPreset("Uranium", 	19000,  new Color(0.506f, 0.494f, 0.49f, 1f)),
		new MaterialPreset("Steel", 	8050,   new Color(0.773f, 0.788f, 0.804f, 1f)),

		new MaterialPreset("Paper", 	800, 	new Color(1f, 1f, 1f, 1f)),
		new MaterialPreset("Plastic", 	1030,  	new Color(0.945f, 0.522f, 0.51f, 1f)),
		new MaterialPreset("Glass", 	2500 ,  new Color(1f, 1f, 1f, 0.25f)),
		new MaterialPreset("Sand", 		1520,  	new Color(0.898f, 0.776f, 0.616f, 1f)),
		new MaterialPreset("Lithium", 	530,   	new Color(0.682f, 0.69f, 0.694f, 1f)),
	
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

	public void SelectCustomMaterial(){

		try{

			if (massMode){

				ExpressionEvaluator.Evaluate(settingsController.radius.text, out double radius);

				if (radius == 0){
					settingsController.radius.text = "1";
					radius = 1;
				}

				MaterialPreset material = customMaterials[tableDataController.materialsDropdown.value];

				settingsController.mass.text = Utils.FormatText(((float)material.MassFromRadius(radius)).ToString());

				settingsController.r.value = material.color.r;
				settingsController.g.value = material.color.g;
				settingsController.b.value = material.color.b;
				settingsController.a.value = material.color.a;
			}
			else{
				
				settingsController.fluidDensity.text = Utils.FormatText(customMaterials[tableDataController.materialsDropdown.value].density.ToString());
			}

			transform.parent.gameObject.SetActive(false);
		}
		catch{
			tableDataController.errorMessage.SetText(MaterialsTableDataController.LOAD_ERROR_MESSAGE);
		}
	}
}
