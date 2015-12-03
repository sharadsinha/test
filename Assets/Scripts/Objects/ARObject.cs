using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ARObject {

	#region Constants
	
	protected const string ID_KEY = "id";
	
	protected const string PREFAB_NAME_KEY = "prefabName";
	
	protected const string TITLE_KEY = "title";
	
	protected const string INFO_KEY = "info";
	
	protected const string IMAGE_NAME_KEY = "imageName";
	
	#endregion
	
	#region Public/Protected Members
	
	/// <summary>
	/// The ID for this AR object.
	/// </summary>
	public string ID {
		get;
		protected set;
	}
	
	/// <summary>
	/// The prefab name for this AR object.
	/// Used to look up the AR object in resources.
	/// </summary>
	public string PrefabName {
		get;
		protected set;
	}
	
	/// <summary>
	/// The title for this AR object.
	/// </summary>
	public string Title {
		get;
		protected set;
	}
	
	/// <summary>
	/// The info string for this AR object.
	/// </summary>
	public string Info {
		get;
		protected set;
	}
	
	/// <summary>
	/// The name of the image for this AR object.
	/// </summary>
	public string ImageName {
		get;
		private set;
	}

	#endregion
	
	#region Public Functions
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ARObject"/> class.
	/// </summary>
	/// <param name="json">The JSON object used to create this class.</param>
	public ARObject(Dictionary<string, object> json) {
		this.ID = (string)json[ARObject.ID_KEY];
		this.PrefabName = (string)json[ARObject.PREFAB_NAME_KEY];
		this.Title = (string)json[ARObject.TITLE_KEY];
		this.Info = (string)json[ARObject.INFO_KEY];
		this.ImageName = (string)json[ARObject.IMAGE_NAME_KEY];
	}
	
	#endregion
}
