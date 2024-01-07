using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class BodiesTableDataController : MonoBehaviour{

	public Slider r, g, b, a;
	[SerializeField] private TMP_InputField bodyName;
	[SerializeField] private TMP_InputField mass, radius;
	[SerializeField] private BodiesTable bodiesTable;
	public TMP_Dropdown bodiesDropdown;
	public TMP_Text errorMessage;

	private string customBodiesPath;
	private List<string> bodiesNames = new List<string>();

	private const string SAVE_ALREADY_EXIST = "There is already another body with that name.";
	private const string SAVE_ERROR_MESSAGE = "That name is not valid.";
	public const string LOAD_ERROR_MESSAGE = "I could not load the selected body.";
	public const string CONFIRM_DELETE = "Click `Delete` again to confirm.";

	private void Start(){

		customBodiesPath = DataController.rootPath + Path.AltDirectorySeparatorChar + "custom_bodies." + DataController.FORMAT;

		if (!File.Exists(customBodiesPath))
			File.WriteAllText(customBodiesPath, $"number_of_custom_bodies{DataController.TEXT_VALUE_SEPARATOR}0");

		LoadBodiesInDropdown();
	}

	public void SaveButton(){

		OnBodyNameChange();

		if (errorMessage.text != "")
			return;

		SaveBody();
	}

	public void OnBodyNameChange(){

		if (!Utils.IsValidName(bodyName.text)){
			errorMessage.SetText(SAVE_ERROR_MESSAGE);
			return;
		}

		if (bodiesNames.Contains(bodyName.text)){
			errorMessage.SetText(SAVE_ALREADY_EXIST);
			return;
		}

		errorMessage.SetText("");
	}

	private void SaveBody(){

		List<string> bodiesToStringList = new List<string>(){

			$"{DataController.LINE_SEPARATOR}{DataController.GROUP_SEPARATOR}name : {bodyName.text}",

			$"mass : {mass.text}",
			$"radius : {radius.text}",

			$"color_red_value : {r.value}",
			$"color_green_value : {g.value}",
			$"color_blue_value : {b.value}",
			$"color_opacity_value : {a.value}",
		};

		DataController.SetTextValueSeparator(bodiesToStringList);

		string bodiesToString = string.Join(DataController.LINE_SEPARATOR, bodiesToStringList);

		File.AppendAllText(customBodiesPath, bodiesToString);

		AddOneToCount();
		LoadBodiesInDropdown();
	}

	public void DeleteBody(){

		if (errorMessage.text == CONFIRM_DELETE){

			errorMessage.SetText("");

			List<string> settingsList = FileContentToList(customBodiesPath).ToList();
			ExpressionEvaluator.Evaluate(DataController.RemoveVariableName(settingsList[0]), out int nBodies);

			if (nBodies == 0)
				return;

			int index = bodiesDropdown.value * 7 + 1;

			settingsList[index - 1] = settingsList[index - 1].Replace(DataController.LINE_SEPARATOR, "");

			for (int i = 0; i < 7; i++){
				settingsList.RemoveAt(index); // 7 : Number of settings per customBody
			}

			File.WriteAllText(customBodiesPath, string.Join(DataController.LINE_SEPARATOR, settingsList));
			
			SubtractOneToCount();
			LoadBodiesInDropdown();
			
			return;
		}

		errorMessage.SetText(CONFIRM_DELETE);
	}

	// Load
	private void LoadBodiesInDropdown(){

		bodiesNames.Clear();
		bodiesTable.customBodies.Clear();

		string[] settingsList = FileContentToList(customBodiesPath);
		DataController.RemoveAllVariableNames(settingsList);

		ExpressionEvaluator.Evaluate(settingsList[0], out int nBodies);

		for (int i = 0; i < nBodies; i++){

			int index =  i * 7 + 1; // 7 : Number of settings per customBody

			bodiesTable.customBodies.Add(new BodyPreset());

			BodyPreset body = bodiesTable.customBodies[i];
			
			body.name = settingsList[index];

			ExpressionEvaluator.Evaluate(settingsList[index + 1], out body.mass);
			ExpressionEvaluator.Evaluate(settingsList[index + 2], out body.radius);

			ExpressionEvaluator.Evaluate(settingsList[index + 3], out body.color.r);
			ExpressionEvaluator.Evaluate(settingsList[index + 4], out body.color.g);
			ExpressionEvaluator.Evaluate(settingsList[index + 5], out body.color.b);
			ExpressionEvaluator.Evaluate(settingsList[index + 6], out body.color.a);

			bodiesNames.Add(bodiesTable.customBodies[i].name);
		}

		bodiesDropdown.ClearOptions();
		bodiesDropdown.AddOptions(bodiesNames);		
	}

	private void AddOneToCount(){

		string[] settingsList = FileContentToList(customBodiesPath);
		ExpressionEvaluator.Evaluate(DataController.RemoveVariableName(settingsList[0]), out int nBodies);

		settingsList[0] = $"number_of_custom_bodies{DataController.TEXT_VALUE_SEPARATOR}{nBodies + 1}";
		File.WriteAllText(customBodiesPath, string.Join(DataController.LINE_SEPARATOR, settingsList));
	}
	
	private void SubtractOneToCount(){

		string[] settingsList = FileContentToList(customBodiesPath);
		ExpressionEvaluator.Evaluate(DataController.RemoveVariableName(settingsList[0]), out int nBodies);

		settingsList[0] = $"number_of_custom_bodies{DataController.TEXT_VALUE_SEPARATOR}{nBodies - 1}";
		File.WriteAllText(customBodiesPath, string.Join(DataController.LINE_SEPARATOR, settingsList));
	}

	public void ResetErrors(){
		errorMessage.SetText("");
	}

	private string[] FileContentToList(string path){
		string bodiesToString = File.ReadAllText(path);
		return bodiesToString.Split(new[] {DataController.LINE_SEPARATOR}, StringSplitOptions.None);
	}
}
