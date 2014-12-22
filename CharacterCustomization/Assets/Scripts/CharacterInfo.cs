using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WHZCharacterInfo
{
	public string name;
	public string bundleName;
	public string uid;
	static Dictionary<string, WWW> wwws = new Dictionary<string, WWW>();

	public Parts part = Parts.C_NONE;
	AssetBundleRequest gameObjectRequest;
	AssetBundleRequest materialRequest;
	AssetBundleRequest boneNameRequest;

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
public enum Parts { //角色属性
	C_NONE = 0,
	C_BODY, //
	C_FACE ,
	C_HAIR,
	C_ARM,
	C_LEG,
	C_SHOES,
	C_COAT,
	C_TROUSERS,
	C_EAR,
	C_HAT
};
public enum FaceItem{//脸部的属性
	F_NONE = 0,
	F_FACE,
	F_MOUTH,
	F_EYE,
	F_BROW,
	F_NOSE,
	F_GLASSES
}
