//using System.Net.NetworkInformation;
using UnityEngine;
//using System.IO;

/**
 *BaoGameFramework 2012.10.3
 *工具函数，集合一些工具
 **/

public static class BGTools
{
    //Get Child GameObject  With Name
    public static GameObject FindChild(string rootObjectName, string childName)
    {
       var go = GameObject.Find(rootObjectName);
      if(go!=null)return FindChild(go,childName);
       return null;
    }

    public static GameObject FindChild(GameObject rootObject, string childName)
    {
		Transform t=rootObject.transform.FindChild(childName);
		if(t!=null)return t.gameObject;
        
        for(int i=0;i<rootObject.transform.childCount;i++)
        {
			Transform trs=rootObject.transform.GetChild(i);
          	if(trs.childCount>0){
				GameObject go=FindChild(trs.gameObject,childName);
				if(go!=null)return go;
				
			}
			
        }
        return null;
    }
	public static bool IsWiFI{get{
		return Application.internetReachability==NetworkReachability.ReachableViaLocalAreaNetwork;
		}}
	public static bool Is3G{get{
		return Application.internetReachability==NetworkReachability.ReachableViaCarrierDataNetwork;
		}}

    /// 返回pc，ipad,iphone,android
    public static string GetDeviceLoginModel()
    {
        string dn;
#if  UNITY_ANDROID
			dn="android";				
#else
        if (Base.IOS_IPAD || Base.PC)
        {
            dn = "ipad";
        }
        else if (Base.IOS_IPHONE)
        {
            dn = "iphone";
        }
        else
        {

            dn = "pc";
        }
#endif
        return dn;
    }

    public static string GetMacAddress()
    {
        string macAddress = "";
		/*
		try{
#if !UNITY_ANDROID
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adapter in nics)
        {
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                macAddress = adapter.GetPhysicalAddress().ToString();
                break;
            }
        }
#endif
		}catch(System.Exception e){
			
		}*/
        //need to xx:xx:xx:xx mode?
        return macAddress;
    }

    public static string GetDeviceID()
    {
#if UNITY_EDITOR || UNITY_IPHONE
		return SystemInfo.deviceUniqueIdentifier;
#elif UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.happiplay.tools.ExternalCall");
		return jc.CallStatic<string>("getDeviceID");
#else
		return SystemInfo.deviceUniqueIdentifier;
#endif
    }

    public static string GetDeviceName()
    {
#if !UNITY_ANDROID
        return SystemInfo.deviceName;
#endif
        return "";
    }

    public static string GetDeviceModel()
    {
        return SystemInfo.deviceModel;
    }

    public static string GetToken()
    {
#if UNITY_IOS
		if( UnityEngine.NotificationServices.deviceToken!=null){
				byte[] token  =UnityEngine. NotificationServices.deviceToken;
		        string hexToken =  System.BitConverter.ToString(token);//.Replace('-', '%');
				return  hexToken;
		}
#endif
        return "";
    }

	public static string GetSimType()
	{
#if UNITY_EDITOR
		return "mobile";
#elif UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.happiplay.tools.ExternalCall");
		return jc.CallStatic<string>("getSimType");
#else
		return "mobile";
#endif
	}

}