using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MementoCell : MonoBehaviour {

	#region Private Members
	
	/// <summary>
	/// Reference to the main image on the left side of the cell.
	/// </summary>
	[SerializeField]
	private Image mainImage = null;
	
	/// <summary>
	/// Reference to the title text field.
	/// </summary>
	[SerializeField]
	private Text infoText = null;

	/// <summary>
	/// Place holders for icons that will fit in the mementos view
	/// </summary>
	[SerializeField]
	private Image[] iconSpots = null;

	/// <summary>
	/// The maximum number of allowed content icons to be displayed
	/// </summary>
	private const int MAX_CONTENT_ICON_SPOTS = 4;

	/// <summary>
	/// The info icon to use if info is available
	/// </summary>
	[SerializeField]
	private Sprite infoIcon = null;

	/// <summary>
	/// The QR icon to use if QR content is available
	/// </summary>
	[SerializeField]
	private Sprite qrIcon = null;
	
	/// <summary>
	/// The callback function for whent this cell is selected.
	/// </summary>
	private CellSelectedDelegate callback = null;
	
	/// <summary>
	/// The memento this cell is representing.
	/// </summary>
	private Memento memento = null;
	
	#endregion
	
	#region Public Members
	
	/// <summary>
	/// The format for the callback for when this cell is selected.
	/// </summary>
	public delegate void CellSelectedDelegate(Memento memento);
	
	#endregion
	
	#region MonoBehaviour
	
	/// <summary>
	/// Assert on serialized fields.
	/// </summary>
	void Awake() {
		DebugUtils.Assert(this.mainImage != null, "Main Image not set on MementoCell");
		DebugUtils.Assert(this.infoText != null, "Info Text not set on MementoCell");
		DebugUtils.Assert(this.iconSpots.Length == MAX_CONTENT_ICON_SPOTS, "Icon Spots doesn't have enough space for all of the possible icons on the MementoCell");
		DebugUtils.Assert(this.infoIcon != null, "Info Icon not set on MementoCell");
		DebugUtils.Assert(this.qrIcon != null, "QR Icon not set on MementoCell");
	}
	
	#endregion
	
	#region Public Methods
	
	/// <summary>
	/// Set up this cell with a memento and a callback.
	/// </summary>
	/// <param name="memento">The memento this cell will represent.</param>
	/// <param name="callback">The callback function for when this cell is selected.</param>
	public void Setup(Memento memento, CellSelectedDelegate callback) {
		this.memento = memento;
		
		this.mainImage.sprite = Resources.Load<Sprite>(this.memento.ImageName);
		this.infoText.text = memento.Title;
		int nextIconIndex = 0;
		if (memento.supportsInfo) {
			this.iconSpots[nextIconIndex++].sprite = this.infoIcon;
		}
		if (memento.supportsQRContent) {
			this.iconSpots[nextIconIndex++].sprite = this.qrIcon;
		}
		for (int i = 0; i < nextIconIndex; i++) {
			this.iconSpots[i].gameObject.SetActive(true); // Activates all of the supported icons
		}
		this.callback = callback;
	}
	
	/// <summary>
	/// Called when this cell is clicked.
	/// </summary>
	public void CellClicked() {
		this.callback(this.memento);
	}
	
	#endregion
}
