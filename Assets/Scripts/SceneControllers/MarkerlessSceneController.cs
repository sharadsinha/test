using UnityEngine;
using System.Collections;

public class MarkerlessSceneController : SceneMonobehaviour {

	#region Private Members

	/// <summary>
	/// Reference to the root for the AR Content
	/// </summary>
	[SerializeField]
	private GameObject contentRoot = null;
	
	/// <summary>
	/// Reference to the AR Manager for this scene.
	/// </summary>
	private MarkerlessHybridARManager arManager = null;
	
	/// <summary>
	/// The renderer used to display the keyframe.
	/// </summary>
	[SerializeField]
	private MeshRenderer keyframeDisplay = null;
	
	/// <summary>
	/// The UI root of the viewfinding square.
	/// </summary>
	[SerializeField]
	private GameObject squareUIRoot = null;
	
	/// <summary>
	/// The start button used to take the keyframe for markerless.
	/// </summary>
	[SerializeField]
	private SlideAnimation startButton = null;
	
	/// <summary>
	/// The tracking buttons to be shown after initialization.
	/// </summary>
	[SerializeField]
	private SlideAnimation trackingButtons = null;
	
	/// <summary>
	/// The pre-init instructions to be shown after keyframe is taken.
	/// </summary>
	[SerializeField]
	private SlideAnimation preInitInstructions = null;
	
	/// <summary>
	/// Reference to the device camera display.
	/// </summary>
	private DeviceCameraDisplay cameraDisplay = null;
	
	/// <summary>
	/// A copy of the texture used as the last keyframe.
	/// </summary>
	private Texture2D keyframe = null;
	
	/// <summary>
	/// The memento this scene was opened with.
	/// </summary>
	private Memento memento = null;
	
	#endregion

	#region SceneMonobehaviour Methods

	/// <summary>
	/// Assert on serialized fields and subscribe to events.
	/// </summary>
	/// <param name="data">The memento object to display on this scene.</param>
	public override void OnViewCreate(object data = null) {
		DebugUtils.Assert(this.contentRoot != null, "Content Root not set on MarkerlessSceneController");
		DebugUtils.Assert(this.keyframeDisplay != null, "Keyframe Display not set on MarkerlessSceneController");
		DebugUtils.Assert(this.squareUIRoot != null, "Square UI Root not set on MarkerlessSceneController");
		DebugUtils.Assert(this.startButton != null, "Start Button not set on MarkerlessSceneController");
		DebugUtils.Assert(this.trackingButtons != null, "Tracking Buttons not set on MarkerlessSceneController");
		
		DebugUtils.Assert(data != null && typeof(Memento) == data.GetType(), "You must pass a Memento to the MarkerlessSceneController");

		this.contentRoot.SetActive(false);
		
		this.memento = (Memento)data;
		GameObject loadedContent = GameObject.Instantiate(Resources.Load<GameObject>(this.memento.PrefabName));
		if (loadedContent == null) {
			DebugUtils.LogError("Content ID: " + this.memento.PrefabName + " is not in app.");
		} else {
			loadedContent.transform.SetParent(this.contentRoot.transform, false);
			loadedContent.transform.localPosition = Vector3.zero;
			MovableTrackball[] contentTrackballs = loadedContent.GetComponentsInChildren<MovableTrackball>(true);
			if (contentTrackballs.Length > 0) {
				contentTrackballs[0].SetMovementAbilities(false, false);
			}
		}
	
		this.arManager = this.GetComponentInChildren<MarkerlessHybridARManager>();
		DebugUtils.Assert(this.arManager != null, "Couldn't find a child MarkerlessHybridARManager for MarkerlessSceneController");
		this.arManager.trackingStateUpdateEvent += this.TrackingStateUpdated;
		
		this.cameraDisplay = this.GetComponentInChildren<DeviceCameraDisplay>();
		DebugUtils.Assert(this.cameraDisplay != null, "Couldn't find a child DeviceCameraDisplay for MarkerlessSceneController");

		ServiceLocator.Get<TopBarController>().SetLeftButton(null, 0f, TopBarController.BackButtonLoadingScreenAction);
		
		if (ServiceLocator.Get<NavigationSceneManager>().LastScene.SceneName == Constants.MEMENTOS_SCENE_NAME) {
			ServiceLocator.Get<TopBarController>().SetTitleWithKey(Constants.SCAN_TITLE_KEY, 0f);
			this.arManager.ResetTrackingJob();
		} else {
			this.Begin();
		}
		
		this.arManager.CacheController();
	}

	/// <summary>
	/// Unsubscribe from events.
	/// </summary>
	public override void OnViewRemove() {
		this.arManager.trackingStateUpdateEvent -= this.TrackingStateUpdated;
	}
	
	#endregion
	
	#region SceneMonobehaviour
	
	/// <summary>
	/// Make sure to shut down the AR manager properly before exit.
	/// </summary>
	public override IEnumerator OnViewHide() {
		if (this.arManager != null) {
			this.arManager.StopHomography();
			
			while (!this.arManager.IsSafeToShutDown) {
				yield return null;
			}
			this.arManager = null;
		}
	}
	
	#endregion
	
	#region Callback Methods
	
	/// <summary>
	/// Called by the start button. Begins the AR Manager tracking and records the keyframe.
	/// </summary>
	public void Begin() {
		this.arManager.StartHomography();
		
		WebCamTexture frame = (WebCamTexture)this.cameraDisplay.GetComponentInChildren<Renderer>().material.mainTexture;
		this.keyframe = new Texture2D(frame.width, frame.height);
		this.keyframe.SetPixels32(frame.GetPixels32());
		this.keyframe.Apply();
		
		this.keyframeDisplay.material.mainTexture = this.keyframe;
		
		this.startButton.SetVisible(false);
		this.trackingButtons.SetVisible(true);
		ServiceLocator.Get<TopBarController>().SetTitleWithKey(Constants.VIEW_TITLE_KEY);
	}
	
	/// <summary>
	/// Called when the more info button is pressed.
	/// </summary>
	public void MoreInfoButtonPressed() {
		ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.WIKI_SCENE_NAME, Constants.LOADING_SCENE_NAME, this.memento);
	}
	
	/// <summary>
	/// Called when the selfie button is pressed.
	/// </summary>
	public void SelfieButtonPressed() {
		ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.SELFIE_SCENE_NAME, Constants.LOADING_SCENE_NAME, this.memento);
	}
	
	/// <summary>
	/// Callback for when the AR tracking state has updated.
	/// </summary>
	/// <param name="isTracking">The new tracking state. True if we are currently tracking the camera motion.</param>
	private void TrackingStateUpdated(bool isTracking) {
		this.contentRoot.SetActive(this.memento != null && this.arManager.IsInitialized());
		this.squareUIRoot.SetActive(!isTracking);
		this.trackingButtons.SetVisible(this.arManager.IsInitialized());
		this.preInitInstructions.SetVisible(isTracking && !this.arManager.IsInitialized());
	}
	
	#endregion
}
