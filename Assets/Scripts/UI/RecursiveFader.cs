using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecursiveFader : MonoBehaviour {

	#region Private Members
	
	/// <summary>
	/// The time it takes for the graphics to fade in.
	/// </summary>
	[SerializeField]
	private float fadeInTime = 0.2f;
	
	/// <summary>
	/// The time it takes the graphics to fade out.
	/// </summary>
	[SerializeField]
	private float fadeOutTime = 1.0f;
	
	/// <summary>
	/// True if all the graphics should start off with alpha zero.
	/// </summary>
	[SerializeField]
	private bool startInvisible = false;

	/// <summary>
	/// List of all the graphic objects on this GameObject and it's children.
	/// </summary>
	private List<Graphic> graphicList = new List<Graphic>();
	
	/// <summary>
	/// List of the original alpha values for all the graphic objects in graphicList.
	/// </summary>
	private List<float> originalAlphas = new List<float>();

	/// <summary>
	/// Timer indicating how long it has been since the last time the visibility state changed (in seconds).
	/// </summary>
	private float timer = 0;
	
	/// <summary>
	/// The alpha this fader is fading to for all it's graphic objects.
	/// </summary>
	private float targetAlpha = 1f;
	
	/// <summary>
	/// The alpha this fader is fading from for all it's graphic objects.
	/// </summary>
	private float startingAlpha = 1f;
	
	/// <summary>
	/// The current alpha value for this fader.
	/// </summary>
	private float currentAlpha = 1f;
	
	#endregion
	
	#region Monobehaviour Methods

	/// <summary>
	/// Get references to all the graphic objects recursively and set the initial alpha.
	/// </summary>
	void Start() {
		this.graphicList.AddRange(this.GetComponents<Graphic>());
		this.graphicList.AddRange(this.GetComponentsInChildren<Graphic>());
		
		this.graphicList.ForEach(graphic => originalAlphas.Add(graphic.color.a));
		
		if (this.startInvisible) {
			this.targetAlpha = 0f;
			this.currentAlpha = 0f;
			this.startingAlpha = 0f;
		}
		
		this.SetAlpha();
	}
	
	/// <summary>
	/// Calculate and set the new alpha value for this frame based off time since visibility set.
	/// </summary>
	void Update() {
		this.timer += Time.deltaTime;
		float larpVal;
		
		if (this.targetAlpha == 1f) {
			larpVal = (this.fadeInTime==0 ? 1f : this.timer / this.fadeInTime);
		} else {
			larpVal = (this.fadeOutTime==0 ? 1f : this.timer / this.fadeOutTime);
		}
		
		float before = this.currentAlpha;
		this.currentAlpha = Mathf.Lerp(this.startingAlpha, this.targetAlpha, larpVal);
		
		if (this.currentAlpha != before) {
			this.SetAlpha();
		}
	}
	
	#endregion
	
	#region Public Methods
	
	/// <summary>
	/// Sets the visiblity of all the graphic objects on this object and it's children.
	/// </summary>
	/// <param name="visible">If set to <c>true</c> graphics will fade in.</param>
	/// <param name="instant">If set to <c>true</c> graphics alpha will be set instantly.</param>
	public void SetVisiblity(bool visible, bool instant = false) {
		float before = this.targetAlpha;
		this.targetAlpha = visible ? 1f : 0f;
		
		if (before != this.targetAlpha) {
			this.startingAlpha = this.currentAlpha;
			this.timer = 0;
		}
		
		if (instant) {
			this.currentAlpha = this.startingAlpha = this.targetAlpha;
			this.SetAlpha();
		}
	}
	
	#endregion
	
	#region Helper Methods
	
	/// <summary>
	/// Sets the alpha value for each graphic object.
	/// Will not set an alpha value greater than the one the object started with.
	/// </summary>
	private void SetAlpha() {
		for (int i = 0; i < this.graphicList.Count; i++) {
			Graphic g = this.graphicList[i];
			Color c = g.color;
			float limit = this.originalAlphas[i];
			
			c.a = Mathf.Clamp(this.currentAlpha, 0f, limit);
			this.graphicList[i].color = c;
		}
	}
	
	#endregion
}
