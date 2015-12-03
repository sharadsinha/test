using UnityEngine;
using System.Collections;

public class ResetTutorialButton : MonoBehaviour {

	/// <summary>
	/// Debug button function used to reset the tutorial state.
	/// </summary>
	public void ResetTutorial() {
		XMGSaveLoadUtils.Instance.SaveString(Constants.TUTORIAL_1_KEY, System.Boolean.FalseString);
		XMGSaveLoadUtils.Instance.SaveString(Constants.TUTORIAL_2_KEY, System.Boolean.FalseString);
		XMGSaveLoadUtils.Instance.SaveString(Constants.TUTORIAL_3_KEY, System.Boolean.FalseString);
		XMGSaveLoadUtils.Instance.SaveString(Constants.SELFIE_TUTORIAL_KEY, System.Boolean.FalseString);
		XMGSaveLoadUtils.Instance.SaveString(Constants.MEMENTOES_TUTORIAL_KEY, System.Boolean.FalseString);
		XMGSaveLoadUtils.Instance.SaveString(Constants.MARKERLESS_TUTORIAL_KEY, System.Boolean.FalseString);
		Application.Quit();
	}
}
