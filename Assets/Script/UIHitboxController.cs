using UnityEngine;
using UnityEngine.UI;

public class UIHitboxController : MonoBehaviour{
	
	[SerializeField] private RectTransform bodyEditorHitboxToAssign;
	[SerializeField] private RectTransform controlsHitboxToAssign;
	[SerializeField] private RectTransform addOnTouchHitboxToAssign;
	[SerializeField] private RectTransform toggleEditorHitboxToAssign;
	[SerializeField] private Canvas canvasToAssign;

	private static RectTransform bodyEditorHitbox;
	private static RectTransform controlsHitbox;
	private static RectTransform addOnTouchHitbox;
	private static RectTransform toggleEditorHitbox;
	private static CanvasScaler canvasScaler;
	private static Canvas canvas;
	
	
	private void Start(){
		bodyEditorHitbox = bodyEditorHitboxToAssign;
		controlsHitbox = controlsHitboxToAssign;
		addOnTouchHitbox = addOnTouchHitboxToAssign;
		toggleEditorHitbox = toggleEditorHitboxToAssign;
		canvas = canvasToAssign;
		canvasScaler = canvas.GetComponent<CanvasScaler>();
	}


	private static bool IsMouseOverRect(RectTransform rect){

		if (!rect.gameObject.activeInHierarchy)
			return false;

		Vector2 mousePosition = Input.mousePosition;
		Vector2 startPosition =  new Vector2(rect.position.x, rect.position.y);
		
		Vector2 scale = new Vector2(rect.sizeDelta.x * canvas.transform.localScale.x, rect.sizeDelta.y * canvas.transform.localScale.y);

		Vector2 endPosition = startPosition + scale;

		if (mousePosition.x > startPosition.x && mousePosition.x < endPosition.x)
			if (mousePosition.y > startPosition.y && mousePosition.y < endPosition.y)
				return true;

		return false;
	}

	public static bool MouseOverBodyEditor{
		get{
			return IsMouseOverRect(bodyEditorHitbox);
		}
	}

	public static bool MouseOverControls{
		get{
			return IsMouseOverRect(controlsHitbox) || IsMouseOverRect(toggleEditorHitbox);
		}
	}

	public static bool MouseOverAddOnTouchButton{
		get{
			return IsMouseOverRect(addOnTouchHitbox);
		}
	}
}
