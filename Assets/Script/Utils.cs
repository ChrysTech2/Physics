using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

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

	public static bool IsValidName(string name){

		if (string.IsNullOrWhiteSpace(name))
			return false;

		if (name == "")
			return false;

		if (name[0] == ' ' || name[name.Length-1] == ' ')
			return false;

		char lastChar = '\0';

		foreach (char lettter in name){

			if (!(char.IsLetterOrDigit(lettter) || lettter == '_' || lettter == '-' || lettter == ' '))
				return false;

			if (lettter == ' ' && lastChar == ' ')
				return false;
			
			lastChar = lettter;
		}

		return true;
	}
}
