using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MementosSceneController : SceneMonobehaviour {

	/// <summary>
	/// This must be done in awake rather than OnViewCreate so that it moves before it's rendered.
	/// </summary>
	void Awake() {
		if (ServiceLocator.Has<NavigationSceneManager>() && !ServiceLocator.Get<NavigationSceneManager>().IsPopping) {
			this.transform.position = Vector3.right * Screen.width;
		}
	}

	/// <summary>
	/// Sets up the TopBar transitions for when the Mementos Scene loads
	/// </summary>
	/// <param name="data">Any additional data required for initializing the scene.</param>
	public override void OnViewCreate(object data = null) {
		float secondsToTransition = 0f;
		if (!ServiceLocator.Get<NavigationSceneManager>().IsPopping) {
			secondsToTransition = Constants.SCENE_TRANSITION_TIME;
		}
		ServiceLocator.Get<TopBarController>().SetTitleWithKey(Constants.MEMENTOS_TITLE_KEY, secondsToTransition);
		ServiceLocator.Get<TopBarController>().SetLeftButton(null, secondsToTransition);
	}
	
	/// <summary>
	/// Animate the view displaying if it's being pushed on top of another scene.
	/// </summary>
	public override IEnumerator OnViewDisplay() {
		if (!ServiceLocator.Get<NavigationSceneManager>().IsPopping) {
			while (this.transform.position.x > 0f) {
				yield return null;
				
				this.transform.position = Vector3.right * (this.transform.position.x - (Time.deltaTime / Constants.SCENE_TRANSITION_TIME) * Screen.width);
			}
			
			this.transform.position = Vector3.zero;
		}
	}
	
	/// <summary>
	/// Animate the view off screen if it's being popped.
	/// </summary>
	public override IEnumerator OnViewHide() {
		if (ServiceLocator.Get<NavigationSceneManager>().IsPopping) {
			while (this.transform.position.x < Screen.width) {
				this.transform.position = Vector3.right * (this.transform.position.x + (Time.deltaTime / Constants.SCENE_TRANSITION_TIME) * Screen.width);
				yield return null;
			}
		}
	}
}
