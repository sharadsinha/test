using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ContentManager : MonoBehaviour {

	#region Constants

	private const string SAVED_MEMENTOS_KEY = "SAVED_MEMENTOS";

	#endregion

	#region Private Members

	/// <summary>
	/// The json files containing the preloaded content.
	/// </summary>
	[SerializeField]
	private List<TextAsset> exhibitFiles = new List<TextAsset>();
	
	/// <summary>
	/// The exhibits being managed.
	/// </summary>
	private List<Exhibit> exhibits = new List<Exhibit>();

	/// <summary>
	/// The mementos that the user has unlocked by scanning their related exhibits
	/// </summary>
	private List<Memento> unlockedMementos = new List<Memento>();

	#endregion

	#region Monobehaviour

	/// <summary>
	/// Deserialize the content files.
	/// </summary>
	void Awake () {
		foreach (TextAsset asset in this.exhibitFiles) {
			Dictionary<string, object> json = (Dictionary<string, object>)MiniJSON.Json.Deserialize(asset.text);
			Exhibit exhibit = new Exhibit(json);
			this.exhibits.Add(exhibit);
		}
		this.LoadUnlockedMementos();
	}

	#endregion

	#region Public Methods
	
	/// <summary>
	/// Gets all the mementos from the exhibits this manager is aware of.
	/// </summary>
	public List<Memento> GetAllMementos() {
		List<Memento> ret = new List<Memento>();
		foreach (Exhibit exhibit in this.exhibits) {
			ret.AddRange(exhibit.GetMementos());
		}
		
		return ret;
	}

	/// <summary>
	/// Gets all the mementos that the user has unlocked.
	/// </summary>
	public List<Memento> GetUnlockedMementos() {
		return new List<Memento>(this.unlockedMementos);
	}

	/// <summary>
	/// Gets the number of unlocked mementos.
	/// </summary>
	public int GetNumberOfUnlockedMementos() {
		return this.unlockedMementos.Count;
	}
	
	/// <summary>
	/// Gets the exhibit with the given ID.
	/// </summary>
	/// <param name="id">The ID if the exhibit to be returned.</param>
	public Exhibit GetExhibit(string id) {
		return this.exhibits.Find(exhibit => exhibit.ID == id);
	}

	/// <summary>
	/// Unlocks the associated mementos that go with the scanned exhibit.
	/// </summary>
	/// <param name="scannedExhibit">The exhbit that we are scanning.</param>
	public void UnlockAssociatedMementos(Exhibit scannedExhibit) {
		foreach (Memento memento in scannedExhibit.GetMementos()) {
			if (!this.unlockedMementos.Contains(memento)) {
				this.unlockedMementos.Add(memento);
			}
		}
		this.SaveUnlockedMementos();
	}

	/// <summary>
	/// Unlocks all mementos.
	/// </summary>
	public void UnlockAllMementos() {
		foreach (Exhibit exhibit in this.exhibits) {
			this.UnlockAssociatedMementos(exhibit);
		}
	}

	/// <summary>
	/// Locks all mementos.
	/// </summary>
	public void LockAllMementos() {
		this.unlockedMementos.Clear();
		this.SaveUnlockedMementos();
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Saves the unlocked mementos using XMGSaveLoadUtils
	/// </summary>
	private void SaveUnlockedMementos() {
		string savedMementoIDString = string.Join(",", this.unlockedMementos.Select(memento => memento.ID).ToArray());
		XMGSaveLoadUtils.Instance.SaveString(ContentManager.SAVED_MEMENTOS_KEY, savedMementoIDString);
	}

	/// <summary>
	/// Loads the unlocked mementos from XMGSaveLoadUtils and populates this.unlockedMementos
	/// </summary>
	private void LoadUnlockedMementos() {
		List<string> savedMementoIDList = XMGSaveLoadUtils.Instance.LoadString(ContentManager.SAVED_MEMENTOS_KEY, "").Split(',').ToList();
		this.unlockedMementos = this.GetAllMementos().FindAll(memento => savedMementoIDList.Contains(memento.ID));
	}

	#endregion

}
