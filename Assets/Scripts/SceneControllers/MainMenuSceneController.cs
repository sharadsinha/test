using UnityEngine;
using Prime31;
using System.Collections;

public class MainMenuSceneController : SceneMonobehaviour {

	/// <summary>
	/// The sprite to display when the TopBar is displaying a mementos button
	/// </summary>
	[SerializeField]
	private Sprite mementosSprite = null;

	/// <summary>
	/// This must be done in awake rather than OnViewCreate so that it moves before it's rendered.
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.mementosSprite != null, "The Mementos Sprite in Main Menu Scene Controller (Script) is not set");
		if (ServiceLocator.Has<NavigationSceneManager>()) {
			if (ServiceLocator.Get<NavigationSceneManager>().IsPopping) {
				this.transform.position = Vector3.left * Screen.width / 2f;
			} else if (ServiceLocator.Get<NavigationSceneManager>().LastScene == null) { // The main menu scene is only ever root when the SplashScene doesn't get loaded in
				this.transform.position = Vector3.zero;
			} else {
				this.transform.position = Vector3.right * Screen.width;
			}
		}
	}

	/// <summary>
	/// Sets up the TopBar transitions for when the Main Menu Scene loads
	/// </summary>
	/// <param name="data">Any additional data required for initializing the scene.</param>
	public override void OnViewCreate(object data = null) {
		float secondsToTransition = 0f;
		if (ServiceLocator.Get<NavigationSceneManager>().IsPopping) {
			secondsToTransition = Constants.SCENE_TRANSITION_TIME;
		}
		ServiceLocator.Get<TopBarController>().ShowTopBar();
		ServiceLocator.Get<TopBarController>().SetTitleWithKey(null, secondsToTransition);
		ServiceLocator.Get<TopBarController>().SetLeftButton(this.mementosSprite, secondsToTransition, () => {this.MementosButtonPressed();}, false);
	}

	/// <summary>
	/// Animate the view on screen.
	/// This animation will change if the view is being pushed on or popped to.
	/// </summary>
	public override IEnumerator OnViewDisplay() {
		if (ServiceLocator.Get<ContentManager>().GetNumberOfUnlockedMementos() > 0) {
			ServiceLocator.Get<TopBarController>().EnableLeftButton();
		} else {
			ServiceLocator.Get<TopBarController>().DisableLeftButton();
		}
		if (ServiceLocator.Get<NavigationSceneManager>().IsPopping) {
			while (this.transform.position.x < 0f) {
				yield return null;
				
				this.transform.position = Vector3.right * (this.transform.position.x + (Time.deltaTime / Constants.SCENE_TRANSITION_TIME) * Screen.width / 2f);
			}
			
			this.transform.position = Vector3.zero;
		} else {
			while (this.transform.position.x > 0f) {
				yield return null;
				
				this.transform.position = Vector3.right * (this.transform.position.x - (Time.deltaTime / Constants.SCENE_TRANSITION_TIME) * Screen.width);
			}
			
			this.transform.position = Vector3.zero;
		}
	}

	/// <summary>
	/// Animate the scene hiding.
	/// </summary>
	public override IEnumerator OnViewHide() {
		while (this.transform.position.x > -Screen.width / 2) {
			this.transform.position = Vector3.right * (this.transform.position.x - (Time.deltaTime / Constants.SCENE_TRANSITION_TIME) * Screen.width / 2f);
			yield return null;
		}
		ServiceLocator.Get<TopBarController>().EnableLeftButton();
	}

	/// <summary>
	/// Callback for the scan button
	/// </summary>
	public void ScanButtonPressed() {
		ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.QR_SCENE_NAME, Constants.LOADING_SCENE_NAME);
	}

	/// <summary>
	/// Callback for memento button
	/// </summary>
	public void MementosButtonPressed() {
		ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.MEMENTOS_SCENE_NAME);
	}
}
