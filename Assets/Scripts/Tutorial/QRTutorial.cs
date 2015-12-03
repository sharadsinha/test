using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class QRTutorial : MonoBehaviour {

	#region Private Members

	/// <summary>
	/// Parent GameObject for the first part of the QR Scene tutorial.
	/// </summary>
	[SerializeField]
	private GameObject part1 = null;
	
	/// <summary>
	/// Parent GameObject for the second part of the QR Scene tutorial.
	/// </summary>
	[SerializeField]
	private GameObject part2 = null;
	
	/// <summary>
	/// Parent GameObject for the third part of the QR Scene tutorial.
	/// </summary>
	[SerializeField]
	private GameObject part3 = null;
	
	/// <summary>
	/// A reference to the AR manager used to detect tracking state.
	/// </summary>
	[SerializeField]
	private TrackingARManager arManager = null;

	#endregion
	
	#region MonoBehaviour

	/// <summary>
	/// Assert on serialized fields and set active state for first part of tutorial.
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.part1 != null, "Part 1 not set on QRTutorial");
		DebugUtils.Assert(this.part2 != null, "Part 2 not set on QRTutorial");
		DebugUtils.Assert(this.part3 != null, "Part 3 not set on QRTutorial");
		DebugUtils.Assert(this.arManager != null, "AR Manager not set on QRTutorial");
	
		if (!System.Convert.ToBoolean(XMGSaveLoadUtils.Instance.LoadString(Constants.TUTORIAL_1_KEY, System.Boolean.FalseString))) {
			this.part1.SetActive(true);
			this.arManager.trackingStateUpdateEvent += this.TrackingStateUpdated;
			XMGSaveLoadUtils.Instance.SaveString(Constants.TUTORIAL_1_KEY, System.Boolean.TrueString);
		}
	}
	
	/// <summary>
	/// Unsubscribe from event.
	/// </summary>
	void OnDestroy() {
		this.arManager.trackingStateUpdateEvent += this.TrackingStateUpdated;
	}
	
	/// <summary>
	/// Detect touch up to switch from second to third part of tutorial.
	/// </summary>
	void Update() {
		if (this.part2.activeInHierarchy) {
			if (Input.touchCount >= 1 && Input.touches[0].phase == TouchPhase.Ended && !EventSystem.current.IsPointerOverGameObject(0)) {
				this.part2.SetActive(false);
				this.part3.SetActive(true);
			}
		}
	}
	
	#endregion
	
	#region Event Handlers
	
	/// <summary>
	/// Detect beginning of tracking to switch from first to second part of tutorial.
	/// </summary>
	/// <param name="isTracking">If set to <c>true</c> is tracking.</param>
	private void TrackingStateUpdated(bool isTracking) {
		if (isTracking) {
			this.part1.SetActive(false);
			this.part2.SetActive(true);
			this.arManager.trackingStateUpdateEvent -= this.TrackingStateUpdated;
		}
	}
	
	#endregion
}
