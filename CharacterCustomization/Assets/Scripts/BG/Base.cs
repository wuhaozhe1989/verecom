using UnityEngine;

public class Base : MonoBehaviour
{
    public static bool IOS_IPAD = false;
    public static bool IOS_IPHONE = false;
    public static bool PC = false;

    private static bool inited;

    private void Awake()
    {
        init();
    }

    public static void init()
    {
        if (inited) return;
        inited = true;
        if (BGTools.GetDeviceModel().ToLower().IndexOf("ipad") >= 0)
        {
            IOS_IPAD = true;
            IOS_IPHONE = false;
            PC = false;
        }
        else if (BGTools.GetDeviceModel().ToLower().IndexOf("iphone") >= 0)
        {
            IOS_IPAD = false;
            IOS_IPHONE = true;
            PC = false;
        }
        else
        {
#if IOS_IPHONE
				IOS_IPAD=false;
				IOS_IPHONE=true;
				PC=false;
#elif IOS_IPAD
				IOS_IPAD=true;
				IOS_IPHONE=false;
				PC=false;
#elif PC
				IOS_IPAD=false;
				IOS_IPHONE=false;
				PC=true;
#elif UNITY_IOS
			IOS_IPHONE=true;
#endif
        }
        processVerAndOtherConfig();
      //if(UICamera.current==null)  Global.goToLogin();
    }

    private static void processVerAndOtherConfig()
    { 
#if UNITY_ANDROID
			
			Configs.version="1.0.5";
			Configs.devicetype="android";
			Configs.bundleid="com.happiplay.texas";
			Configs.downloadURL=Configs.HttpServerUrl+"/mobile/download/android.php";
#else
		/*	Configs.version="1.0.5";
			Configs.devicetype="iphone";
			Configs.bundleid="com.happiplay.texas";
			Configs.downloadURL=Configs.HttpServerUrl+"/mobile/download/ios.php?devicetype=iphone";*/

			
#endif
	 #if !UNITY_EDITOR

        Configs.bundleid = ExternalInterface.BundleID;

    #endif
    }


    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}