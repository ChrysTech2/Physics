using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public static class GravityMode{
	public const int Disabled = 0;
	public const int Directional = 1;
	public const int Centered = 2;
}

public static class FocusMode{
	public const int Disabled = 0;
	public const int Enabled = 1;
	public const int XAxis = 2;
	public const int YAxis = 3;
}

public static class BorderMode{
	public const int Disabled = 0;
	public const int Rectangle = 1;
	public const int Circle = 2;
}

public static class Times{
	public const int SecondsPerMinute = 60;
	public const int SecondsPerHour = 3_600;
	public const int SecondsPerDay = 86_400;
	public const int SecondsPerWeek = 604_800;
	public const int SecondsPerMonth = 2_592_000;
	public const int SecondsPerYear = 31_536_000;
}

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

	public static GameObject GetParent(TMP_InputField obj){
		return obj.transform.parent.gameObject;
	}
	public static GameObject GetParent(TMP_Dropdown obj){
		return obj.transform.parent.gameObject;
	}

	public static TMP_Text GetTextChild(TMP_InputField obj){
		return obj.transform.parent.GetChild(0).GetComponent<TMP_Text>();
	}

	public static void SetTextChild(TMP_InputField obj, string text){
		GetTextChild(obj).SetText(text);
	}

	public void ResetWorld(){
		SceneManager.LoadScene("Simulation");
	}

	public static void SetActiveIf(TMP_InputField obj, bool condition){
		GetParent(obj).SetActive(condition);
	}
	public static void SetActiveIf(TMP_Dropdown obj, bool condition){
		GetParent(obj).SetActive(condition);
	}
	public static void SetActiveIf(Toggle obj, bool condition){
		obj.gameObject.SetActive(condition);
	}

	public static Vector2Double ToVector2Double(Vector2 vector){
		return new Vector2Double(vector.x, vector.y);
	}

}
