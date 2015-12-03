using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OptionsMenuController : MonoBehaviour {

	#region Private Members

	/// <summary>
	/// Speed modifier for how quickly the options menu will slide in.
	/// Play back will take 1/animationSpeed * the total animation length to slide in
	/// </summary>
	[SerializeField]
	private float animationSpeed = 1.0f;

	/// <summary>
	/// The animator that we trigger when we want to show or hide the options menu
	/// </summary>
	[SerializeField]
	private Animator optionsMenuAnimator = null;

	/// <summary>
	/// The mute image to display in the options menu
	/// </summary>
	[SerializeField]
	private Image muteImage = null;

	/// <summary>
	/// The icon to display when the app has been muted
	/// </summary>
	[SerializeField]
	private Sprite mutedIcon = null;

	/// <summary>
	/// The icons to display when the app is not muted
	/// </summary>
	[SerializeField]
	private Sprite unmutedIcon = null;

	/// <summary>
	/// The language image to display in the options menu
	/// </summary>
	[SerializeField]
	private Image languageImage = null;

	/// <summary>
	/// The icon to display when English is selected
	/// </summary>
	[SerializeField]
	private Sprite englishIcon = null;

	/// <summary>
	/// The icond to display when French is selected
	/// </summary>
	[SerializeField]
	private Sprite frenchIcon = null;

	/// <summary>
	/// The button that will darken when the options menu is open
	/// </summary>
	[SerializeField]
	private Image optionsButtonImage = null;

	/// <summary>
	/// Boolean to check if the options menu is currently open or not
	/// </summary>
	private bool isOptionsMenuOpen = false;

	/// <summary>
	/// Boolean to check if the application is currently muted or not
	/// </summary>
	private bool isMuted = false;

	/// <summary>
	/// The key that we will save and retrieve our muted state with
	/// </summary>
	private const string IS_MUTED_SAVED_KEY = "MUTED";

	#endregion

	#region Private Constants

	private const string ENGLISH_LANGUAGE_CODE = "en";

	private const string FRENCH_LANGUAGE_CODE = "fr";

	private const string SHOW_OPTIONS_TRIGGER_NAME = "ShowOptions";
	
	private const string HIDE_OPTIONS_TRIGGER_NAME = "HideOptions";
	
	private const string ANIMATION_SPEED_VARIABLE_NAME = "Speed";

	#endregion

	#region MonoBehaviour

	/// <summary>
	/// Asserts that serialized variables are not null
	/// Sets up the different pieces of the options menu so that it gets populated and created properly
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.optionsMenuAnimator != null, "There is no Options Menu Animator attached to the Options Menu Controller (Script)");
		DebugUtils.Assert(this.muteImage != null, "There is no Mute Image attached to the Options Menu Controller (Script)");
		DebugUtils.Assert(this.mutedIcon != null, "There is no Muted Icon attached to the Options Menu Controller (Script)");
		DebugUtils.Assert(this.unmutedIcon != null, "There is no Unmuted Icon attached to the Options Menu Controller (Script)");
		DebugUtils.Assert(this.languageImage != null, "There is no Language Image attached to the Options Menu Controller (Script)");
		DebugUtils.Assert(this.englishIcon != null, "There is no English Icon attached to the Options Menu Controller (Script)");
		DebugUtils.Assert(this.frenchIcon != null, "There is no French Icon attached to the Options Menu Controller (Script)");
		DebugUtils.Assert(this.optionsButtonImage != null, "There is no Options Button Image attached to the Options Menu Controller (Script)");
		DebugUtils.Assert(this.animationSpeed > 0f, "The Animation Speed in the Options Menu Controller (Script) is not greater than 0");

		this.SetAnimationSpeed(this.animationSpeed);
	}

	/// <summary>
	/// Sets the correct initial language, and sets up a listener to update the options menu button when the language changes externally
	/// </summary>
	void Start() {
		bool initialMuteState = System.Convert.ToBoolean(XMGSaveLoadUtils.Instance.LoadString(OptionsMenuController.IS_MUTED_SAVED_KEY, System.Boolean.FalseString));
		this.SetMute(initialMuteState);
		this.UpdateLanguageIcons();
		ServiceLocator.Get<LocalizationManager>().OnLanguageChangedEvent += new LocalizationManager.LanguageChangedDelegate(this.UpdateLanguageIcons);
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Toggles the options menu to see or hide it
	/// </summary>
	public void ToggleMenu() {
		if (!this.isOptionsMenuOpen) {
			this.optionsButtonImage.color = Color.grey;
			this.optionsMenuAnimator.SetTrigger(OptionsMenuController.SHOW_OPTIONS_TRIGGER_NAME);
			this.isOptionsMenuOpen = true;
		} else {
			this.optionsButtonImage.color = Color.white;
			this.optionsMenuAnimator.SetTrigger(OptionsMenuController.HIDE_OPTIONS_TRIGGER_NAME);
			this.isOptionsMenuOpen = false;
		}
	}

	/// <summary>
	/// Sets the animation speed of the options menu transition
	/// </summary>
	public void SetAnimationSpeed(float speed) {
		if (speed > 0f) {
			this.animationSpeed = speed;
			this.optionsMenuAnimator.SetFloat(OptionsMenuController.ANIMATION_SPEED_VARIABLE_NAME, this.animationSpeed);
		}
	}

	/// <summary>
	/// Toggles between muted and unmuted
	/// </summary>
	public void ToggleMute() {
		this.SetMute(!this.isMuted);
	}

	/// <summary>
	/// Sets the mute value
	/// </summary>
	/// <param name="value">If set to <c>true</c> the application will be muted.</param>
	private void SetMute(bool value) {
		this.isMuted = value;
		if (this.isMuted) {
			this.muteImage.sprite = this.mutedIcon;
			//STUB
		} else {
			this.muteImage.sprite = this.unmutedIcon;
			//STUB
		}
		XMGSaveLoadUtils.Instance.SaveString(OptionsMenuController.IS_MUTED_SAVED_KEY, this.isMuted.ToString());
	}

	/// <summary>
	/// Toggles the language between English and French
	/// </summary>
	public void ToggleLanguage() {
		LocalizationManager localizationManager = ServiceLocator.Get<LocalizationManager>();
		string currentLanguage = localizationManager.CurrentLanguage;
		if (currentLanguage == OptionsMenuController.ENGLISH_LANGUAGE_CODE) {
			localizationManager.ChangeLanguage(OptionsMenuController.FRENCH_LANGUAGE_CODE);
		} else if (currentLanguage == OptionsMenuController.FRENCH_LANGUAGE_CODE) {
			localizationManager.ChangeLanguage(OptionsMenuController.ENGLISH_LANGUAGE_CODE);
		} else {
			DebugUtils.LogWarning("Warning: The OptionsMenuController does not support the selected language \"" + currentLanguage + "\" at this time");
		}
	}

	/// <summary>
	/// Updates the language icon in the dropdown options menu to reflect the currently selected language
	/// </summary>
	public void UpdateLanguageIcons() {
		LocalizationManager localizationManager = ServiceLocator.Get<LocalizationManager>();
		string currentLanguage = localizationManager.CurrentLanguage;
		if (currentLanguage == OptionsMenuController.ENGLISH_LANGUAGE_CODE) {
			this.languageImage.sprite = this.englishIcon;
		} else if (currentLanguage == OptionsMenuController.FRENCH_LANGUAGE_CODE) {
			this.languageImage.sprite = this.frenchIcon;
		} else {
			DebugUtils.LogWarning("Warning: The OptionsMenuController does not support the selected language \"" + currentLanguage + "\" at this time");
		}
	}

	/// <summary>
	/// Displays help for the current scene
	/// </summary>
	public void ShowHelp() {
		//TODO: Fill this in later
		//STUB
	}

	#endregion

}
