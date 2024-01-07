using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public static class GravityMode{
	public const int Disabled = 0;
	public const int Directional = 1;
	public const int Centered = 2;
	public const int Velocity = 3;
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

	// Other Stuff
	
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

		string toEvaluate = inputField.text.Replace("K", "*(1000)").Replace("k", "*(1000)");
		toEvaluate = toEvaluate.Replace("M", "*(1000000)").Replace("G", "*(1000000000)");
		toEvaluate = toEvaluate.Replace("AU", "*(1.496e11)").Replace("au", "*(1.496e11)");

		ExpressionEvaluator.Evaluate(toEvaluate, out float value);

		inputField.text = FormatText(value.ToString());
	}

	public static bool IsValidName(string name){

		if (name.Contains(DataController.LINE_SEPARATOR) || name.Contains(DataController.TEXT_VALUE_SEPARATOR))
			return false;

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

	public static TMP_Text GetTextChild(TMP_InputField obj, int textIndex = 0){
		return obj.transform.parent.GetChild(textIndex).GetComponent<TMP_Text>();
	}

	public static void SetTextChild(TMP_InputField obj, string text, int textIndex = 0){
		GetTextChild(obj, textIndex).SetText(text);
	}

	public static void ResetWorld(){
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

	public static Color ColorOfCheckmark(Toggle toggle){
		return toggle.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>().color;
	}

	public static void Quit(){
		Application.Quit();
	}

	public static void OpenInstagram(){
		Application.OpenURL("https://www.instagram.com/chrys_tech/");
	}

	public static string FormatTime(double t){

		double time;
		string timeUnit;

		if (t < Times.SecondsPerMinute){
			time = t;
			timeUnit = "seconds";
		}
		else if (t < Times.SecondsPerHour){
			time = t / Times.SecondsPerMinute;
			timeUnit = "minutes";
		}
		else if (t < Times.SecondsPerDay){
			time = t / Times.SecondsPerHour;
			timeUnit = "hours";
		}
		else if (t < Times.SecondsPerWeek){
			time = t / Times.SecondsPerDay;
			timeUnit = "days";
		}
		else if (t < Times.SecondsPerMonth){
			time = t / Times.SecondsPerWeek;
			timeUnit = "weeks";
		}
		else if (t < Times.SecondsPerYear){
			time = t / Times.SecondsPerMonth;
			timeUnit = "months";
		}
		else{
			time = t / Times.SecondsPerYear;
			timeUnit = "years";
		}

		return $"Time ({timeUnit}): {(float)time}";

	}
}

