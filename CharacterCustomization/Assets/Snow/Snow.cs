using UnityEngine;
using System.Collections;

public class Snow : MonoBehaviour 
{
	public GameObject Blizzard;
	public GameObject FastSnow;
	public GameObject FluffySnow;
	void Start () 
	{
		Blizzard.SetActiveRecursively (false);
		FastSnow.SetActiveRecursively (false);
		FluffySnow.SetActiveRecursively (false);	
	}

	void OnGUI()
	{
		if(GUILayout.Button("Blizzard"))
		{
			Blizzard.SetActiveRecursively (true);
			FastSnow.SetActiveRecursively (false);
			FluffySnow.SetActiveRecursively (false);
		}
		if(GUILayout.Button("FastSnow"))
		{
			FastSnow.SetActiveRecursively (true);
			Blizzard.SetActiveRecursively (false);
			FluffySnow.SetActiveRecursively (false);
		}
		if(GUILayout.Button("FluffySnow"))
		{
			FluffySnow.SetActiveRecursively (true);
			Blizzard.SetActiveRecursively (false);
			FastSnow.SetActiveRecursively (false);
		}
		if(GUILayout.Button("Close"))
		{
			Blizzard.SetActiveRecursively (false);
			FastSnow.SetActiveRecursively (false);
			FluffySnow.SetActiveRecursively (false);
		}
	}
}
