using UnityEngine;
using System.Collections;

public class ParticleEffects : MonoBehaviour {
	public GameObject paraticle;
	public GameObject obj;
	private int paraticleID=0;
	// Use this for initialization
	void Start () {
		//paraticle=GameObject.Find("RainWithSplash_Shuriken");
	
	}
	
	// Update is called once per frame
	void Update () {
		if (paraticleID == 1) 
		{
			paraticle.particleSystem.Play() ;
		} 
		else if (paraticleID == 2) 
		{
			paraticle.transform.particleEmitter.emit = true;;	
		}
	
	}
	void OnGUI()
	{
		if (GUI.Button (new Rect (300, 35, 100, 25), "1")) 
		{
			paraticleID=1;
		}
		if(GUI.Button(new Rect (300,65,100,25),"2"))
		{

		}
						
	}
}
