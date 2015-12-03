using UnityEngine;
using System.Collections;

public class SplashSceneController : SceneMonobehaviour {

	/// <summary>
	/// Animate the view hiding.
	/// </summary>
	public override IEnumerator OnViewHide() {
		while (this.transform.position.x > -Screen.width / 2) {
			this.transform.position = Vector3.right * (this.transform.position.x - (Time.deltaTime / Constants.SCENE_TRANSITION_TIME) * Screen.width / 2f);
			yield return null;
		}
	}

	/// <summary>
	/// Callback for both language buttons.
	/// </summary>
	/// <param name="code">The code for the chosen language.</param>
	public void LanguageChosen(string code) {
		ServiceLocator.Get<LocalizationManager>().ChangeLanguage(code);
		ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.MAIN_MENU_SCENE_NAME);
	}
}
