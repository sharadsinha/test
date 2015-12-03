using UnityEngine;
using System.Collections;

public class TestTopBarTransitionsTest : MonoBehaviour {

	/// <summary>
	/// The top bar controller that we will use to run the tests
	/// </summary>
	[SerializeField]
	private TopBarController topBarController = null;

	/// <summary>
	/// A given sprite to try to display during tests
	/// </summary>
	[SerializeField]
	private Sprite testSprite = null;

	/// <summary>
	/// Asserts that serialized fields are non-null
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.topBarController != null, "Top Bar Controller is null in Test Top Bar Transitions (Script)");
		DebugUtils.Assert(this.testSprite != null, "Test Sprite is null in Test Top Bar Transitions (Script)");
	}

	/// <summary>
	/// Starts the tests
	/// </summary>
	void Start () {
		this.topBarController.ShowTopBar();
		print("Set Title to 'Info'");
		this.topBarController.SetTitleWithKey(Constants.INFO_TITLE_KEY);
		StartCoroutine(this.TestTopBar());
	}

	/// <summary>
	/// The tests that will be conducted
	/// </summary>
	/// <returns>The top bar.</returns>
	private IEnumerator TestTopBar() {
		print("Set Title to 'Mementos'");
		yield return new WaitForSeconds(5);
		this.topBarController.SetTitleWithKey(Constants.MEMENTOS_TITLE_KEY);
		print("Set LeftImage to NULL");
		yield return new WaitForSeconds(5);
		this.topBarController.SetLeftButton(null);
		print("Set LeftImage to Test Sprite");
		yield return new WaitForSeconds(5);
		this.topBarController.SetLeftButton(testSprite);
		print("Set Title to CASM");
		yield return new WaitForSeconds(5);
		this.topBarController.SetTitleWithKey(null);
		print("Set LeftImage to null");
		yield return new WaitForSeconds(5);
		this.topBarController.SetLeftButton();
	}
}
