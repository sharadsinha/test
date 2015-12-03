using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Memento : ARObject {

	/// <summary>
	/// Whether or not the memento supports info
	/// </summary>
	public bool supportsInfo = true;

	/// <summary>
	/// Whether or not the memento supports QR content
	/// </summary>
	public bool supportsQRContent = true;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Memento"/> class.
	/// </summary>
	/// <param name="json">The JSON object this memento will be created from.</param>
	public Memento(Dictionary<string, object> json) : base(json) {
		this.supportsInfo = !string.IsNullOrEmpty(this.Info);
		this.supportsQRContent = !string.IsNullOrEmpty(this.PrefabName);
	}
}
