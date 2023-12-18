using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Settings : MonoBehaviour{

	// Global Settings
	public double secondsPerFrame = 0.02;
	
	// User Input
	[SerializeField] private TMP_InputField x, y;
	[SerializeField] private TMP_InputField velocityX, velocityY;
	[SerializeField] private TMP_InputField mass, radius, bodyName;
	[SerializeField] private Slider r, g, b, a;


	// Other Stuff
	[SerializeField] private Image circle;
	[SerializeField] private Body body;

	[SerializeField] private TMP_Text errorMessage;
	private BodyController bodyController;

	private void Start(){
		bodyController = FindObjectOfType<BodyController>();
//		errorMessage.text = "";
	}

	public void AddBody(){

		ApplyDataToBody();

		CheckForErrors();
		

		/*body.position = position;
		body.velocity = velocity;

		body.mass = mass;
		body.radius = radius;

		body.name = bodyName;*/
		
		bodyController.AddBody(body);
	}

	private void ApplyDataToBody(){

		ExpressionEvaluator.Evaluate(x.text, out body.position.x);
		ExpressionEvaluator.Evaluate(y.text, out body.position.y);

		ExpressionEvaluator.Evaluate(velocityX.text, out body.velocity.x);
		ExpressionEvaluator.Evaluate(velocityY.text, out body.velocity.y);

		ExpressionEvaluator.Evaluate(mass.text, out body.mass);
		ExpressionEvaluator.Evaluate(radius.text, out body.radius);

		body.name = bodyName.text;

		body.color = new Color(r.value, g.value, b.value, a.value);

	}

	private void CheckForErrors(){


		// validate name



	}

	public void OnColorChange(){
		circle.color = new Color(r.value, g.value, b.value, a.value);
	}

	
}
