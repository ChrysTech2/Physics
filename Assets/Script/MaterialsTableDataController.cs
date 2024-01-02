using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class MaterialsTableDataController : MonoBehaviour{

	public Slider r, g, b, a;
	[SerializeField] private TMP_InputField materialName;
	[SerializeField] private TMP_InputField density;
	[SerializeField] private MaterialsTable materialsTable;
	public TMP_Dropdown materialsDropdown;
	public TMP_Text errorMessage;

	private string customMaterialsPath;
	private List<string> materialsNames = new List<string>();

	private const string SAVE_ALREADY_EXIST = "There is already another material with that name.";
	private const string SAVE_ERROR_MESSAGE = "That name is not valid.";
	public const string LOAD_ERROR_MESSAGE = "I could not load the selected material.";

	private void Start(){

		customMaterialsPath = DataController.rootPath + Path.AltDirectorySeparatorChar + "custom_materials." + DataController.FORMAT;

		if (!File.Exists(customMaterialsPath))
			File.WriteAllText(customMaterialsPath, $"number_of_custom_materials{DataController.TEXT_VALUE_SEPARATOR}0");

		LoadMaterialsInDropdown();
	}

	public void SaveButton(){

		OnMaterialNameChange();

		if (errorMessage.text != "")
			return;

		SaveMaterial();
	}

	public void OnMaterialNameChange(){

		if (!Utils.IsValidName(materialName.text)){
			errorMessage.SetText(SAVE_ERROR_MESSAGE);
			return;
		}

		if (materialsNames.Contains(materialName.text)){
			errorMessage.SetText(SAVE_ALREADY_EXIST);
			return;
		}

		errorMessage.SetText("");
	}

	private void SaveMaterial(){

		List<string> materialsToStringList = new List<string>(){

			$"{DataController.LINE_SEPARATOR}{DataController.GROUP_SEPARATOR}name : {materialName.text}",

			$"density : {density.text}",

			$"color_red_value : {r.value}",
			$"color_green_value : {g.value}",
			$"color_blue_value : {b.value}",
			$"color_opacity_value : {a.value}",
		};

		DataController.SetTextValueSeparator(materialsToStringList);

		string materialsToString = string.Join(DataController.LINE_SEPARATOR, materialsToStringList);

		File.AppendAllText(customMaterialsPath, materialsToString);

		AddOneToCount();
		LoadMaterialsInDropdown();
	}

	public void DeleteMaterial(){

		List<string> settingsList = FileContentToList(customMaterialsPath).ToList();
		ExpressionEvaluator.Evaluate(DataController.RemoveVariableName(settingsList[0]), out int nMaterials);

		if (nMaterials == 0)
			return;

		int index = materialsDropdown.value * 6 + 1;

		settingsList[index - 1] = settingsList[index - 1].Replace(DataController.LINE_SEPARATOR, "");

		for (int i = 0; i < 6; i++){
			settingsList.RemoveAt(index); // 7 : Number of settings per customBody
		}

		File.WriteAllText(customMaterialsPath, string.Join(DataController.LINE_SEPARATOR, settingsList));
		
		SubtractOneToCount();
		LoadMaterialsInDropdown();
	}

	// Load
	private void LoadMaterialsInDropdown(){

		materialsNames.Clear();
		materialsTable.customMaterials.Clear();

		string[] settingsList = FileContentToList(customMaterialsPath);
		DataController.RemoveAllVariableNames(settingsList);

		ExpressionEvaluator.Evaluate(settingsList[0], out int nMaterials);

		for (int i = 0; i < nMaterials; i++){

			int index =  i * 7 + 1; // 7 : Number of settings per customBody

			materialsTable.customMaterials.Add(new MaterialPreset());

			MaterialPreset material = materialsTable.customMaterials[i];
			
			material.name = settingsList[index];

			ExpressionEvaluator.Evaluate(settingsList[index + 1], out material.density);

			ExpressionEvaluator.Evaluate(settingsList[index + 2], out material.color.r);
			ExpressionEvaluator.Evaluate(settingsList[index + 3], out material.color.g);
			ExpressionEvaluator.Evaluate(settingsList[index + 4], out material.color.b);
			ExpressionEvaluator.Evaluate(settingsList[index + 5], out material.color.a);

			materialsNames.Add(materialsTable.customMaterials[i].name);
		}

		materialsDropdown.ClearOptions();
		materialsDropdown.AddOptions(materialsNames);		
	}

	private void AddOneToCount(){

		string[] settingsList = FileContentToList(customMaterialsPath);
		ExpressionEvaluator.Evaluate(DataController.RemoveVariableName(settingsList[0]), out int nMaterials);

		settingsList[0] = $"number_of_custom_bodies{DataController.TEXT_VALUE_SEPARATOR}{nMaterials + 1}";
		File.WriteAllText(customMaterialsPath, string.Join(DataController.LINE_SEPARATOR, settingsList));
	}
	
	private void SubtractOneToCount(){

		string[] settingsList = FileContentToList(customMaterialsPath);
		ExpressionEvaluator.Evaluate(DataController.RemoveVariableName(settingsList[0]), out int nMaterials);

		settingsList[0] = $"number_of_custom_bodies{DataController.TEXT_VALUE_SEPARATOR}{nMaterials - 1}";
		File.WriteAllText(customMaterialsPath, string.Join(DataController.LINE_SEPARATOR, settingsList));
	}

	public void ResetErrors(){
		errorMessage.SetText("");
	}

	private string[] FileContentToList(string path){
		string materialsToString = File.ReadAllText(path);
		return materialsToString.Split(new[] {DataController.LINE_SEPARATOR}, StringSplitOptions.None);
	}

}
