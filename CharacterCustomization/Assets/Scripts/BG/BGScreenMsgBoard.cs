using System.Collections;
using UnityEngine;

/**
 *BaoGameFramework 2012.10.3
 *显示log信息，临时显示
 **/

[AddComponentMenu("BaoGameFrameWork/MsgBoard")]
public class BGScreenMsgBoard : MonoBehaviour
{
    public static GameObject onlyone = null;
    public int MaxMessageLength = 10;
    private bool faded;
    public bool isDestroyWhenChangeScene = false;
    private float lastupdate;
    public ArrayList msgList = new ArrayList();

    // Use this for initialization
    /*void Start () {
	
	}*/
    //这里需要索引一个


    private void Awake()
    {
        if (onlyone != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        onlyone = gameObject;
        if (!isDestroyWhenChangeScene)
            DontDestroyOnLoad(gameObject);
        //this.pushMsg("Mac Address:" + BGTools.GetMacAddress());
    }

    // Update is called once per frame
    private void Update()
    {
        //print ((Time.time-lastupdate)>=20f);
        if (!faded && Time.time - lastupdate >= 20 && !faded)
        {
            gameObject.SetActive(false);
            faded = true;
        }
    }

    public void pushMsg(string inMsg)
    {
        gameObject.SetActive(true);
        if (msgList.Count >= MaxMessageLength)
        {
            msgList.RemoveAt(0);
        }
        msgList.Add(inMsg);
        lastupdate = Time.time;
        faded = false;
    }

    public void popMsg()
    {
        msgList.RemoveAt(0);
    }


    private void OnGUI()
    {
        for (int i = 0; i < msgList.Count; ++i)
        {
            string msg = msgList[i].ToString();
            //GUIStyle st=new GUIStyle();
            //st=GUI.skin.GetStyle();
            //st.font.name="STHeiti";
            //st.font.fontNames
            GUI.Label(new Rect(0, 0 + 20*i, ScreenTools.getLocalX(msg.Length*16), 20), msg);
        }
    }
}