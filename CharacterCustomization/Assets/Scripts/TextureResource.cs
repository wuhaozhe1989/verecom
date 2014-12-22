using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureResource : MonoBehaviour {
	public static Texture2D facetexture = null;

	public static GameObject arm = null;
	public static List<GameObject> CoatList  = new List<GameObject>();
	public static List<GameObject> TrousersList =new List<GameObject>();

	// Use this for initialization
	void Start () {
		facetexture = Resources.Load ("AF_T_Eye/AF_T_Eye_01") as Texture2D;
		arm = Resources.Load ("AF_M_Jacket/AF_M_Jacket_01") as GameObject;
		for (int i = 1; i< 17; i++)
		{
			string path = string.Format("AF_M_Jacket/AF_M_Jacket_{0:D2}/AF_M_Jacket_{0:D2}",i);
			GameObject temp  = Resources.Load (path) as GameObject;
			CoatList.Add(temp);
		}
		for (int i =0; i<16; i++) 
		{
			string path = string.Format("AF_M_Trousers/AF_M_Trousers_{0:D2}/AF_M_Trousers_{0:D2}",i);
			GameObject temp  = Resources.Load (path) as GameObject;
			TrousersList.Add(temp);
		}

		//facetexture.

	}

	// Update is called once per frame
	void Update () {
	
	}
}
