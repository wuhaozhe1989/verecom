using UnityEngine;
using System.Collections;

public class StartUI : MonoBehaviour {
	public GameObject selectUI;
	// Use this for initialization
	void Start () {
		//transform.gameObject.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void StartPlay()
	{
		transform.gameObject.SetActive (false);
		selectUI.SetActive (true);
	}
}
