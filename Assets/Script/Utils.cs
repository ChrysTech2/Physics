using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Utils : MonoBehaviour{
	
	public static void SetActiveTrue(GameObject obj){
		obj.SetActive(true);
	}

	public static void SetActiveFalse(GameObject obj){
		obj.SetActive(false);
	}

	public static void ToggleActive(GameObject obj){
		obj.SetActive(!obj.activeSelf);
	}

	public static string FormatText(string str){

		str = str.Replace(",", ".");
		str = str.Replace("E", "e");
		str = str.Replace("+", "");

		return str;
	}

	public static void EvaluateInputField(TMP_InputField inputField){

		ExpressionEvaluator.Evaluate(inputField.text, out float value);
		inputField.text = FormatText(value.ToString());
	}
	// bool validate string (string str) {}

	


}
