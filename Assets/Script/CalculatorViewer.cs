using System;
using TMPro;
using UnityEngine;

public class CalculatorViewer : MonoBehaviour{
	
	[SerializeField] private BodyController bodyController;
	[SerializeField] private TMP_InputField terminalVelocity;
	private Body bodyToView;
	private Settings settings;

	private void Start(){
		settings = bodyController.settings;
	}

	private void FixedUpdate(){

		bodyToView = bodyController.bodyEditor.bodyToEdit;

		if (settings.fluidDensity != 0 && settings.dragCoefficient != 0){

			terminalVelocity.text = ((float)TerminalVelocity()).ToString() + " m/s";


		}
		else{
			terminalVelocity.text = "No Limit";
		}

	}

	private double TerminalVelocity(){
		return Math.Sqrt(Math.Abs(bodyToView.mass * settings.gravityAcceleration) * (1 - settings.fluidDensity/bodyToView.density) / (bodyToView.area * settings.fluidDensity * settings.dragCoefficient));
	}
}
