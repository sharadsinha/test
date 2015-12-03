using UnityEngine;
using System.Collections;

public class MementoDebugOptions : MonoBehaviour {

	/// <summary>
	/// Unlocks all mementos.
	/// </summary>
	public void UnlockAllMementos() {
		ServiceLocator.Get<ContentManager>().UnlockAllMementos();
		Application.Quit();
	}

	/// <summary>
	/// Locks all mementos.
	/// </summary>
	public void LockAllMementos() {
		ServiceLocator.Get<ContentManager>().LockAllMementos();
		ServiceLocator.Get<TopBarController>().DisableLeftButton();
	}
}
