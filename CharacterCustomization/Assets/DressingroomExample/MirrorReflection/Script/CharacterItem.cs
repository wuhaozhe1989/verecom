﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CharacterItem {

	public string name;
	public string bundleName;
	
	// The WWWs for retrieving the appropriate assetbundle are stored 
	// statically, so CharacterElements that share an assetbundle can
	// use the same WWW.
	// path to assetbundle -> WWW for retieving required assets
	static Dictionary<string, WWW> wwws = new Dictionary<string, WWW>();
	
	// The required assets are loaded asynchronously to avoid delays
	// when first using them. A LoadAsync results in an AssetBundleRequest
	// which are stored here so we can check their progress and use the
	// assets they contain once they are loaded.
	AssetBundleRequest gameObjectRequest;
	AssetBundleRequest materialRequest;
	AssetBundleRequest boneNameRequest;
	
	public CharacterItem(string name, string bundleName)
	{
		this.name = name;
		this.bundleName = bundleName;
	}
	
	// Returns the WWW for retieving the assetbundle required for this 
	// CharacterElement, and creates a WWW only if one doesnt exist already. 
	public WWW WWW
	{
		get
		{
			if (!wwws.ContainsKey(bundleName))
				wwws.Add(bundleName, new WWW(CharacterGenerator.AssetbundleBaseURL + bundleName));
			return wwws[bundleName];
		}
	}
	
	// Checks whether the SkinnedMeshRenderer and Material for this
	// CharacterElement are loaded, and starts the asynchronous loading
	// of those assets if it has not started already.
	public bool IsLoaded
	{
		get
		{
			if (!WWW.isDone) return false;
			
			if (gameObjectRequest == null)
				gameObjectRequest = WWW.assetBundle.LoadAsync("rendererobject", typeof(GameObject));
			
			if (materialRequest == null)
				materialRequest = WWW.assetBundle.LoadAsync(name, typeof(Material));
			
			if (boneNameRequest == null)
				boneNameRequest = WWW.assetBundle.LoadAsync("bonenames", typeof(StringHolder));
			
			if (!gameObjectRequest.isDone) return false;
			if (!materialRequest.isDone) return false;
			if (!boneNameRequest.isDone) return false;
			
			return true;
		}
	}
	
	public SkinnedMeshRenderer GetSkinnedMeshRenderer()
	{
		GameObject go = (GameObject)Object.Instantiate(gameObjectRequest.asset);
		go.renderer.material = (Material)materialRequest.asset;
		return (SkinnedMeshRenderer)go.renderer;
	}
	
	public string[] GetBoneNames()
	{
		var holder = (StringHolder)boneNameRequest.asset;
		return holder.content;
	}
}