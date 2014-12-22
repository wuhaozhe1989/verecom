using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundlesAll : MonoBehaviour {
	[MenuItem("Custom Editor/Create AssetBunldes ALL")]
	static void CreateAssetBunldesALL ()
	{
		
		Caching.CleanCache ();
		
		string Path = Application.dataPath + "/StreamingAssets/ALL.assetbundle";
		
		Object[] SelectedAsset = Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
		
		foreach (Object obj in SelectedAsset) 
		{
			Debug.Log ("Create AssetBunldes name :" + obj);
		}
		
		//这里注意第二个参数就行
		if (BuildPipeline.BuildAssetBundle (null, SelectedAsset, Path, BuildAssetBundleOptions.CollectDependencies)) {
			AssetDatabase.Refresh ();
		} else {
			
		}
	}
}
