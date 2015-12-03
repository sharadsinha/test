using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalizationManager : MonoBehaviour {

	#region Private Constants
	
	private const string LANGUAGE_CODE_KEY = "id";

	private const string LANGUAGE_NAME_KEY = "name";

	private const string LOCALIZATION_DICT_KEY = "dict";

	/// <summary>
	/// The key that we will save and retrieve our previously selected language with
	/// </summary>
	private const string LANGUAGE_SAVED_KEY = "PREVIOUS_LANGUAGE";

	#endregion

	#region Private Members
	
	/// <summary>
	/// Reference to each of the json files containing the localization dicts.
	/// </summary>
	[SerializeField]
	private List<TextAsset> languageFiles = new List<TextAsset>();

	/// <summary>
	/// The current language code this manager is using to look up localized text.
	/// </summary>
	public string CurrentLanguage {
		get {
			return this.currentLanguage;
		}
	}
	private string currentLanguage = null;
	
	/// <summary>
	/// A dictionary of language codes to localization dictionaries.
	/// </summary>
	private Dictionary<string, Dictionary<string, string>> languageTable = new Dictionary<string, Dictionary<string, string>>();

	/// <summary>
	/// A dictionary of language codes to language names
	/// </summary>
	private Dictionary<string, string> languageCodeToNameTable = new Dictionary<string, string>();

	#endregion
	
	#region Public Members
	
	/// <summary>
	/// The callback format for when the language code changes.
	/// </summary>
	public delegate void LanguageChangedDelegate();
	
	/// <summary>
	/// Event signaling when the language code changes.
	/// </summary>
	public event LanguageChangedDelegate OnLanguageChangedEvent = null;
	
	#endregion

	#region Public Methods
	
	/// <summary>
	/// Gets the localized text with the specified language code and key.
	/// </summary>
	/// <param name="language">The language code for the text to be localized.</param>
	/// <param name="key">The key for the text to be localized.</param>
	public string this[string language, string key] {
		get {
			if (this.languageTable[language].ContainsKey(key)) {
				return this.languageTable[this.currentLanguage][key];
			} else {
				DebugUtils.LogError("LocalizationManager cannot find key: <" + key + "> for language <" + language + ">");
				return "";
			}
		}
	}
	
	/// <summary>
	/// Gets the localized text with the specified key using the current language code.
	/// </summary>
	/// <param name="key">The key for the text to be localized.</param>
	public string this[string key] {
		get {
			return this[this.currentLanguage, key];
		}
	}
	
	/// <summary>
	/// Gets the formatted text with the specified key and the current language code.
	/// This method requires the text to contain {0}, {1} etc. and for the number of those {#} to match the number of items.
	/// </summary>
	/// <returns>The localized text with the items replacing {#} values.</returns>
	/// <param name="key">The key for the text to be localized.</param>
	/// <param name="items">A list of items in order of how they should be placed into the localized string.</param>
	public string GetFormattedText(string key, List<string> items) {
		string text = this[key];
		
		for (int i = 0; i < items.Count; i++) {
			text = text.Replace("{" + i + "}", items[i]);
		}
		
		return text;
	}
	
	/// <summary>
	/// Returns a list of all language codes this manager supports.
	/// </summary>
	public List<string> GetSupportedLanguages() {
		return new List<string>(this.languageTable.Keys);
	}

	/// <summary>
	/// Converts a language code to a user friendly language name
	/// </summary>
	/// <returns>The name of the corresponding language</returns>
	/// <param name="code">The language code to translate</param>
	public string ConvertLanguageCodeToName(string code) {
		if (this.languageCodeToNameTable.ContainsKey(code)) {
			return this.languageCodeToNameTable[code];
		} else {
			DebugUtils.LogWarning("Unable to find language code translation");
			return "";
		}
	}
	
	/// <summary>
	/// Changes the current language code and notifies listeners of this change.
	/// </summary>
	/// <param name="newLanguage">The new current language code.</param>
	public void ChangeLanguage(string newLanguage) {
		if (this.GetSupportedLanguages().Contains(newLanguage)) {
			this.currentLanguage = newLanguage;
			XMGSaveLoadUtils.Instance.SaveString(LocalizationManager.LANGUAGE_SAVED_KEY, this.currentLanguage);
			
			if (this.OnLanguageChangedEvent != null) {
				this.OnLanguageChangedEvent();
			}
		}
	}

	#endregion
	
	#region MonoBehaviour
	
	/// <summary>
	/// Deserialize all the language files into the language table, then remove the files.
	/// </summary>
	void Awake() {
		this.currentLanguage = XMGSaveLoadUtils.Instance.LoadString(LocalizationManager.LANGUAGE_SAVED_KEY);

		foreach (TextAsset asset in this.languageFiles) {
			Dictionary<string, object> language = (Dictionary<string, object>)MiniJSON.Json.Deserialize(asset.text);
			Dictionary<string, object> dict = (Dictionary<string, object>)language[LocalizationManager.LOCALIZATION_DICT_KEY];
			Dictionary<string, string> stringDict = new Dictionary<string, string>();
			string languageCode = (string)language[LocalizationManager.LANGUAGE_CODE_KEY];
			string languageName = (string)language[LocalizationManager.LANGUAGE_NAME_KEY];
			this.languageCodeToNameTable[languageCode] = languageName;
			
			if (this.languageTable.ContainsKey(languageCode)) {
				stringDict = this.languageTable[languageCode];
			} else {
				this.languageTable.Add(languageCode, stringDict);
			}
			
			foreach (KeyValuePair<string, object> kvp in dict) {
				if (stringDict.ContainsKey(kvp.Key)) {
					DebugUtils.LogError("LocalizationManager already contains key: <" + kvp.Key + "> for language code <" + languageCode + ">");
				}
				stringDict.Add(kvp.Key, (string)kvp.Value);
			}
		}
		this.languageFiles.Clear();
	}
	
	#endregion

}
