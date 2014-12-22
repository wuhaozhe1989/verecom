using System;
using UnityEngine;
using System.Collections;

public class Load : MonoBehaviour {

	private string BundleURL="file://Mac/Users/Shared/Unity/3D_ChangeControl/Assets/StreamingAssets/Cube.assetbundle";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI(){
		if (GUI.Button (new Rect (200, 35, 100, 25), "Load")) 
		{
			StartCoroutine(LoadGameObject());
		}
	}
	IEnumerator LoadGameObject(){
		WWW bundle = new WWW (BundleURL);
		yield return bundle;
		yield return Instantiate (bundle.assetBundle.mainAsset);
		bundle.assetBundle.Unload (false);
	}
}
