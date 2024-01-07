using UnityEngine;
using UnityEngine.UI;

public class EasterEggsController : MonoBehaviour{

	[SerializeField] private Transform controls;

	private Color color = Color.red;
	private bool easterEggActive = false;
	private float angle = 0;
	
	private void Update(){

		if (!easterEggActive)
			return;

		color.r = Mathf.Cos(angle);
		color.g = Mathf.Cos(angle + Mathf.PI);
		color.b = Mathf.Cos(angle + Mathf.PI/2);

		SetControlsColor();

		angle+=Time.deltaTime;
	}

	private void SetControlsColor(){

		foreach (Transform child in controls)
			child.gameObject.GetComponent<Image>().color = color;
	}

	public void ToggleEasterEgg(){

		easterEggActive = !easterEggActive;

		if (easterEggActive)
			return;

		angle = 0;
		color = new Color(1,1,1,1);
		SetControlsColor();
	}
}
