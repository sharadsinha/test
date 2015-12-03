using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MovableTrackball : MonoBehaviour {

	#region Private Members

	/// <summary>
	/// True if this script is currently set to rotate the object on drag.
	/// </summary>
	private bool rotationMode = true;

	/// <summary>
	/// The collider used for raycasting.
	/// </summary>
	[SerializeField]
	Collider rayCollider = null;

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
	/// The amount of scaling applied per pixel movement on pinch.
	/// </summary>
	[SerializeField]
	private float scaleFactor = 0.005f;
	
	/// <summary>
	/// The number of degrees rotated per Unity unit dragged.
	/// </summary>
	[SerializeField]
	private float rotationFactor = 30f;
	
	/// <summary>
	/// True if the touch that began the current touch interaction was on a UI element.
	/// </summary>
	private bool firstTouchOnUI = false;
	
	/// <summary>
	/// True if the user is able to pan the object around using the trackball.
	/// </summary>
	private bool ableToPan = false;

	/// <summary>
	/// True if the user is able to freely rotate the object using the trackball.
	/// </summary>
	private bool ableToFreelyRotate = false;

	/// <summary>
	/// The main touch that we will save to determine movement and touch interactions.
	/// </summary>
	private Touch savedTouch1;

	/// <summary>
	/// The secondary touch that we will save to determine pinch to zoom and two finger interactions.
	/// </summary>
	private Touch savedTouch2;

	/// <summary>
	/// The number of touches that were on the device in the last frame.
	/// </summary>
	private int previousTouchCount = 0;

	/// <summary>
	/// Boolean that is used to limit the actions that happen when transitioning from multi-finger to single finger interactions and vice-versa.
	/// </summary>
	private bool restrictSingleFingerMovement = false;
	
	#endregion

	#region Public Methods

	/// <summary>
	/// Sets the movement abilities of the trackball
	/// </summary>
	/// <param name="panningAbility">If set to <c>true</c> the user is able to pan the object around by touching on the object and dragging. Otherwise, the object is stuck in place.</param>
	/// <param name="freeRotationAbility">If set to <c>true</c> the user is able to freely rotate the object by touching off the object and dragging. Otherwise, the object rotates around the normal to the plane it's on.</param>
	public void SetMovementAbilities(bool panningAbility, bool freeRotationAbility) {
		this.ableToPan = panningAbility;
		this.ableToFreelyRotate = freeRotationAbility;
	}

	#endregion
	
	#region MonoBehaviour Methods
	
	/// <summary>
	/// Assert on serialized fields
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.rayCollider != null, "Ray Collider not set on Movabletrackball");
	}
	
	/// <summary>
	/// Set the rotation, translation, or scale based on touch interaction.
	/// </summary>
	void Update() {
		// If first touch is on the UI, don't handle any drag events.
		if (Input.touchCount >= 1 && Input.touches[0].phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(0)) {
			this.firstTouchOnUI = true;
		}

		//Reset saved touches if they are new touches
		if (Input.touchCount >= 1 && Input.touches[0].phase == TouchPhase.Began) {
			this.savedTouch1 = Input.touches[0];
		}
		if (Input.touchCount >= 2 && Input.touches[1].phase == TouchPhase.Began) {
			this.savedTouch2 = Input.touches[1];
		}

		//Reset saved touches if the touch count changes
		if (Input.touchCount != this.previousTouchCount) {
			if (Input.touchCount >= 1) {
				this.savedTouch1 = Input.touches[0];
			}
			if (Input.touchCount >= 2) {
				this.savedTouch2 = Input.touches[1];
			}
			//If we can pan we don't want the user to be able to pan it when switching from more than 1 finger to 1 finger, until they lift their one finger off, or put more on
			if (this.ableToPan && Input.touchCount == 1 && this.previousTouchCount > 1) {
				this.restrictSingleFingerMovement = true;
				this.rotationMode = true;
			} else if (this.ableToPan && Input.touchCount > 1 && this.previousTouchCount == 1) {
				this.restrictSingleFingerMovement = false;
			}
		}
	
		if (!this.firstTouchOnUI && Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(0)) { // Pan or Rotate object
			Touch touch = Input.touches[0];
			
			Vector2 p1 = this.savedTouch1.position;
			Vector2 p2 = touch.position;
			this.savedTouch1 = touch;
			
			Ray r1 = Camera.main.ScreenPointToRay(p1);
			Ray r2 = Camera.main.ScreenPointToRay(p2);
			
			// Find the location of the 2 touch points on the z = 0 plane.
			Vector3 from = r1.origin + r1.direction * (Mathf.Abs(r1.origin.z) / r1.direction.z);
			Vector3 to = r2.origin + r2.direction * (Mathf.Abs(r2.origin.z) / r2.direction.z);

			if (this.ableToPan && this.ableToFreelyRotate) {
				this.HandleSingleTouch(touch, r2, from, to, this.HandleFreeRotateTouch);
			} else if (this.ableToPan) {
				this.HandleSingleTouch(touch, r2, from, to, this.HandleRestrictedTouch);
			} else if (this.ableToFreelyRotate) {
				this.HandleFreeRotateTouch(from, to);
			} else {
				this.HandleRestrictedTouch(from, to);
			}
		} else if (!this.firstTouchOnUI && Input.touchCount > 1 && !EventSystem.current.IsPointerOverGameObject(0)) { // Scale Object
			Touch touch1 = Input.touches[0];
			Touch touch2 = Input.touches[1];

			// Find the position in the previous frame of each touch.
			Vector2 touch1PrevPos = this.savedTouch1.position;
			Vector2 touch2PrevPos = this.savedTouch2.position;
			this.savedTouch1 = touch1;
			this.savedTouch2 = touch2;
			
			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
			float touchDeltaMag = (touch1.position - touch2.position).magnitude;
			
			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;
			
			float scale = this.transform.localScale.x;
			scale = Mathf.Clamp(scale + deltaMagnitudeDiff * this.scaleFactor, this.minScale, this.maxScale);
			
			this.transform.localScale = Vector3.one * scale;
		}
		
		// Reset touch state.
		if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended) {
			this.firstTouchOnUI = false;
			this.restrictSingleFingerMovement = false;
		}
		
		this.previousTouchCount = Input.touchCount;
	}
	
	#endregion

	#region Delegates

	/// <summary>
	/// Delegate used that defines how the movable trackball will rotate the object.
	/// </summary>
	/// <param name="from">The location of the starting touch on the z = 0 plane</param>
	/// <param name="to">The location of the final touch on the z = 0 plane</param>
	private delegate void TouchRotationDelegate(Vector3 from, Vector3 to);

	#endregion

	#region Helper Methods

	/// <summary>
	/// Handles the single touch.
	/// We can pan freely, and rotate based on the given rotation handling delegate.
	/// </summary>
	/// <param name="touch">The status of the touch that is being used to control the trackball</param>
	/// <param name="r2">The ray corresponding to a Ray between where the user touched and the plane that the content is on</param>
	/// <param name="from">The location of the starting touch on the z = 0 plane</param>
	/// <param name="to">The location of the final touch on the z = 0 plane</param>
	/// <param name="handleTouchRotation">The delegate that will dicatate how to handle touch rotation.</param>
	private void HandleSingleTouch(Touch touch, Ray r2, Vector3 from, Vector3 to, TouchRotationDelegate handleTouchRotation) {
		// Check for intersection with this object
		if (touch.phase == TouchPhase.Began) {
			RaycastHit hit;
			
			// Set rotation mode based on touch down being on this object.
			if (Physics.Raycast(r2, out hit) && hit.collider == this.rayCollider) {
				this.rotationMode = false;
			} else {
				this.rotationMode = true;
			}
		}
		
		if (this.rotationMode) {
			handleTouchRotation(from, to);
		} else {
			if (!this.restrictSingleFingerMovement) {
				this.transform.localPosition = new Vector3(to.x , to.y, this.transform.localPosition.z);
			}
		}
	}
	
	/// <summary>
	/// Handles the free rotate touch.
	/// We can't pan but we can freely rotate.
	/// </summary>
	/// <param name="from">The location of the starting touch on the z = 0 plane</param>
	/// <param name="to">The location of the final touch on the z = 0 plane</param>
	private void HandleFreeRotateTouch(Vector3 from, Vector3 to) {
		Vector3 diff = new Vector3(to.y - from.y, from.x - to.x, 0f);
		this.transform.localRotation = Quaternion.Euler(diff * this.rotationFactor) * this.transform.localRotation;
	}

	/// <summary>
	/// Handles the restricted touch.
	/// We can't pan and we can't rotate freely.
	/// </summary>
	/// <param name="from">The location of the starting touch on the z = 0 plane</param>
	/// <param name="to">The location of the final touch on the z = 0 plane</param>
	private void HandleRestrictedTouch(Vector3 from, Vector3 to) {
		this.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Vector3.Angle(from, to) * Mathf.Sign(Vector3.Cross(from, to).z))) * this.transform.localRotation;
	}

	#endregion
}
