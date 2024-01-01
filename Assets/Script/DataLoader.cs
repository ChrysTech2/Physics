using System.IO;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class DataLoader : MonoBehaviour{
	
   	[SerializeField] private DataController data;

	public void LoadSimulationFromFolder(string simulationName){

		string simulationPath = data.SimulationsPath + Path.AltDirectorySeparatorChar + simulationName;

		string finalBodiesPath = $"{simulationPath}{Path.AltDirectorySeparatorChar}bodies.{DataController.FORMAT}";
		string finalSettingsPath = $"{simulationPath}{Path.AltDirectorySeparatorChar}settings.{DataController.FORMAT}";
		
		string bodiesToString = File.ReadAllText(finalBodiesPath);
		string settingsToString = File.ReadAllText(finalSettingsPath);
		
		LoadBodiesFromString(bodiesToString);
		LoadSettingsFromString(settingsToString);
		
		data.settings.ApplyDataToSettings();
	}

	public void LoadSimulationFromTextFiles(TextAsset settings, TextAsset bodies){
		
		string bodiesToString = bodies.text;
		string settingsToString = settings.text;
		
		LoadBodiesFromString(bodiesToString);
		LoadSettingsFromString(settingsToString);
		
		data.settings.ApplyDataToSettings();
	}

	public void DeleteSimulationFolder(string simulationName){

		string simulationPath = data.SimulationsPath + Path.AltDirectorySeparatorChar + simulationName;
		Directory.Delete(simulationPath, true);
	}

	private string[] settingsList;
	private int i = 0;
	private void LoadSettingsFromString(string settingsToString){

		i = 0;

		settingsList = settingsToString.Split(new[] {DataController.LINE_SEPARATOR}, StringSplitOptions.None);

		for (int i = 0; i < settingsList.Length; i++){
			settingsList[i] = RemoveVariableName(settingsList[i]);
		}

		data.bodyController.touchControl.addOnTouch.isOn = false;

		// Settings -> Body
		SetValue(data.settings.x);
		SetValue(data.settings.y);

		SetValue(data.settings.velocityX);
		SetValue(data.settings.velocityY);

		SetValue(data.settings.mass);
		SetValue(data.settings.radius);
		SetValue(data.settings.bodyName);

		SetValue(data.settings.r);
		SetValue(data.settings.g);
		SetValue(data.settings.b);
		SetValue(data.settings.a);

		SetValue(data.settings.randomMode);

		// Settings -> General
		SetValue(data.settings.secondsPerFrame);
		SetValue(data.settings.touchMultiplier);
		SetValue(data.settings.lineDuration);
		SetValue(data.settings.lineThickness);
		SetValue(data.settings.showCenterOfGravity);

		// Settings -> Parent
		SetValue(data.settings.useParent);
		SetValue(data.settings.parent);
		SetValue(data.settings.sumParentRadius);
		SetValue(data.settings.sumAutoVelocity);
		SetValue(data.settings.sumBodyRadius);

		// Settings -> Collisions
		SetValue(data.settings.calculateCollisions);
		SetValue(data.settings.mergeBodiesInCollisions);
		SetValue(data.settings.coefOfRestitution);

		SetValue(data.settings.borderMode);
		SetValue(data.settings.borderX);
		SetValue(data.settings.borderY);

		// Settings -> Gravity
		SetValue(data.settings.attractionGravityConstant);
		SetValue(data.settings.gravityMode);
		SetValue(data.settings.gravityAcceleration);
		SetValue(data.settings.gravityAngle);

		// Settings -> Forces
		SetValue(data.settings.fluidDensity);
		SetValue(data.settings.dragCoefficient);

		// BodyController
		ExpressionEvaluator.Evaluate(settingsList[i], out float scale); 
		data.bodyController.scale = scale;
		i++;

		SetValue(ref data.bodyController.t);

		// CameraController
		SetValue(ref data.cameraController.focus);
		SetValue(ref data.cameraController.index);

		SetValue(ref data.cameraController.offset.x);
		SetValue(ref data.cameraController.offset.y);

		SetValue(ref data.cameraController.bodyPosition.x);
		SetValue(ref data.cameraController.bodyPosition.y);

		// BodyEditor
		SetValue(data.bodyEditor.showColor);
		SetValue(data.bodyEditor.showAngle);

		// GraphController
		SetValue(data.graphController.position);
		SetValue(data.graphController.velocity);
		SetValue(data.graphController.acceleration);

		SetValue(data.graphController.positionAngle);
		SetValue(data.graphController.velocityAngle);
		SetValue(data.graphController.accelerationAngle);
		
		SetValue(data.graphController.positionX);
		SetValue(data.graphController.positionY);

		SetValue(data.graphController.velocityX);
		SetValue(data.graphController.velocityY);

		SetValue(data.graphController.accelerationX);
		SetValue(data.graphController.accelerationY);

		SetValue(data.graphController.totalMomentum);
		SetValue(data.graphController.totalKineticEnergy);

		bool graphActive = true;

		SetValue(ref graphActive);
		
		data.graphController.gameObject.SetActive(graphActive);
		data.graphController.hideButton.SetActive(graphActive);
		data.graphController.showButton.SetActive(!graphActive);
	}

	private void LoadBodiesFromString(string settingsToString){

		while(data.bodyController.bodies.Count > 0){

			data.bodyController.DeleteBody(data.bodyController.bodies[0]);
		}

		settingsList = settingsToString.Split(new[] {DataController.LINE_SEPARATOR}, StringSplitOptions.None);

		for (int i = 0; i < settingsList.Length; i++){
			settingsList[i] = RemoveVariableName(settingsList[i]);
		}

		ExpressionEvaluator.Evaluate(settingsList[0], out int nBodies);
		Body bodyToCreate = data.settings.bodyToCreate;

		for (int i = 0; i < nBodies; i++){

			int index = i * 12 + 1;

			bodyToCreate.name = settingsList[index];

			ExpressionEvaluator.Evaluate(settingsList[index + 1], out bodyToCreate.position.x);
			ExpressionEvaluator.Evaluate(settingsList[index + 2], out bodyToCreate.position.y);

			ExpressionEvaluator.Evaluate(settingsList[index + 3], out bodyToCreate.velocity.x);
			ExpressionEvaluator.Evaluate(settingsList[index + 4], out bodyToCreate.velocity.y);

			ExpressionEvaluator.Evaluate(settingsList[index + 5], out bodyToCreate.mass);
			ExpressionEvaluator.Evaluate(settingsList[index + 6], out bodyToCreate.radius);

			ExpressionEvaluator.Evaluate(settingsList[index + 7], out bodyToCreate.color.r);
			ExpressionEvaluator.Evaluate(settingsList[index + 8], out bodyToCreate.color.g);
			ExpressionEvaluator.Evaluate(settingsList[index + 9], out bodyToCreate.color.b);
			ExpressionEvaluator.Evaluate(settingsList[index + 10], out bodyToCreate.color.a);

			ExpressionEvaluator.Evaluate(settingsList[index + 11], out bodyToCreate.nCollisions);

			data.bodyController.AddBody();

		}

	}

	private string RemoveVariableName(string str){
		return str.Substring(str.IndexOf(DataController.TEXT_VALUE_SEPARATOR) + DataController.TEXT_VALUE_SEPARATOR.Length);
	}

	private void SetValue(TMP_InputField field){

		field.text = settingsList[i];
		i++;
	}

	private void SetValue(Slider slider){

		ExpressionEvaluator.Evaluate(settingsList[i], out float value);
		slider.value = value;
		i++;
	}

	private void SetValue(Toggle toggle){

		bool.TryParse(settingsList[i], out bool value);
		toggle.isOn = value;
		i++;
	}

	private void SetValue(TMP_Dropdown dropdown){

		ExpressionEvaluator.Evaluate(settingsList[i], out int value);
		dropdown.value = value;
		i++;
	}

	private void SetValue(ref float value){
		ExpressionEvaluator.Evaluate(settingsList[i], out value);
		i++;
	}

	private void SetValue(ref int value){
		ExpressionEvaluator.Evaluate(settingsList[i], out value);
		i++;
	}

	private void SetValue(ref double value){
		ExpressionEvaluator.Evaluate(settingsList[i], out value);
		i++;
	}

	private void SetValue(ref bool value){
		bool.TryParse(settingsList[i], out value);
		i++;
	}

}
