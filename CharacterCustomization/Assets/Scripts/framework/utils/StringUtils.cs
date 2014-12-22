using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
public static class StringTools
{
    public static string urlDecode(string str)
    {
        return WWW.UnEscapeURL(str);
    }
	/// <summary>
        /// 中英文截取字符串类
        /// </summary>
        /// <param name="strinput"></param>
        /// <param name="strlen"></param>
        /// <returns></returns>
        public static string CutString(string strinput, int strlen)
        {
            strinput = strinput.Trim();
            Byte[] mybyte = System.Text.Encoding.Default.GetBytes(strinput);
            if (mybyte.Length > strlen)
            {
                String resultstr = "";
                for (int i = 0; i <= strinput.Length; i++)
                {
                    Byte[] tempByte = System.Text.Encoding.Default.GetBytes(resultstr);
                    if (tempByte.Length < strlen)
                    { resultstr += strinput.Substring(i, 1); }
                    else
                    { break; }
                }
                return resultstr + "...";
            }
            else
            {
                return strinput;
            }
        }

    public static string md5(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 =
            new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
	public static string EncryptVars(Hashtable vars,string privatekey){
		ArrayList keys=new ArrayList();
		foreach (DictionaryEntry kv  in vars)
        {
            	  keys.Add(kv.Key+"="+WWW.EscapeURL(kv.Value as string));
        }
		keys.Sort();
		string buildstr=string.Join("&",(string[])keys.ToArray(typeof(string)));
		buildstr+="&ssid="+StringTools.md5(buildstr.ToLower()+privatekey);
		return buildstr;
	}
	public static string EncryptLoginVars(Hashtable vars){
		
		ArrayList keys=new ArrayList();
		foreach (DictionaryEntry kv  in vars)
        {
            	  keys.Add(kv.Key+"="+WWW.EscapeURL(kv.Value as string));
        }
		keys.Sort();
		string buildstr=string.Join("&",(string[])keys.ToArray(typeof(string)));

#if UNITY_EDITOR||UNITY_STANDALONE_OSX||UNITY_IPHONE

		return buildstr+"&ssid="+happiLoginEncode(buildstr.ToLower());//TODO add happiLoginEncode function and open this annotation
#elif UNITY_ANDROID
	     try{
			AndroidJavaClass jc = new AndroidJavaClass("com.happiplay.tools.ExternalCall");
			return buildstr+"&ssid="+ jc.CallStatic<string>("happiLoginEncode",buildstr.ToLower()).ToString();
		}catch(Exception e){
			
		}

#endif
		return buildstr;

	}

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	[DllImport("LoginEncrypt")]
	private static extern string happiLoginEncode(string urlstring);
#elif UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern string happiLoginEncode(string urlstring);
#endif

	
}