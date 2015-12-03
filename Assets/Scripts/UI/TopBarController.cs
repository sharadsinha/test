using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof (CanvasGroup))]
public class TopBarController : MonoBehaviour, AndroidBackClickHandler {

	#region Static Variables

	/// <summary>
	/// The back button action to use when we want to transition through a loading scene
	/// </summary>
	public static UnityAction BackButtonLoadingScreenAction = () => {ServiceLocator.Get<NavigationSceneManager>().PopScene(Constants.LOADING_SCENE_NAME);};

	#endregion

	#region Serialized Fields

	/// <summary>
	/// The left button on the TopBar
	/// </summary>
	[SerializeField]
	private Button leftButton = null;

	/// <summary>
	/// The image field that will be displayed on the left button of the TopBar
	/// </summary>
	[SerializeField]
	private Image leftButtonImage = null;

	/// <summary>
	/// The text field to display along the TopBar
	/// </summary>
	[SerializeField]
	private Text titleText = null;

	/// <summary>
	/// The image field to display along the TopBar
	/// </summary>
	[SerializeField]
	private Image titleImage = null;

	/// <summary>
	/// The canvas group that the TopBar is on.
	/// </summary>
	private CanvasGroup canvasGroup = null;

	#endregion

	#region Private Variables

	private bool isDisplayingText = false;

	/// <summary>
	/// The MaskableGraphic that is currently displayed on the TopBar, either a text string, or an image
	/// </summary>
	private MaskableGraphic displayedGraphic = null;

	private Sprite defaultLeftButtonSprite = null;

	/// <summary>
	/// The text that is displayed in the TopBar.
	/// Since it's a LocalizedText field, it will be translated when the select language changes
	/// </summary>
	private LocalizedText localizedTextField = null;

	/// <summary>
	/// Flag that indicates whether or not the back button will be "clicked" by a physical back button on device.
	/// </summary>
	private bool allowAndroidBackButton = true;

	#endregion

	#region Monobehaviour

