using UnityEngine;
using System.Collections;

public class TEST: MonoBehaviour {
	static int  n_index = 1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI()
	{

		if (GUI.Button (new Rect (10, 10, 150, 100), "I am a button")) {
			CharacterManager.Instance.ChangeFBX(Parts.C_ARM,"mm",FaceItem.F_EYE);
			Debug.Log ("you clicked the button");
			}

		if (GUI.Button (new Rect (10, 200, 100, 100), "change coat")) {
			n_index++;
			string str = n_index.ToString();//string.Format("AF_M_Jacket_{0:D2}",n_index);
			CharacterManager.Instance.ChangeFBX(Parts.C_COAT,str,FaceItem.F_NONE);
			Debug.Log("++++++++++++++++++"+n_index);
			}

		if (GUI.Button (new Rect (10, 300, 100, 100), "change Trousers")) {
			n_index++;
			string str = n_index.ToString();//string.Format("AF_M_Jacket_{0:D2}",n_index);
			CharacterManager.Instance.ChangeFBX(Parts.C_TROUSERS,str,FaceItem.F_NONE);
			Debug.Log("++++++++++++++++++"+n_index);
		}

	}

}
