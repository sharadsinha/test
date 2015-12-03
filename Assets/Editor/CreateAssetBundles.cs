using UnityEditor;

public class CreateAssetBundles {

	/// <summary>
	/// Script taken from unity website to build asset bundles because Unity is too lazy to make this native.
	/// </summary>
	[MenuItem ("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles () {
		BuildPipeline.BuildAssetBundles("AssetBundles");
	}
	
}
