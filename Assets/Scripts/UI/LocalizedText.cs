using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour {

	#region Private Members
	
	/// <summary>
	/// The localization key used to localize this text.
	/// </summary>
	[SerializeField]
	private string localizationKey = "";
	
	/// <summary>
	/// The items to be inserted into a formatted localized string.
	/// </summary>
	private List<string> formattedTextItems = new List<string>();

	/// <summary>
	/// Reference to the text field this script will update.
	/// </summary>
	private Text textField = null;
	
	#endregion

	#region MonoBehaviour

	/// <summary>
	/// Get local references and subscribe to event.
	/// </summary>
	void Awake() {
		this.textField = this.GetComponent<Text>();
		
		if (string.IsNullOrEmpty(this.localizationKey)) {
			DebugUtils.LogError("Text " + this.gameObject.name + " missing Localization Key");
		}
		
		ServiceLocator.Get<LocalizationManager>().OnLanguageChangedEvent += this.SetTextField;
	}
	
	/// <summary>
	/// Unsubscribe from event.
	/// </summary>
	void OnDestroy() {
		ServiceLocator.Get<LocalizationManager>().OnLanguageChangedEvent -= this.SetTextField;
	}
	
	/// <summary>
	/// Set the text field initially.
	/// </summary>
	void Start() {
		this.SetTextField();
	}
	
	#endregion
	
	#region Helper Methods
	
	/// <summary>
	/// Sets the text field to be the current language's localized text.
	/// </summary>
	private void SetTextField() {
		this.textField.text = ServiceLocator.Get<LocalizationManager>().GetFormattedText(this.localizationKey, this.formattedTextItems);
	}
	
	#endregion
	
	#region Public Methods
	
	/// <summary>
	/// Sets a new key for this localized text and update the text field.
	/// </summary>
	/// <param name="key">The new key.</param>
	/// <param name="formattedTextItems">The formatted text items to replace in the localized text.</param>
	public void SetTextKey(string key, List<string> formattedTextItems = null) {
		this.localizationKey = key;
		if (formattedTextItems != null) {
			this.formattedTextItems = formattedTextItems;
		}
		
		this.SetTextField();
	}
	
	#endregion
	
}
