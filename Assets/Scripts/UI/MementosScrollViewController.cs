using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(ScrollRect))]
public class MementosScrollViewController : MonoBehaviour {
	
	#region Private Members
	
	/// <summary>
	/// Reference to the prefab for the cell this scrollview will be populated with.
	/// </summary>
	[SerializeField]
	private MementoCell cellPrefab = null;
	
	/// <summary>
	/// Reference to the Scroll Rect attached to this object.
	/// </summary>
	private ScrollRect scrollRect = null;

	/// <summary>
	/// The amount of padding to add between each cell, in pixels
	/// </summary>
	[SerializeField]
	private float verticalPadding = 0f;

	#endregion
	
	#region MonoBehaviour

	/// <summary>
	/// Assert on Serialized Fields and get local references.
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.cellPrefab != null, "Missing reference to Cell Prefab on MementosScrollViewController");
		
		this.scrollRect = this.GetComponent<ScrollRect>();
		this.verticalPadding *= UnitySceneUtility.GetReferenceResolutionScaleFactor(this.GetComponentInParent<CanvasScaler>());
	}

	/// <summary>
	/// Create all the cells and add them to the scroll view.
	/// </summary>
	void Start() {
		float height = this.cellPrefab.GetComponent<RectTransform>().offsetMin.y;
		float totalHeight = 0;
	
		bool first = true;
		foreach (Memento memento in ServiceLocator.Get<ContentManager>().GetUnlockedMementos()) {
			if (!first) {
				first = false;
			} else {
				totalHeight -= this.verticalPadding;
			}
			
			MementoCell cell = GameObject.Instantiate<MementoCell>(this.cellPrefab);
			cell.transform.SetParent(this.scrollRect.content.transform);
			cell.transform.localScale = Vector3.one;
			RectTransform trans = cell.GetComponent<RectTransform>();
			trans.offsetMin = new Vector2(0, trans.offsetMin.y);
			trans.offsetMax = new Vector2(0, trans.offsetMax.y);
			Vector3 pos = cell.transform.localPosition;
			pos.y = totalHeight + height * 0.5f;
			cell.transform.localPosition = pos;
			totalHeight += height;
			
			cell.Setup(memento, this.CellSelected);
		}
		
		this.scrollRect.content.offsetMin = new Vector2(this.scrollRect.content.offsetMin.x, totalHeight);
	}
	
	#endregion
	
	#region Callback Functions
	
	/// <summary>
	/// Callback for when a cell is selected.
	/// </summary>
	/// <param name="memento">The memento corresponding to the selected cell.</param>
	private void CellSelected(Memento memento) {
		if (memento.supportsQRContent) {
			ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.MARKERLESS_SCENE_NAME, Constants.LOADING_SCENE_NAME, memento);
		} else if (memento.supportsInfo) {
			ServiceLocator.Get<NavigationSceneManager>().PushScene(Constants.WIKI_SCENE_NAME, Constants.LOADING_SCENE_NAME, memento);
		}
	}
	
	#endregion
}
