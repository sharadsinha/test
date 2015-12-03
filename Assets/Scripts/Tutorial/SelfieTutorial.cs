using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelfieTutorial : MonoBehaviour {

	/// <summary>
	/// The parent object of the tutorial UI.
	/// </summary>
	[SerializeField]
	GameObject tutorialPanel = null;
		
	/// <summary>
	/// Assert on serialized field and set tutorial active state.
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.tutorialPanel != null, "Tutorial Panel not set on SelfieTutorial");
	
		if (!System.Convert.ToBoolean(XMGSaveLoadUtils.Instance.LoadString(Constants.SELFIE_TUTORIAL_KEY, System.Boolean.FalseString))) {
			this.tutorialPanel.SetActive(true);
			XMGSaveLoadUtils.Instance.SaveString(Constants.SELFIE_TUTORIAL_KEY, System.Boolean.TrueString);
		} else {
			this.gameObject.SetActive(false);
		}
	}
}
