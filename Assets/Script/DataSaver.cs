using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : MonoBehaviour{

	[SerializeField] private DataController data;

	public void SaveSimulationOnFolder(string simulationName){

		string simulationPath = data.SimulationsPath + Path.AltDirectorySeparatorChar + simulationName;

		data.settings.ApplyDataToSettings();

		Directory.CreateDirectory(simulationPath);
		SaveSettingsOnFile(simulationPath);
		SaveBodiesOnFile(simulationPath);
	}
	
	public void SaveSettingsOnFile(string simulationPath){

		List<string> settingsToStringList = new List<string>(){

			// Setttings -> Body
			$"x : {data.settings.x.text}",
			$"y : {data.settings.y.text}",
			$"velocity_x : {data.settings.velocityX.text}",
			$"velocity_y : {data.settings.velocityY.text}",

			$"mass : {data.settings.mass.text}",
			$"radius : {data.settings.radius.text}",
			$"name : {data.settings.bodyName.text}",

			$"color_red_value : {data.settings.r.value}",
			$"color_green_value : {data.settings.g.value}",
			$"color_blue_value : {data.settings.b.value}",
			$"color_opacity_value : {data.settings.a.value}",

			$"random_mode : {data.settings.randomMode.isOn}",

			// Settings -> General
			$"seconds_per_frame : {data.settings.secondsPerFrame.text}",
			$"touch_multiplier : {data.settings.touchMultiplier.text}",
			$"line_duration : {data.settings.lineDuration.text}",
			$"line_thickness : {data.settings.lineThickness.text}",
			$"show_center_of_gravity : {data.settings.showCenterOfGravity.isOn}",

			// Settings -> Parent
			$"use_parent : {data.settings.useParent.isOn}",
			$"parent_value : {data.settings.parent.value}",
			$"sum_parent_radius : {data.settings.sumParentRadius.isOn}",
			$"sum_auto_velocity : {data.settings.sumAutoVelocity.isOn}",
			$"sum_body_radius : {data.settings.sumBodyRadius.isOn}",

			// Settings -> Collisions
			$"collisions : {data.settings.calculateCollisions.isOn}",
			$"merge_bodies : {data.settings.mergeBodiesInCollisions.isOn}",
			$"coefficient_of_restitution : {data.settings.coefOfRestitution.text}",

			$"border_mode_value : {data.settings.borderMode.value}",
			$"border_x : {data.settings.borderX.text}",
			$"border_y : {data.settings.borderY.text}",

			// Settings -> Gravity
			$"attraction_gravity_constant : {data.settings.attractionGravityConstant.text}",
			$"global_gravity_mode_value : {data.settings.gravityMode.value}",
			$"global_gravity_acceleration : {data.settings.gravityAcceleration.text}",
			$"global_gravity_angle : {data.settings.gravityAngle.text}",

			// Settings -> Forces
			$"fluid_density : {data.settings.fluidDensity.text}",
			$"drag_coefficient : {data.settings.dragCoefficient.text}",
			
			// BodyController
			$"controller_scale : {data.bodyController.scale}",
			$"controller_time : {data.bodyController.t}",

			// CameraController
			$"camera_focus_mode : {data.cameraController.focus}",
			$"camera_index : {data.cameraController.index}",

			$"camera_offset_x : {data.cameraController.offset.x}",
			$"camera_offset_y : {data.cameraController.offset.y}",

			$"camera_body_x : {data.cameraController.bodyPosition.x}",
			$"camera_body_y : {data.cameraController.bodyPosition.y}",

			// BodyEditor
			$"editor_show_color : {data.bodyEditor.showColor.isOn}",
			$"editor_show_angle : {data.bodyEditor.showAngle.isOn}",

			// GraphController
			$"graph_position : {data.graphController.position.isOn}",
			$"graph_velocity : {data.graphController.velocity.isOn}",
			$"graph_acceleration : {data.graphController.acceleration.isOn}",

			$"graph_position_angle : {data.graphController.positionAngle.isOn}",
			$"graph_velocity_angle : {data.graphController.velocityAngle.isOn}",
			$"graph_acceleration_angle : {data.graphController.accelerationAngle.isOn}",

			$"graph_x : {data.graphController.positionX.isOn}",
			$"graph_y : {data.graphController.positionY.isOn}",

			$"graph_velocity_x : {data.graphController.velocityX.isOn}",
			$"graph_velocity_y : {data.graphController.velocityY.isOn}",

			$"graph_acceleration_x : {data.graphController.accelerationX.isOn}",
			$"graph_acceleration_y : {data.graphController.accelerationY.isOn}",

			$"graph_total_momentum : {data.graphController.totalMomentum.isOn}",
			$"graph_total_kinetic_energy : {data.graphController.totalKineticEnergy.isOn}",

			// Menus
			$"graph_open : {data.graphController.gameObject.activeSelf}",
		};

		SetTextValueSeparator(settingsToStringList);

		string settingsToString = string.Join(DataController.LINE_SEPARATOR, settingsToStringList);	

		string finalPath = $"{simulationPath}{Path.AltDirectorySeparatorChar}settings.{DataController.FORMAT}";

		File.WriteAllText(finalPath, settingsToString);
	}

	private void SaveBodiesOnFile(string simulationPath){

		List<string> bodiesToStringList = new List<string>(){
			$"number_of_bodies : {data.bodyController.bodies.Count}"
		};

		foreach (Body body in data.bodyController.bodies){

			List<string> currentBodyToStringList = new List<string>(){

				$"{DataController.BODIES_SEPARATOR}name : {body.name}",

				$"x : {body.position.x}",
				$"y : {body.position.y}",

				$"velocity_x : {body.velocity.x}",
				$"velocity_y : {body.velocity.y}",

				$"mass : {body.mass}",
				$"radius : {body.radius}",

				$"color_red_value : {body.color.r}",
				$"color_green_value : {body.color.g}",
				$"color_blue_value : {body.color.b}",
				$"color_opacity_value : {body.color.a}",

				$"number_of_collisions : {body.nCollisions}",
			};

			bodiesToStringList.Add(string.Join(DataController.LINE_SEPARATOR, currentBodyToStringList));
		}

		SetTextValueSeparator(bodiesToStringList);

		string bodiesToString = string.Join(DataController.LINE_SEPARATOR, bodiesToStringList);

		string finalPath = $"{simulationPath}{Path.AltDirectorySeparatorChar}bodies.{DataController.FORMAT}";

		File.WriteAllText(finalPath, bodiesToString);

	}

	private void SetTextValueSeparator(List<string> list){

		for (int i = 0; i < list.Count; i++)
			list[i] = list[i].Replace(" : ", DataController.TEXT_VALUE_SEPARATOR);

	}
}
