using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TouchInteractionController : MonoBehaviour {
	
	/// <summary>
	/// The maximum scale this object will be scaled to by this script.
	/// </summary>
	[SerializeField]
	private float maxScale = 3f;
	
	/// <summary>
	/// The minimum scale this object will be scaled to by this script.
	/// </summary>
	[SerializeField]
	private float minScale = 0.1f;
	
	/// <summary>
	/// The factor multiplied by the amount the user pinches to determine the scale change.
	/// </summary>
	[SerializeField]
	private float scaleFactor = 0.01f;
	
	/// <summary>
	/// Listen for touches to rotate or scale this object.
	/// </summary>
	void Update () {
		if (Input.touchCount > 1) {
			Touch touch1 = Input.touches[0];
			Touch touch2 = Input.touches[1];
			
			float pinch = touch1.deltaPosition.magnitude + touch2.deltaPosition.magnitude - (touch1.deltaPosition + touch2.deltaPosition).magnitude;
			pinch *= Mathf.Sign(Vector2.Dot(touch1.position - touch2.position, touch1.deltaPosition)); 
			
			float scale = this.transform.localScale.x;
			scale = Mathf.Clamp(scale + pinch * this.scaleFactor, this.minScale, this.maxScale);
			
			this.transform.localScale = Vector3.one * scale;
		} else if (Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(0)) {
			Quaternion rot = Quaternion.Euler(0, 0, Input.touches[0].deltaPosition.x * 0.5f);
			this.transform.localRotation = this.transform.localRotation * rot;
		}
	}
}
