using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlideAnimation : MonoBehaviour {
	
	#region Private Members
	
	/// <summary>
	/// If false, the object will instantly move offscreen on start.
	/// </summary>
	[SerializeField]
	private bool startOnScreen = true;

	/// <summary>
	/// The time it will take for the object to animate off or on screen.
	/// </summary>
	[SerializeField]
	private float animationTime = 0.5f;

	/// <summary>
	/// The position of the object when it's hidden.
	/// </summary>
	private float hiddenPos = 0f;
	
	/// <summary>
	/// The position of the object when it's visible.
	/// </summary>
	private float visiblePos = 0f;

	/// <summary>
	/// The position the object is animating from.
	/// </summary>
	private float sourcePos = 0f;
	
	/// <summary>
	/// The position the object is animating to.
	/// </summary>
	private float targetPos = 0f;
	
	/// <summary>
	/// The time since the object's isShowing state changed.
	/// </summary>
	private float timeSinceSet = 0f;
	
	/// <summary>
	/// Reference to the rect transform of the object.
	/// </summary>
	private RectTransform rect = null;

	/// <summary>
	/// True if the object should be shown on screen.
	/// </summary>
	private bool isShowing = true;
	
	#endregion
	
	#region Monobahaviour
	
	/// <summary>
	/// Record position values and set initial position.
	/// </summary>
	void Start() {
		DebugUtils.Assert(this.animationTime > 0, "Animation time must be greater than 0");
	
		this.isShowing = this.startOnScreen;
	
		this.rect = this.GetComponent<RectTransform>();
		
		float height = Screen.height * (this.rect.anchorMax.y - this.rect.anchorMin.y) / UnitySceneUtility.GetReferenceResolutionScaleFactor(this.GetComponentInParent<CanvasScaler>());
		
		this.visiblePos = this.rect.anchoredPosition.y;
		this.hiddenPos = this.visiblePos - height;
		
		this.targetPos = (this.isShowing ? this.visiblePos : this.hiddenPos);
		this.sourcePos = this.targetPos;
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, this.targetPos);
	}
	
	/// <summary>
	/// Animate until desired position is reached.
	/// </summary>
	void Update() {
		this.timeSinceSet += Time.deltaTime;
	
		this.rect.anchoredPosition = new Vector2(this.rect.anchoredPosition.x, Mathf.Lerp(this.sourcePos, this.targetPos, this.timeSinceSet / this.animationTime));
		
		if (this.rect.anchoredPosition.y == this.targetPos) {
			this.enabled = false;
		}
	}
	
	#endregion
	
	#region Public Methods
	
	/// <summary>
	/// Sets this animator's visibility state.
	/// </summary>
	/// <param name="visible">If set to <c>true</c> visible.</param>
	public void SetVisible(bool visible) {
		if (visible == this.isShowing) {
			return;
		}
	
		this.isShowing = visible;
		this.enabled = true;
		
		this.sourcePos = this.rect.anchoredPosition.y;
		this.targetPos = (this.isShowing ? this.visiblePos : this.hiddenPos);
		
		this.timeSinceSet = 0f;
	}
	
	#endregion
}
