using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuTutorial : MonoBehaviour {

	/// <summary>
	/// Parent GameObject for the first part of the main menu tutorial.
	/// </summary>
	[SerializeField]
	GameObject part1Parent = null;

	/// <summary>
	/// Parent GameObject for the second part of the main menu tutorial.
	/// </summary>
	[SerializeField]
	GameObject part2Parent = null;
	
	/// <summary>
	/// Parent GameObject for the third part of the main menu tutorial.
	/// </summary>
	[SerializeField]
	GameObject part3Parent = null;
	
	/// <summary>
	/// Asset on serialized fields and toggle active state on tutorial parts.
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.part1Parent != null, "Part 1 Parent not set on MainMenuTutorial");
		DebugUtils.Assert(this.part2Parent != null, "Part 2 Parent not set on MainMenuTutorial");
		DebugUtils.Assert(this.part3Parent != null, "Part 3 Parent not set on MainMenuTutorial");
	
		if (!System.Convert.ToBoolean(XMGSaveLoadUtils.Instance.LoadString(Constants.TUTORIAL_1_KEY, System.Boolean.FalseString))) {
			this.part1Parent.SetActive(true);
		} else if (!System.Convert.ToBoolean(XMGSaveLoadUtils.Instance.LoadString(Constants.TUTORIAL_2_KEY, System.Boolean.FalseString))) {
			if (ServiceLocator.Get<ContentManager>().GetNumberOfUnlockedMementos() > 0) {
				this.part2Parent.SetActive(true);
				XMGSaveLoadUtils.Instance.SaveString(Constants.TUTORIAL_2_KEY, System.Boolean.TrueString);
			}
		} else if (!System.Convert.ToBoolean(XMGSaveLoadUtils.Instance.LoadString(Constants.TUTORIAL_3_KEY, System.Boolean.FalseString))) {
			this.part3Parent.SetActive(true);
			XMGSaveLoadUtils.Instance.SaveString(Constants.TUTORIAL_3_KEY, System.Boolean.TrueString);
		} else {
			this.gameObject.SetActive(false);
		}
	}
}
