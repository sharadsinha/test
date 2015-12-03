using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WikiSceneController : SceneMonobehaviour {

	/// <summary>
	/// Reference to the image object.
	/// </summary>
	[SerializeField]
	private Image image = null;
	
	/// <summary>
	/// Reference to the title text field.
	/// </summary>
	[SerializeField]
	private Text titleText = null;
	
	/// <summary>
	/// Reference to the description text field.
	/// </summary>
	[SerializeField]
	private Text descriptionText = null;
	
	/// <summary>
	/// Reference to the ratings view.
	/// </summary>
	[SerializeField]
	private RatingsViewController ratingsView = null;

	/// <summary>
	/// Reference to the rect transform of the scrollview content.
	/// </summary>
	[SerializeField]
	private RectTransform scrollViewContent = null;

	/// <summary>
	/// The AR object we are displaying info for.
	/// </summary>
	private ARObject arObject = null;

	/// <summary>
	/// Sets up the TopBar transitions for when the Info Scene loads.
	/// Loads and displays info for the ar object data.
	/// </summary>
	/// <param name="data">The AR object to display info for.</param>
	public override void OnViewCreate(object data = null) {
		DebugUtils.Assert(this.image != null, "Image not set on WikiSceneController");
		DebugUtils.Assert(this.titleText != null, "Title Text not set on WikiSceneController");
		DebugUtils.Assert(this.descriptionText != null, "Description Text not set on WikiSceneController");
		DebugUtils.Assert(this.ratingsView != null, "Ratings View not set on WikiSceneController");
		DebugUtils.Assert(this.scrollViewContent != null, "Scroll View Content not set on WikiSceneController");
	
		this.arObject = (ARObject)data;
		this.image.sprite = Resources.Load<Sprite>(this.arObject.ImageName);
		this.titleText.text = this.arObject.Title;
		this.descriptionText.text = this.arObject.Info;
	
		ServiceLocator.Get<TopBarController>().SetTitleWithKey(Constants.INFO_TITLE_KEY, 0f);
		ServiceLocator.Get<TopBarController>().SetLeftButton(null, 0f, TopBarController.BackButtonLoadingScreenAction);
	}

	
	/// <summary>
	/// Expand the content size to fit the expanded description text.
	/// </summary>
	public override IEnumerator OnViewDisplay() {
		// Wait for size of the description text to be set.
		yield return null;
		yield return null;
		yield return null;

		Vector2 offsetMin = this.scrollViewContent.offsetMin;
		offsetMin.y -= this.descriptionText.rectTransform.rect.height;
		this.scrollViewContent.offsetMin = offsetMin;
	}
	
}
