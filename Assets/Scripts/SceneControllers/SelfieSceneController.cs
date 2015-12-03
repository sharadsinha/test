using Prime31;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

public class SelfieSceneController : SceneMonobehaviour {

	#region Native Methods

#if UNITY_IOS
	/// <summary>
	/// Displays a native IOS share sheet that can be used to share the selfie images.
	/// </summary>
	/// <param name="path">Location of the saved selfie image.</param>
	/// <param name="arrowHeightPercentage">Location of the arrow height on screen, in total percentage of screen height from the top downwards.</param>
	/// <param name="subject">Default subject to send along with the image in supported applications, such as email.</param>
	/// <param name="message">Default message to send along with the image in supported applications, such as email.</param>
	[DllImport("__Internal")]
	private static extern void _displayIOSShareSheet(string path, float arrowHeightPercentage, string subject, string message);
#endif
	
	#endregion

	#region Constants
	
	private const string SCREENSHOT_FILE_NAME = "screenShot.png";

	#endregion
	
	#region Private Members

	/// <summary>
	/// Reference to the device camera display.
	/// </summary>
	[SerializeField]
	private DeviceCameraDisplay deviceCameraDisplay = null;
	
	/// <summary>
	/// The root of the UI that will be deactivated when the screenshot is taken.
	/// </summary>
	[SerializeField]
	private GameObject disableOnScreenshot = null;
	
	/// <summary>
	/// The canvas group used to fade out all UI for the screenshot.
	/// </summary>
	[SerializeField]
	private CanvasGroup uiCanvasGroup = null;

	/// <summary>
	/// The canvas group used to fade the flash in and out for the screenshot.
	/// </summary>
	[SerializeField]
	private CanvasGroup flashCanvasGroup = null;
	
	/// <summary>
	/// UI Root for the share overlay.
	/// </summary>
	[SerializeField]
	private GameObject shareUI = null;
	
	/// <summary>
	/// UI Image to display the preview of the photo to be shared.
	/// </summary>
	[SerializeField]
	private Image shareImagePreview = null;
	
	/// <summary>
	/// The time it will take the fade out all the UI elements before the screenshot it taken.
	/// </summary>
	[SerializeField]
	private float hideUITime = 0.5f;
	
	/// <summary>
	/// The time it will take to fade in the flash image after taking a screenshot.
	/// </summary>
	[SerializeField]
	private float showFlashTime = 0.25f;
	
	/// <summary>
	/// The time it will take to fade out the flash image after the screenshot file is loaded from memory.
	/// </summary>
	[SerializeField]
	private float hideFlashTime = 1f;
	
	/// <summary>
	/// The root object the AR content will be parented under.
	/// </summary>
	[SerializeField]
	private Transform contentRoot = null;
	
	/// <summary>
	/// The ar object that will be displayed.
	/// </summary>
	private ARObject arObject = null;

	/// <summary>
	/// The height percentage of the screen that the share button is located in, that we will align the pop over arrow to, with 0% being the top of the screen and 100% being the bottom.
	/// </summary>
	[SerializeField]
	private float shareButtonHeightPercentage = 0.90f;
	
	#endregion
	
	#region Helper Methods

	/// <summary>
	/// Performs the screenshot and reads the file from memory.
	/// Since the screenshot and file write are async, this function is a coroutine to enable better control flow.
	/// </summary>
	private IEnumerator PerformScreenshot() {
		this.uiCanvasGroup.interactable = false;
		this.deviceCameraDisplay.Pause();
		
		// Fade out all UI
		float timer = this.hideUITime;
		while (this.uiCanvasGroup.alpha > 0f) {
			timer -= Time.deltaTime;
			this.uiCanvasGroup.alpha = timer / this.hideUITime;
			ServiceLocator.Get<TopBarController>().SetAlpha(timer / this.hideUITime);
			yield return null;
		}
		
		// Wait for render to UI is gone.
		yield return null;
	
#if UNITY_EDITOR
		Application.CaptureScreenshot(Application.persistentDataPath + "/" + SelfieSceneController.SCREENSHOT_FILE_NAME);
#elif UNITY_ANDROID
		Application.CaptureScreenshot(SelfieSceneController.SCREENSHOT_FILE_NAME);
#else
		Application.CaptureScreenshot(SelfieSceneController.SCREENSHOT_FILE_NAME);
#endif

		// Give time for screenshot to be taken.
		yield return null;
		yield return null;
		yield return null;

		// Animate in the flash
		timer = 0f;
		while (this.flashCanvasGroup.alpha < 1f) {
			timer += Time.deltaTime;
			this.flashCanvasGroup.alpha = timer / this.showFlashTime;
			yield return null;
		}
		
		// Give time for file to exist.
		yield return null;
		yield return null;
		yield return null;

		this.deviceCameraDisplay.Play();
		
		// Present the share UI if we see the screenshot in the file system.
		string path = Application.persistentDataPath + "/" + SelfieSceneController.SCREENSHOT_FILE_NAME;
		FileInfo info = new FileInfo(path);
		
		while (info == null || !info.Exists || info.Length <= 0) {
			yield return null;
			info = new FileInfo(path);
		}
		
		// Wait again to avoid any possible file conflicts.
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;

		this.shareUI.SetActive(true);
		
		byte[] file = File.ReadAllBytes(path);
		Texture2D image = new Texture2D(2, 2);
		image.LoadImage(file);
		this.shareImagePreview.sprite = Sprite.Create(image, new Rect(0f, 0f, image.width, image.height), new Vector2(0.5f, 0.5f));

		ServiceLocator.Get<TopBarController>().ShowTopBar();
		ServiceLocator.Get<TopBarController>().SetAlpha(1f);
		ServiceLocator.Get<TopBarController>().SetLeftButton(null, 0f, () => {this.CloseShareUI();});
		ServiceLocator.Get<TopBarController>().SetTitleWithKey(Constants.SELFIE_SHARE_TITLE_KEY, 0f);

		this.uiCanvasGroup.alpha = 1f;

		// Animate out the flash
		timer = this.hideFlashTime;
		while (this.flashCanvasGroup.alpha > 0f) {
			timer -= Time.deltaTime;
			this.flashCanvasGroup.alpha = timer / this.hideFlashTime;
			yield return null;
		}

		this.uiCanvasGroup.interactable = true;
	}
	
