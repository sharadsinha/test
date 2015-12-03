using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RatingsViewController : MonoBehaviour {

	#region Private Members

	/// <summary>
	/// The sprite to be used for an empty user rating star.
	/// </summary>
	[SerializeField]
	private Sprite emptyUserStar = null;
	
	/// <summary>
	/// The sprite to be used for a filled user rating star.
	/// </summary>
	[SerializeField]
	private Sprite filledUserStar = null;
	
	/// <summary>
	/// The list of user rating star images.
	/// </summary>
	[SerializeField]
	private List<Image> userRatingStars = new List<Image>();
	
	#endregion
	
	#region MonoBehaviour
	
	/// <summary>
	/// Assert on serialized fields and clear the user review stars.
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.emptyUserStar != null, "Empty User Star not set on RatingsViewController.");
		DebugUtils.Assert(this.filledUserStar != null, "Filled User Star not set on RatingsViewController.");
		DebugUtils.Assert(this.userRatingStars.Count == 5, "Should be 5 objects in User Ratings Starts on RatingsViewController.");
		
		// TODO: Load these from user's previous review if we have that data.
		this.userRatingStars.ForEach(star => star.sprite = this.emptyUserStar);
		
		// TODO: Set avg rating stars based on backend data.
	}
	
	#endregion
	
	#region Callback Methods
	
	/// <summary>
	/// Callback for when a star image is clicked.
	/// </summary>
	/// <param name="index">The index of the star that was clicked.</param>
	public void StarClicked(int index) {
		for (int i = 0; i < userRatingStars.Count; i++) {
			this.userRatingStars[i].sprite = (i <= index ? this.filledUserStar : this.emptyUserStar);
		}
		
		// TODO: Set rating in backend data.
	}
	
	#endregion
	
}
