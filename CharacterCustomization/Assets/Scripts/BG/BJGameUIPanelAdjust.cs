using UnityEngine;
using System.Collections;

public class BJGameUIPanelAdjust : MonoBehaviour {
	void Awake(){
		if(Base.IOS_IPAD){
			var pos=transform.localPosition;
			pos.y=-64;
			transform.localPosition=pos;
		
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
