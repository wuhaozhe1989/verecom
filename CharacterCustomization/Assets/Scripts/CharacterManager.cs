using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour {
	public GameObject body =null;
	public GameObject face = null;
	public GameObject hair =null;
	public GameObject leg= null;
	public GameObject arm = null;
	public GameObject coat = null;
	public GameObject thousers = null;
	public GameObject shoes = null;
	public GameObject hat = null;
	public GameObject ear = null;
	public static CharacterManager Instance = null; 
	// Use this for initialization
	//public  Texture[] faceTextures = null;
	void Start () {
		//glasses.SetActive (false);
		Instance = this;
		hat.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public  void ChangeFBX (Parts part, string index, FaceItem item = FaceItem.F_NONE)
	{
		if ((part != Parts.C_FACE )&&(part != Parts.C_NONE)) {//不是脸上的情况下换mesh
						//GameObject Destroygameobject = new GameObject ();
						GameObject FartherPart = GetGameobject (part);
						//GameObject Destroygameobject =  //NGUITools.FindInParents (FartherPart);
						foreach (Transform Destroygameobject in FartherPart.transform) {
								DestroyImmediate(Destroygameobject.gameObject);
						}
			int p = int.Parse(index);

			GameObject newpart = new GameObject(); 
			
			switch(part)
			{
			case Parts.C_COAT:
				newpart= GameObject.Instantiate ( TextureResource.CoatList[p])as GameObject;
				break;
			case Parts.C_TROUSERS:
				 newpart = GameObject.Instantiate ( TextureResource.TrousersList[p])as GameObject;
				break;

			}
			//GameObject newpart = GameObject.Instantiate ( TextureResource.CoatList[p])as GameObject;
			newpart.transform.parent = FartherPart.transform;
			newpart.transform.localRotation = Quaternion.identity;
			newpart.transform.localPosition = new Vector3(0,0,0);
			newpart.transform.localScale = new Vector3(1,1,1);

						//DestroyObject(ThisPart.GetComponentsInChildren<>());
				} else if(part == Parts.C_FACE) {
			   switch(item){
			case FaceItem.F_FACE:
				//face.GetComponentsInChildren<SkinnedMeshRenderer>().
				if(face.GetComponentInChildren<MeshRenderer>()==null){
					Debug.LogError("kklklklklklklklkl");
				}
				Debug.Log("_________________________:"+face.GetComponentInChildren<MeshRenderer>().materials.Length);
				//face.GetComponentInChildren<MeshRenderer>().materials[0].mainTexture = new Texture();
				break;
			case FaceItem.F_EYE:
				Debug.Log("_________________________:"+face.GetComponentInChildren<MeshRenderer>().materials.Length);
				//face.GetComponentInChildren<MeshRenderer>().materials[2].CopyPropertiesFromMaterial
				face.GetComponentInChildren<MeshRenderer>().materials[2].mainTexture = TextureResource.facetexture;
				break;
			case FaceItem.F_MOUTH:
				face.GetComponentInChildren<MeshRenderer>().materials[3].mainTexture = new Texture();
				break;
			case FaceItem.F_BROW:
				face.GetComponentInChildren<MeshRenderer>().materials[4].mainTexture = new Texture();
				break;
			case FaceItem.F_NOSE:
				face.GetComponentInChildren<MeshRenderer>().materials[2].mainTexture = new Texture();
				break;
				face.GetComponentInChildren<MeshRenderer>().materials[0].mainTexture = new Texture();
			case FaceItem.F_GLASSES:
				break;
			    }

				}else{
			    Debug.Log("this is wrong part ,part is null!");
		       }
	}

   public void ChangeTexture(Parts part ,string index){
		GameObject FartherPart = GetGameobject (part);
		GameObject changePart=null;
		foreach (Transform child in FartherPart.transform) {
			changePart = child.gameObject;
		}
	
		//changePart.GetComponent<SkinnedMeshRenderer>().material.mainTexture = Texture.
		}

GameObject GetGameobject(Parts part){
		GameObject ThisPart = null;
		switch (part) {
		case Parts.C_BODY:
			ThisPart = body;
			break;
		case  Parts.C_FACE:
			ThisPart = face;
			break;
		case  Parts.C_HAIR:
			ThisPart = hair;
			break;
		case  Parts.C_LEG:
			ThisPart = leg;
			break;
		case  Parts.C_ARM:
			ThisPart = arm;
			break;
		case  Parts.C_COAT:
			ThisPart = coat;
			break;
		case  Parts.C_TROUSERS:
			ThisPart = thousers;
			break;
		case  Parts.C_EAR:
			ThisPart = ear;
			break;
		/*case  Parts.C_EYE:
			ThisPart = face;
			break;
		case  Parts.C_NOSE:
			ThisPart = face;
			break;
		case  Parts.C_BROW:
			ThisPart = face;
			break;
		case Parts.C_MOUTH:
			ThisPart = face;
			break;*/
		case Parts.C_SHOES:
			ThisPart = shoes;
			break;
		case Parts.C_HAT:
			ThisPart = hat;
			break;
		/*case Parts.C_GLASSES:
			ThisPart = glasses;
			break;*/
		default:
			ThisPart = null;
			Debug.LogError("this part is null!!!");
			break;
		}
		return ThisPart;
		}
}
