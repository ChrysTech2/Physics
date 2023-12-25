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

		new BodyPreset("Mercury", 	3.285 * Math.Pow(10,23), 2_439_700, 	new Color(0.31f, 0.31f, 0.31f, 1f)),
		new BodyPreset("Venus", 	4.867 * Math.Pow(10,24), 6_051_800, 	new Color(0.878f, 0.643f, 0.259f, 1f)),
		new BodyPreset("Earth", 	5.972 * Math.Pow(10,24), 6_371_000, 	new Color(0.243f, 0.714f, 0.906f, 1f)),
		new BodyPreset("Mars", 		6.417 * Math.Pow(10,23), 3_389_500, 	new Color(0.871f, 0.522f, 0.22f, 1f)),
		new BodyPreset("Jupiter", 	1.898 * Math.Pow(10,27), 69_911_000, 	new Color(0.659f, 0.533f, 0.4f, 1f)),

		new BodyPreset("Saturn", 	5.683 * Math.Pow(10,26), 58_232_000, 	new Color(0.827f, 0.761f, 0.576f, 1f)),
		new BodyPreset("Uranus", 	8.681 * Math.Pow(10,25), 25_362_000, 	new Color(0.714f, 0.89f, 0.961f, 1f)),
		new BodyPreset("Neptune",	1.024 * Math.Pow(10,26), 24_622_000, 	new Color(0.176f, 0.243f, 0.686f, 1f)),
		new BodyPreset("Pluto", 	1.309 * Math.Pow(10,22), 1_188_300, 	new Color(0.894f, 0.757f, 0.616f, 1f)),
		new BodyPreset("Ceres", 	9.100 * Math.Pow(10,20), 473_000, 		new Color(0.478f, 0.447f, 0.431f, 1f)),

		new BodyPreset("Sun", 		1.989 * Math.Pow(10,30), 696_340_000, 	new Color(1f, 1f, 1f, 1f)),
		new BodyPreset("Ganymede", 	1.482 * Math.Pow(10,23), 2_634_100, 	new Color(0.341f, 0.306f, 0.282f, 1f)),
		new BodyPreset("Europa", 	4.800 * Math.Pow(10,22), 1_560_800, 	new Color(0.576f, 0.549f, 0.522f, 1f)),
		new BodyPreset("Callisto", 	1.076 * Math.Pow(10,23), 2_410_300, 	new Color(0.259f, 0.243f, 0.208f, 1f)),
		new BodyPreset("Io", 		8.932 * Math.Pow(10,22), 1_821_600, 	new Color(0.643f, 0.525f, 0.247f, 1f)),

		new BodyPreset("Moon", 		7.348 * Math.Pow(10,22), 1_737_400, 	new Color(0.361f, 0.357f, 0.349f, 1f)),
		new BodyPreset("Vesta", 	2.589 * Math.Pow(10,20), 262_700, 		new Color(0.537f, 0.514f, 0.467f, 1f)),
		new BodyPreset("Eris", 		1.670 * Math.Pow(10,22), 1_163_000, 	new Color(0.416f, 0.384f, 0.38f, 1f)),
		new BodyPreset("Makemake", 	3.100 * Math.Pow(10,21), 715_000, 		new Color(0.588f, 0.451f, 0.424f, 1f)),
		new BodyPreset("Tethys", 	6.170 * Math.Pow(10,20), 531_000, 		new Color(0.6f, 0.6f, 0.6f, 1f)),

		new BodyPreset("Titan", 	1.345 * Math.Pow(10,23), 2_574_700, 	new Color(0.855f, 0.714f, 0.4f, 1f)),
		new BodyPreset("Enceladus", 1.081 * Math.Pow(10,20), 252_100, 		new Color(0.576f, 0.631f, 0.69f, 1f)),
		new BodyPreset("Mimas", 	3.751 * Math.Pow(10,19), 198_200, 		new Color(0.286f, 0.286f, 0.286f, 1f)),
		new BodyPreset("Dione", 	1.050 * Math.Pow(10,21), 561_400, 		new Color(0.702f, 0.702f, 0.702f, 1f)),
		new BodyPreset("Iapetus", 	1.806 * Math.Pow(10,21), 734_500, 		new Color(0.545f, 0.522f, 0.475f, 1f)),

		new BodyPreset("Neutr.Star",4.476 * Math.Pow(10,30), 11_000, 		new Color(0.812f, 0.925f, 0.922f, 1f)),
		new BodyPreset("Whit.Dwarf",1.485 * Math.Pow(10,30), 7_000_000, 	new Color(1f, 1f, 1f, 1f)),
		new BodyPreset("BlackHole", 8.155 * Math.Pow(10,36), 12_000_000_000,new Color(0.02f, 0.02f, 0.02f, 1f)),
		new BodyPreset("Human", 	85.90, 					 0.2715, 		new Color(0.58f, 0.439f, 0.349f, 1f)),
		new BodyPreset("Ball", 		0.482, 					 0.12, 			new Color(0.988f, 0.467f, 0.035f, 1f)),

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
