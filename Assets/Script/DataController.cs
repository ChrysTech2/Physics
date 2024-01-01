using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class DataController : MonoBehaviour{

	/*
		Physics
			|_Data						// RootPath
				|_CustomBodies.txt		// customBodiesPath
				|_CustomMaterials.txt	// CustomMaterialsPath
				|_Simulations			// SimulationsPath
					|_Save 1
	*/

	public const string FORMAT = "txt", LINE_SEPARATOR = " #\n", TEXT_VALUE_SEPARATOR = " : ", BODIES_SEPARATOR = "...........................................\n\n";

	private string rootPath, simulationsPath;

	[SerializeField] private DataLoader dataLoader;
	[SerializeField] private DataSaver dataSaver;
	[SerializeField] private TMP_Dropdown simulationsDropdown, templateDropdown;
	[SerializeField] private GameObject saveMenu, loadMenu;

	public BodyController bodyController;
	public GraphController graphController;
	public CameraController cameraController;
	public SettingsController settings;
	public BodyEditor bodyEditor;

	[SerializeField] private TMP_InputField simulationName;
	[SerializeField] private TMP_Text saveErrorMessage, loadErrorMessage;
	private const string CONFIRM_OVERWRITE_MESSAGE = "A simulation with the same name already exist, click `Save` again to overwrite";
	private const string SAVE_ERROR_MESSAGE = "That name is not valid.";
	private const string LOAD_ERROR_MESSAGE = "I could not load the selected simulation.";
	private const string DELETE_ERROR_MESSAGE = "I could not delete the selected simulation.";
	
	private const string CONFIRM_DELETE_MESSAGE = "Click `Delete` again to confirm.";

	private void Start(){

		rootPath =  Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Data";
		simulationsPath = rootPath + Path.AltDirectorySeparatorChar + "Simulations";

		Directory.CreateDirectory(rootPath);
		Directory.CreateDirectory(simulationsPath);

		LoadOptionsInDropdown();
	}

	// Save Data
	public void SaveButton(){

		if (saveErrorMessage.text == CONFIRM_OVERWRITE_MESSAGE){
			dataSaver.SaveSimulationOnFolder(simulationName.text);
			saveMenu.SetActive(false);
			LoadOptionsInDropdown();
			saveErrorMessage.SetText("");
			return;
		}

		if (saveErrorMessage.text != "")
			return;

		if (!FolderAlreadyExist(simulationName.text)){
			dataSaver.SaveSimulationOnFolder(simulationName.text);
			saveMenu.SetActive(false);
			LoadOptionsInDropdown();
			return;	
		}

		saveErrorMessage.SetText(CONFIRM_OVERWRITE_MESSAGE);
	}

	private bool FolderAlreadyExist(string name){

		string[] savedSimulations = Directory.GetDirectories(simulationsPath);
		name = name.ToLower();

		foreach (string simulation in savedSimulations)
			if (Path.GetFileName(simulation).ToLower() == name)
				return true;

		return false;
	}

	public void OnSimulationNameChange(){

		if (!Utils.IsValidName(simulationName.text)){
			saveErrorMessage.SetText(SAVE_ERROR_MESSAGE);
			return;
		}

		saveErrorMessage.SetText("");
	}

	//Load Data
	public void LoadButton(){

		try{

			string selectedSimulationName = simulationsDropdown.options[simulationsDropdown.value].text;
			simulationName.text = selectedSimulationName;

			dataLoader.LoadSimulationFromFolder(selectedSimulationName);
			loadErrorMessage.SetText("");

			loadMenu.SetActive(false);
			gameObject.SetActive(false);
			settings.gameObject.SetActive(false);
			bodyEditor.gameObject.SetActive(false);
		}
		catch{

			loadErrorMessage.SetText(LOAD_ERROR_MESSAGE);
		}
	}

	private void LoadOptionsInDropdown(){

		string[] savedSimulations = Directory.GetDirectories(simulationsPath);

		for (int i = 0; i < savedSimulations.Length; i++){
			savedSimulations[i] = Path.GetFileName(savedSimulations[i]);
		}

		simulationsDropdown.ClearOptions();
		simulationsDropdown.AddOptions(savedSimulations.ToList());
	}


	// Delete Data
	public void DeleteButton(){

		try{
			string selectedSimulationName = simulationsDropdown.options[simulationsDropdown.value].text;
	
			if (loadErrorMessage.text == CONFIRM_DELETE_MESSAGE){

				dataLoader.DeleteSimulationFolder(selectedSimulationName);
				LoadOptionsInDropdown();
				loadErrorMessage.SetText("");
				return;	
			}

			loadErrorMessage.SetText(CONFIRM_DELETE_MESSAGE);
		}

		catch{
			loadErrorMessage.SetText(DELETE_ERROR_MESSAGE);
		}
	}


	// Other Stuff

	public string RootPath{
		get{
			return rootPath;
		}
	}

	public string SimulationsPath{
		get{
			return simulationsPath;
		}
	}
}
