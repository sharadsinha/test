using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QRSceneController : SceneMonobehaviour {
	
	#region Constant Members
	
	/// <summary>
	/// Our QR code must begin with this app key so we know it's a code our app is made to read.
	/// </summary>
	private const string QR_APP_KEY = "GlobaliveXMG";
	
	#endregion
	
	#region Private Members
	
	/// <summary>
	/// Reference to the AR Manager for this scene.
	/// </summary>
	[SerializeField]
	private QRCodeHybridARManager arManager = null;
	
	/// <summary>
	/// Reference to the content root for this scene.
	/// </summary>
	[SerializeField]
	private GameObject contentRoot = null;
	
	/// <summary>
	/// Reference to the UI to be shown when content is loading.
	/// </summary>
	[SerializeField]
	private GameObject loadingUI = null;
	
	/// <summary>
	/// The user instructions at the top of the screen.
	/// </summary>
	[SerializeField]
	private RecursiveFader topInstructions = null;
	
	/// <summary>
	/// The text field used to instruct the user.
	/// </summary>
	[SerializeField]
	private LocalizedText instructionText = null;
	
	/// <summary>
	/// The square showing the user where to place the QR code on screen.
	/// </summary>
	[SerializeField]
	private GameObject QRSquare = null;
	
	/// <summary>
	/// The text box that shows a warning if the user moves the device too much.
	/// </summary>
	[SerializeField]
	private RecursiveFader movementWarning = null;
	
	/// <summary>
	/// Reference to the buttons to be shown when we are tracking a QR code.
	/// </summary>
	[SerializeField]
	private SlideAnimation trackingButtons = null;
	
	/// <summary>
	/// Reference to the loaded AR content.
	/// </summary>
	private GameObject loadedContent = null;

	/// <summary>
	/// A list of movement magnitudes over the last few frames.
	/// </summary>
	private List<float> movementList = new List<float>();
	
	/// <summary>
	/// The exhibit this scene is currently displaying.
	/// </summary>
	private Exhibit exhibit = null;
	
	#endregion
	
	#region SceneMonobehaviour Methods
	
	/// <summary>
	/// Assert on serialized fields and subscribe to events.
	/// </summary>
	/// <param name="data">Any additional data required for initializing the scene.</param>
	public override void OnViewCreate(object data = null) {
		DebugUtils.Assert(this.contentRoot != null, "Content Root not set on QRSceneController");
		DebugUtils.Assert(this.arManager != null, "AR Manager not set on QRSceneController");
		DebugUtils.Assert(this.loadingUI != null, "Loading UI not set on QRSceneController");
		DebugUtils.Assert(this.instructionText != null, "Instruction Text not set on QRSceneController");
		DebugUtils.Assert(this.topInstructions != null, "Top Instructions not set on QRSceneController");
		DebugUtils.Assert(this.QRSquare != null, "QR Square not set on QRSceneController");
		DebugUtils.Assert(this.movementWarning != null, "Movement Warning not set on QRSceneController");
		DebugUtils.Assert(this.trackingButtons != null, "Tracking Buttons not set on QRSceneController");
		
		this.loadingUI.SetActive(false);
		this.QRSquare.SetActive(true);

		this.arManager.trackingStateUpdateEvent += this.TrackingStateUpdated;

		this.instructionText.SetTextKey(Constants.UNDETECTED_INSTRUCTIONS_KEY);
		ServiceLocator.Get<TopBarController>().SetTitleWithKey(Constants.SCAN_TITLE_KEY, 0f);
		ServiceLocator.Get<TopBarController>().SetLeftButton(null, 0f, TopBarController.BackButtonLoadingScreenAction);
		
		if (ServiceLocator.Get<NavigationSceneManager>().LastScene.SceneName == Constants.MAIN_MENU_SCENE_NAME) {
			this.arManager.ResetTrackingJob();
		} else if (!string.IsNullOrEmpty(this.GetTrackedQRContentID())) {
			this.InitExhibit();
		}
		
		this.arManager.CacheController();
	}
	
	/// <summary>
	/// Begin tracking.
	/// </summary>
	public override IEnumerator OnViewDisplay() {
		this.arManager.StartHomography();
		yield return null;
	}
	
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
	
	/// <summary>
	/// Unsubscribe from events.
	/// </summary>
	public override void OnViewRemove() {
		this.arManager.trackingStateUpdateEvent -= this.TrackingStateUpdated;
	}
	
	#endregion
	
	#region Monobehaviour Methods
	
	/// <summary>
	/// Check sensors for excessive movement.
	/// </summary>
	void Update() {
		// Calculate how much the device is moving.
		float movement = (Input.acceleration - Input.gyro.gravity).magnitude + Input.gyro.userAcceleration.magnitude;
		this.movementList.Add(movement);
		
		const int MOVEMENT_CAP = 10;
		if (this.movementList.Count > MOVEMENT_CAP) {
			this.movementList.RemoveAt(0);
		}
		
		// Find the average movement over the last few frames.
		float avg = 0;
		this.movementList.ForEach(f => avg += f);
		avg /= this.movementList.Count;
		
		// Show notice if we're moving too much.
		const float MOVEMENT_THRESHOLD = 0.3f;
		if (avg > MOVEMENT_THRESHOLD) {
			this.movementWarning.SetVisiblity(true);
		} else {
			this.movementWarning.SetVisiblity(false);
		}
	}
	
	#endregion
	
	#region Callback Methods
	
	/// <summary>
	/// Called when the more info button is pressed.
	/// </summary>
	public void MoreInfoButtonPressed() {
		// TODO: Send the QR loaded memento to the following scene.
		ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.WIKI_SCENE_NAME, Constants.LOADING_SCENE_NAME, this.exhibit);
	}
	
	/// <summary>
	/// Called when the selfie button is pressed.
	/// </summary>
	public void SelfieButtonPressed() {
		// TODO: Send the QR loaded memento to the following scene.
		ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.SELFIE_SCENE_NAME, Constants.LOADING_SCENE_NAME, this.exhibit);
	}
	
	/// <summary>
	/// Callback for when the AR tracking state may have been updated.
	/// </summary>
	/// <param name="isTracking">The current tracking state. True if we are currently tracking the camera motion.</param>
	private void TrackingStateUpdated(bool isTracking) {
		if (this.exhibit == null && isTracking) {
			this.InitExhibit(); //Load in the exhibit when we see a code
		} else if (this.exhibit != null && !isTracking) {
			this.instructionText.SetTextKey(Constants.LOST_TRACKING_INSTRUCTIONS_KEY);
		}

		//If we see an invalid code at any point, inform the user about it
		if (!string.IsNullOrEmpty(this.arManager.GetLastDecodedQRCode()) && !this.WasLastQRCodeValid()) {
			this.instructionText.SetTextKey(Constants.INVALID_QR_CODE_INSTRUCTIONS_KEY, new List<string>(){Debug.isDebugBuild ? "\n" + this.arManager.GetLastDecodedQRCode() : ""});
		}

		//Reset tracking if we see a new valid QR code that we want to start tracking instead.
		if (this.WasLastQRCodeValid() && (this.exhibit == null || this.GetLastQRContentID() != this.exhibit.ID)) {
			this.Reset();
		}

		this.QRSquare.SetActive(!isTracking || !this.WasLastQRCodeValid());
		this.topInstructions.SetVisiblity(!isTracking || !this.WasLastQRCodeValid());

		this.contentRoot.SetActive(this.exhibit != null);
		this.trackingButtons.SetVisible(this.exhibit != null);
	}
	
	#endregion
	
	#region Helper Methods

	/// <summary>
	/// Resets the scene and unloads the content.
	/// </summary>
	public void Reset() {
		this.StopAllCoroutines();
		GameObject.Destroy(this.loadedContent);
		this.loadedContent = null;
		this.exhibit = null;
		this.QRSquare.SetActive(true);
		this.arManager.ResetTrackingJob();
	}
	
	/// <summary>
	/// Coroutine used to exit the scene safely.
	/// </summary>
	private IEnumerator SafeExit() {
		if (this.arManager != null) {
			this.arManager.StopHomography();
			
			while (!this.arManager.IsSafeToShutDown) {
				yield return null;
			}
		}
		
		ServiceLocator.Get<NavigationSceneManager>().PopScene(Constants.LOADING_SCENE_NAME);
		yield return null;
	}
	
	/// <summary>
	/// Loads the AR content asynchronously.
	/// </summary>
	/// <param name="contentID">The name of the content in resources.</param>
	private IEnumerator LoadContentAsync(string contentID) {
		this.loadingUI.SetActive(true);
		
		yield return new WaitForSeconds(1);
		
		this.loadedContent = GameObject.Instantiate(Resources.Load<GameObject>(contentID));
		if (this.loadedContent == null) {
			DebugUtils.LogError("QR code content ID is not in app.");
			yield break;
		}
		
		this.loadedContent.transform.SetParent(this.contentRoot.transform, false);
		this.loadedContent.transform.localPosition = Vector3.zero;
		
		MovableTrackball[] contentTrackballs = this.loadedContent.GetComponentsInChildren<MovableTrackball>(true);
		if (contentTrackballs.Length > 0) {
			contentTrackballs[0].SetMovementAbilities(false, false);
		}
		
		this.loadingUI.SetActive(false);
	}
	
	/// <summary>
	/// Initializes the scanned exhibit
	/// </summary>
	private void InitExhibit() {
		this.exhibit = ServiceLocator.Get<ContentManager>().GetExhibit(this.GetTrackedQRContentID());
		
		if (this.exhibit != null) {
			ServiceLocator.Get<ContentManager>().UnlockAssociatedMementos(this.exhibit);
			this.StartCoroutine(this.LoadContentAsync(this.exhibit.PrefabName));
			ServiceLocator.Get<TopBarController>().SetTitleWithKey(Constants.VIEW_TITLE_KEY, 0f);
		}
	}

	/// <summary>
	/// Gets the QR content ID that is associated with the given QR code.
	/// </summary>
	/// <returns>The QR content ID.</returns>
	/// <param name="decodedQRCodeString">The decoded QR code that we want to get the Content ID of.</param>
	private string GetQRContentID(string decodedQRCodeString) {
		string[] split = (string.IsNullOrEmpty(decodedQRCodeString) ? new string[0] : decodedQRCodeString.Split(' '));
		if (split.Length == 2 && split[0] == QRSceneController.QR_APP_KEY) {
			return split[1];
		} else {
			return null;
		}
	}

	/// <summary>
	/// Gets the QR content ID that is associated with the currently tracked QR code.
	/// </summary>
	private string GetTrackedQRContentID() {
		return this.GetQRContentID(this.arManager.GetTrackingDecodedQRCode());
	}

	/// <summary>
	/// Gets the QR content ID that is associated with the last decoded QR code.
	/// </summary>
	private string GetLastQRContentID() {
		return this.GetQRContentID(this.arManager.GetLastDecodedQRCode());
	}

	/// <summary>
	/// Returns a flag that indicates whether or not the last QR code that we saw was valid.
	/// </summary>
	private bool WasLastQRCodeValid() {
		return !string.IsNullOrEmpty(this.GetLastQRContentID());
	}
	
	#endregion
	
}
