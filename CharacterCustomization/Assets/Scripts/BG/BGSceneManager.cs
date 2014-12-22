using System.Collections;
using UnityEngine;

/**
 *BaoGameFramework 2012.10.3
 *用于其他线程的场景切换,base.Satrt(),base.Update().
 **/

[AddComponentMenu("BaoGameFrameWork/BGSceneManager")]
public class BGSceneManager : MonoBehaviour
{
    private readonly ArrayList stackScenesName = new ArrayList();
    private string wantToSceneName;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    private void Update()
    {
        if (wantToSceneName != null)
        {
            Application.LoadLevel(wantToSceneName);
            wantToSceneName = null;
        }
    }

    //直接进入场景
    public void toScene(string sceneName)
    {
        wantToSceneName = sceneName;
    }


    //push/pop操作场景切换
    public void pushScene(string sceneName)
    {
        wantToSceneName = sceneName;
        stackScenesName.Add(wantToSceneName);
    }

    public void popScene()
    {
        if (stackScenesName.Count > 0)
        {
            wantToSceneName = (string) stackScenesName[stackScenesName.Count - 1];
            stackScenesName.RemoveAt(stackScenesName.Count - 1);
        }
    }
}