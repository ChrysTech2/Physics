using UnityEngine;
using UnityEngine.UI;

public class OnChangeEvent : MonoBehaviour{
	
	[SerializeField] private SettingsController settings;
	[SerializeField] private BodyEditor bodyEditor;
	[SerializeField] private Image colorPreview;

	public void OnColorChange(){
		colorPreview.color = new Color(settings.r.value, settings.g.value, settings.b.value, settings.a.value);
	}

	// Events
	public void OnCollisionsSettingsChange(){

		Utils.SetActiveIf(settings.mergeBodiesInCollisions, settings.calculateCollisions.isOn);
		Utils.SetActiveIf(settings.coefOfRestitution, (settings.calculateCollisions.isOn && !settings.mergeBodiesInCollisions.isOn) || settings.borderMode.value != BorderMode.Disabled);

		Utils.SetActiveIf(settings.borderX, settings.borderMode.value != BorderMode.Disabled);
		Utils.SetActiveIf(settings.borderY, settings.borderMode.value == BorderMode.Rectangle);

		if (settings.borderMode.value == BorderMode.Rectangle)
			Utils.SetTextChild(settings.borderX, "Border X");
		else
			Utils.SetTextChild(settings.borderX, "Radius");
		
	}

	public void OnGravitySettingsChange(){

		Utils.SetActiveIf(settings.gravityAcceleration, settings.gravityMode.value != GravityMode.Disabled);
		Utils.SetActiveIf(settings.gravityAngle, settings.gravityMode.value != GravityMode.Disabled && settings.gravityMode.value != GravityMode.Centered);
	}

	public void OnParentSettingsChange(){

		Utils.SetActiveIf(settings.sumParentRadius, settings.useParent.isOn && settings.parent.value != 0);
		Utils.SetActiveIf(settings.sumBodyRadius, settings.useParent.isOn && settings.parent.value != 0);
		Utils.SetActiveIf(settings.sumAutoVelocity, settings.useParent.isOn);
		Utils.SetActiveIf(settings.parent, settings.useParent.isOn);
	}

	public void OnForcesSettingsChange(){

		Utils.SetActiveIf(settings.dragCoefficient, settings.fluidDensity.text != "0");
	}

	public void OnRandomChange(){
		if (settings.randomMode.isOn){
			
			Utils.SetTextChild(settings.x, "Max X");
			Utils.SetTextChild(settings.y, "Max Y");

			Utils.SetTextChild(settings.velocityX, "Max Velocity X");
			Utils.SetTextChild(settings.velocityY, "Max Velocity Y");
		}
		else{

			Utils.SetTextChild(settings.x, "X");
			Utils.SetTextChild(settings.y, "Y");

			Utils.SetTextChild(settings.velocityX, "Velocity X");
			Utils.SetTextChild(settings.velocityY, "Velocity Y");
		}
	}

	public void OnGeneralSettingsChange(){

		if (settings.lineDuration.text == "0")
			settings.bodyController.lineController.DeleteAllLines("Line");

		settings.settings.lineDuration = 0;
		

	}
}
