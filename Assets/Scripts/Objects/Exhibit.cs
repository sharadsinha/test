using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Exhibit : ARObject {

	#region Constants
	
	private const string MEMENTOS_KEY = "mementos";
	
	#endregion
	
	#region Private Members
	
	/// <summary>
	/// The mementos this exhibit unlocks.
	/// </summary>
	private List<Memento> mementos = new List<Memento>();
	
	#endregion
	
	#region Public Functions
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Exhibit"/> class.
	/// </summary>
	/// <param name="json">The JSON object used to create this class.</param>
	public Exhibit(Dictionary<string, object> json) : base(json) {
		if (json.ContainsKey(Exhibit.MEMENTOS_KEY)) {
			foreach (object obj in (List<object>)json[Exhibit.MEMENTOS_KEY]) {
				Memento memento = new Memento(obj as Dictionary<string, object>);
				this.mementos.Add(memento);
			}
		}
	}
	
	/// <summary>
	/// Returns all the mementos this Exhibit unlocks.
	/// </summary>
	public List<Memento> GetMementos() {
		return new List<Memento>(this.mementos);
	}
	
	#endregion
}
