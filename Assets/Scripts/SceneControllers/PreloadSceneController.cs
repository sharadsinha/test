using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreloadSceneController : MonoBehaviour {

	/// <summary>
	/// The services that will be created when the app starts up.
	/// </summary>
	[SerializeField]
	private List<MonoBehaviour> services = new List<MonoBehaviour>();
	
	/// <summary>
	/// Create all the services then move on to the next scene.
	/// </summary>
	void Start () {
		foreach (MonoBehaviour service in this.services) {
			ServiceLocator.BindPrefab(service);
		}

		string previousLanguage = ServiceLocator.Get<LocalizationManager>().CurrentLanguage;
		if (string.IsNullOrEmpty(previousLanguage)) {
			ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.SPLASH_SCENE_NAME);
		} else {
			ServiceLocator.Get<LocalizationManager>().ChangeLanguage(previousLanguage);
			ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.MAIN_MENU_SCENE_NAME);
		}
	}
}
