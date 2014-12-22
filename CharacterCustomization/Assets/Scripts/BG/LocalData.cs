using UnityEngine;
using System.Collections;
using System.IO;
    
using System;
public class LocalData
{
	public string file;
	FileInfo t ;
	FileStream sw;
	public LocalData(string filePathAndName,bool hash=true){
		if(hash){
			file=StringTools.md5(filePathAndName);
		}else{
			
			file=filePathAndName;
		} 
       	t = new FileInfo(Path.Combine (prePath, file));
		_isFileExist=t.Exists;
	} 
    public static string prePath

    {

        get
        {

            string    path = null;

            if (Application.platform == RuntimePlatform.IPhonePlayer)

            {

                path = Application.dataPath.Substring (0, Application.dataPath.Length - 5);

                path = path.Substring(0, path.LastIndexOf('/')) + "/Documents";

            }else if(Application.platform==RuntimePlatform.Android){
				
				path = Application.persistentDataPath+ "/";
				path=path.Substring(0, path.LastIndexOf('/')) ;
			}  else

            {
#if UNITY_EDITOR
				path = Application.dataPath + "/../cache"; 
				
#endif
				

            }
			if(!dirCheck){
				dirCheck=true;
				if(!Directory.Exists(path))
				Directory.CreateDirectory(path);
			}
			
            return path;

        }

    }
	static bool dirCheck=false;
	bool _isFileExist;
	public bool isFileExist{
		
	get{
		return _isFileExist;
		}
	}
	public byte[] ReadFile(){
		try{
			 if (!_isFileExist) {
	           return null;
	        } else {
	          //  sw = t.Open(FileMode.OpenOrCreate);

	        }
			//Debug.LogWarning(sw.Length);
			byte[] obj=new byte[sw.Length];
			sw.Read(obj,0,int.Parse(sw.Length.ToString()));
			sw.Close();
			return obj;
		}catch(Exception e){
			Debug.LogException(e);
		}
		return null;
	}
    public void Delete(){
		if(_isFileExist){
		//	t.Delete();	
		}
		
	}
   public void  SaveFile(byte[] data ){

		if(data==null||data.Length==0)return;
		try{
	        if (!_isFileExist) {
	           // sw = t.Create();
				_isFileExist=true;
	        } else {
				//t.Delete();
	            //sw = t.Create();
				//sw=t.Open(FileMode.Truncate);

	        }
		//sw.BeginWrite(data,0,data.Length,writeFinish,1);

	        sw.Write(data,0,data.Length); 
			sw.Close();
		}catch(Exception e){
			Debug.LogException(e);
		}
    }
	public void AppendSaveFile(byte[] data){
		try{
		if(!_isFileExist){
			//sw=t.Create();
			_isFileExist=true;
		}else{
			//sw=t.Open(FileMode.Append);
		}
		
		sw.Write(data,Convert.ToInt32(sw.Length),data.Length);
		sw.Close();
		}catch(Exception e){
			Debug.LogException(e);
		}
	}
	private void writeFinish(IAsyncResult writeResult)
    {
	}
	public void Dispose(){
		
		if(sw!=null)sw.Dispose();	
		
	}
	 
}