	/// <summary>
	/// Asserts that serialized feilds are non-null
	/// Sets up the TopBar to be used in scenes
	/// Disables the title image and text, sets their alphas to 0 (clear), and grabs the default left button sprite for replacement later
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.leftButton != null, "The Left Button has not been set in Top Bar Controller (Script)");
		DebugUtils.Assert(this.titleText != null, "The Title Text has not been set in Top Bar Controller (Script)");
		DebugUtils.Assert(this.titleImage != null, "The Title Image has not been set in Top Bar Controller (Script)");
		DebugUtils.Assert(this.leftButtonImage != null, "The Left Button Image has not been set in Top Bar Controller (Script)");
		
		this.HideTopBar();
		this.localizedTextField = this.titleText.GetComponent<LocalizedText>();
		this.titleText.gameObject.SetActive(false);
		this.SetGraphicAlpha(this.titleText, 0f);
		this.titleImage.gameObject.SetActive(false);
		this.SetGraphicAlpha(this.titleImage, 0f);
		this.defaultLeftButtonSprite = this.leftButtonImage.sprite;
		this.canvasGroup = this.GetComponent<CanvasGroup>();
	}

	/// <summary>
	/// Sets the back click handler to be us.
	/// </summary>
	void Start() {
		if (ServiceLocator.Has<AndroidBackButtonManager>()) {
			ServiceLocator.Get<AndroidBackButtonManager>().SetBackClickHandler(this);
		}
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Shows the top bar.
	/// </summary>
	public void ShowTopBar() {
		this.gameObject.SetActive(true);
	}

	/// <summary>
	/// Hides the top bar.
	/// </summary>
	public void HideTopBar() {
		this.gameObject.SetActive(false);
	}

	/// <summary>
	/// Sets the alpha value of the Canvas Group that the TopBar is on.
	/// </summary>
	/// <param name="alpha">Alpha.</param>
	public void SetAlpha(float alpha) {
		this.canvasGroup.alpha = alpha;
	}

	/// <summary>
	/// Sets the left button on the top bar.
	/// </summary>
	/// <param name="sprite">The sprite to display on the button.</param>
	/// <param name="secondsToTransition">The number of seconds the transition will last.</param>
	/// <param name="function">Callback function that will run when the button is pressed, if <c>null</c> ithe current scene will be popped.</param>
	/// <param name="allowAndroidBackButton">Setting to allow a physical back button to be used in place of a touch or click.</param> 
	public void SetLeftButton(Sprite sprite = null, float secondsToTransition = Constants.SCENE_TRANSITION_TIME, UnityAction function = null, bool allowAndroidBackButton = true) {
		this.allowAndroidBackButton = allowAndroidBackButton;
		this.leftButton.onClick.RemoveAllListeners();
		this.EnableLeftButton();
		if (function == null) {
			function = () => ServiceLocator.Get<NavigationSceneManager>().PopScene();
		}
		// The given function for the left button is wrapped with a disable before the function gets exectued.
		// This assures that the button is disabled when clicked before the function is carried out, as it could try to trigger the button again otherwise.
		UnityAction functionWrapper = () => {this.DisableLeftButton(); function();};
		if (sprite == null) {
			this.StartCoroutine(this.TransitionLeftButtonImageTo(this.defaultLeftButtonSprite, secondsToTransition));
		} else {
			this.StartCoroutine(this.TransitionLeftButtonImageTo(sprite, secondsToTransition));
		}
		// We don't want to hook a second listener in to disabling the button since the function could then freely fire without disabling the button in a race condition.
		this.leftButton.onClick.AddListener(functionWrapper);
	}
	
	/// <summary>
	/// Enables the left button.
	/// </summary>
	public void EnableLeftButton() {
		this.leftButton.interactable = true;
		this.leftButtonImage.color = Color.white;
	}

	/// <summary>
	/// Disables the left button.
	/// </summary>
	public void DisableLeftButton() {
		this.leftButton.interactable = false;
		this.allowAndroidBackButton = false;
		this.leftButtonImage.color = Color.grey;
	}

	/// <summary>
	/// Sets the title's localized text field key
	/// </summary>
	/// <param name="key">Key that will be set. If null or empty, sets the title to be the Title Image</param>
	/// <param name="secondsToTransition">The number of seconds the transition will last.</param>
	public void SetTitleWithKey(string key, float secondsToTransition = Constants.SCENE_TRANSITION_TIME) {
		if (string.IsNullOrEmpty(key)) { // We want to display an image
			this.StartCoroutine(this.TransitionTitleToImage(secondsToTransition));
		} else { // We want to display text
			this.StartCoroutine(this.TransitionTitleToKey(key, secondsToTransition));
		}
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Transitions the left button image to a given sprite
	/// </summary>
	/// <param name="newSprite">New sprite.</param>
	/// <param name="secondsToTransition">The number of seconds the transition will last.</param>
	private IEnumerator TransitionLeftButtonImageTo(Sprite newSprite, float secondsToTransition) {
		yield return this.StartCoroutine(this.FadeTo(0f, this.leftButtonImage, secondsToTransition / 2f));
		this.leftButtonImage.sprite = newSprite;
		yield return this.StartCoroutine(this.FadeTo(1f, this.leftButtonImage, secondsToTransition / 2f));
	}

	/// <summary>
	/// Transitions the title to be the Title Image.
	/// </summary>
	/// <param name="secondsToTransition">The number of seconds the transition will last.</param>
	private IEnumerator TransitionTitleToImage(float secondsToTransition) {
		if (this.displayedGraphic != null) {
			yield return this.StartCoroutine(this.FadeTo(0f, this.displayedGraphic, secondsToTransition / 2f));
		}
		if (this.isDisplayingText) {
			this.titleText.gameObject.SetActive(false);
		}
		this.SetGraphicAlpha(this.titleImage, 0f);
		this.titleImage.gameObject.SetActive(true);
		yield return this.StartCoroutine(this.FadeTo(1f, this.titleImage, secondsToTransition / 2f));
		this.displayedGraphic = this.titleImage;
		this.isDisplayingText = false;
	}

	/// <summary>
	/// Transitions the title to text.
	/// </summary>
	/// <param name="newText">New text.</param>
	/// <param name="secondsToTransition">The number of seconds the transition will last.</param>
	private IEnumerator TransitionTitleToKey(string newKey, float secondsToTransition) {
		if (this.displayedGraphic != null) {
			yield return this.StartCoroutine(this.FadeTo(0f, this.displayedGraphic, secondsToTransition / 2f));
		}
		if (!this.isDisplayingText) {
			this.titleImage.gameObject.SetActive(false);
		}
		this.SetGraphicAlpha(this.titleText, 0f);
		this.titleText.gameObject.SetActive(true);
		this.localizedTextField.SetTextKey(newKey);
		yield return this.StartCoroutine(this.FadeTo(1f, this.titleText, secondsToTransition / 2f));
		this.displayedGraphic = this.titleText;
		this.isDisplayingText = true;
	}

	/// <summary>
	/// Fades a maskable graphic's alpha to a newly given alpha
	/// </summary>
	/// <param name="aValue">The alpha value that the fade will end on.</param>
	/// <param name="graphic">The maskable graphic who's color will change.</param>
	/// <param name="secondsToTransition">The number of seconds the transition will last.</param>
	private IEnumerator FadeTo(float aValue, MaskableGraphic graphic, float secondsToTransition) {
		if (secondsToTransition == 0f) {
			this.SetGraphicAlpha(graphic, aValue);
			yield break;
		}
		float alpha = graphic.color.a;
		for (float t = 0f; t < 1f; t += Time.deltaTime / secondsToTransition) {
			this.SetGraphicAlpha(graphic, Mathf.Lerp(alpha, aValue, t));
			yield return null;
		}
		this.SetGraphicAlpha(graphic, aValue);
		yield break;
	}

	/// <summary>
	/// Sets a maskable graphics alpha value
	/// </summary>
	/// <param name="graphic">The maskable graphic who's color will change.</param>
	/// <param name="alpha">The desired alpha value.</param>
	private void SetGraphicAlpha(MaskableGraphic graphic, float alpha) {
		Color originalColor = graphic.color;
		originalColor.a = alpha;
		graphic.color = originalColor;
	}

	#endregion

	#region AndroidBackClickHandler Interface Implementation

	/// <summary>
	/// Callback that gets executed when a physical back button is clicked on Android
	/// </summary>
	void AndroidBackClickHandler.OnBackClick() {
		if (this.allowAndroidBackButton) {
			this.leftButton.onClick.Invoke();
		}
	}

	#endregion

}