	#endregion
	
	#region SceneMonoBehaviour Methods

	/// <summary>
	/// Assert on serialized fields and setup the device camera display.
	/// </summary>
	/// <param name="data">The AR object to display.</param>
	public override void OnViewCreate(object data = null) {
		DebugUtils.Assert(this.deviceCameraDisplay !=  null, "Device Camera Display not set on SelfieSceneController");
		DebugUtils.Assert(this.uiCanvasGroup != null, "UI Canvas Group not set on SelfieSceneController");
		DebugUtils.Assert(this.flashCanvasGroup != null, "Flash Canvas Group not set on SelfieSceneController");
		DebugUtils.Assert(this.shareUI !=  null, "Share UI not set on SelfieSceneController");
		DebugUtils.Assert(this.shareImagePreview !=  null, "Share Image Preview not set on SelfieSceneController");
		DebugUtils.Assert(this.disableOnScreenshot != null, "Disable On Screenshot object not set on SelfieSceneController");
		DebugUtils.Assert(this.contentRoot != null, "Content Root not set on SelfieSceneController");
		
		File.Delete(Application.persistentDataPath + "/" + SelfieSceneController.SCREENSHOT_FILE_NAME);
		
		this.arObject = data as ARObject;
		GameObject loadedContent = GameObject.Instantiate(Resources.Load<GameObject>(this.arObject.PrefabName));
		if (loadedContent == null) {
			DebugUtils.LogError("QR code content ID is not in app.");
		} else {
			loadedContent.transform.SetParent(this.contentRoot.transform, false);
			loadedContent.transform.localPosition = Vector3.zero;
			MovableTrackball[] contentTrackballs = loadedContent.GetComponentsInChildren<MovableTrackball>(true);
			if (contentTrackballs.Length > 0) {
				contentTrackballs[0].SetMovementAbilities(true, true);
			}
		}
		
#if UNITY_IOS
		this.deviceCameraDisplay.Init(Mathf.Max(Screen.width, Screen.height), Mathf.Min(Screen.width, Screen.height), true);
#else
		this.deviceCameraDisplay.Init(Mathf.RoundToInt(Constants.DEFAULT_CAMERA_DIMENTIONS.x), Mathf.RoundToInt(Constants.DEFAULT_CAMERA_DIMENTIONS.y), true);
#endif
		
		this.shareUI.SetActive(false);

		ServiceLocator.Get<TopBarController>().SetTitleWithKey(Constants.SELFIE_TITLE_KEY, 0f);
		ServiceLocator.Get<TopBarController>().SetLeftButton(null, 0f, TopBarController.BackButtonLoadingScreenAction);
	}
	
	#endregion
		
	#region Button Callbacks
			
	/// <summary>
	/// Callback from take screenshot button.
	/// </summary>
	public void TakeScreenshot() {
		Screen.autorotateToPortrait = false;
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortraitUpsideDown = false;
		
		this.StartCoroutine(this.PerformScreenshot());
	}
	
	/// <summary>
	/// Close the share UI and delete the local copy of the photo.
	/// </summary>
	public void CloseShareUI() {
		File.Delete(Application.persistentDataPath + "/" + SelfieSceneController.SCREENSHOT_FILE_NAME);
		ServiceLocator.Get<TopBarController>().SetTitleWithKey(Constants.SELFIE_TITLE_KEY, 0f);
		ServiceLocator.Get<TopBarController>().SetLeftButton(null, 0f, TopBarController.BackButtonLoadingScreenAction);
		this.shareUI.SetActive(false);
		
		Screen.autorotateToPortrait = true;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	/// <summary>
	/// Callback from share selfie button.
	/// </summary>
	public void DisplayShareSheet() {
		string screenshotFilePath = Application.persistentDataPath + "/" + SelfieSceneController.SCREENSHOT_FILE_NAME;
		string subjectLine = ServiceLocator.Get<LocalizationManager>()[Constants.SELFIE_IMAGE_SHARE_SUBJECT_KEY];
		string message = ServiceLocator.Get<LocalizationManager>()[Constants.SELFIE_IMAGE_SHARE_MESSAGE_KEY];
#if UNITY_IOS
		SelfieSceneController._displayIOSShareSheet(screenshotFilePath, this.shareButtonHeightPercentage, subjectLine, message);
#elif UNITY_ANDROID
		EtceteraAndroid.shareWithNativeShareIntent(message, subjectLine, null, screenshotFilePath);
#endif
	}
	
	#endregion
	
	#region Testing
	
#if UNITY_EDITOR
	
	/// <summary>
	/// This method is used to mimic the SceneMonobehaviour flow in editor.
	/// </summary>
	void Start() {
		if (!ServiceLocator.Has<NavigationSceneManager>()) {
			this.OnViewCreate(null);
		}
	}
	
#endif

	#endregion
}
