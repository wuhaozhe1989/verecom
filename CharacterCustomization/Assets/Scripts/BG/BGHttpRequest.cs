using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

//using Warensoft.Unity.Communication.Client;
/**
 *封装了网络请求，可以设置time out时间
**/

public interface BGIRequestResult
{
    void requestSuccessed(string responeStr, int requestTag);
    void requestSuccessedTexture(Texture texture, int requestTag);
    void requestFail(int requestTag);
}

public delegate void HttpRequestCallBack(object data);

public class SimpleHttpResult : BGIRequestResult
{
    private HttpRequestCallBack callback;

    public SimpleHttpResult(HttpRequestCallBack _callback)
    {
        callback = _callback;
    }

    void BGIRequestResult.requestSuccessed(string responeStr, int requestTag)
    {
        if(callback!=null)callback(responeStr);
    }

    void BGIRequestResult.requestSuccessedTexture(Texture texture, int requestTag)
    {
        if(callback!=null)callback(texture);
    }

    void BGIRequestResult.requestFail(int requestTag)
    {
        if(callback!=null)callback(null);
    }
}

public class BGHttpRequest
{
    private struct RequestTask
    {
        public BGIRequestResult requestResult;
        public RequestType inType;
        public string url;
        public float inHttpTimeOut;
        public int tag;
        public int taskid;
    }

    //用于加载update的gameobject
    private static GameObject mInstance = null;
    //我们用来限制下同时开工的队列
    public static int ListLimit = 20;
    //队列
    private static BetterList<BGHttpRequest> requests = new BetterList<BGHttpRequest>();
    private static BetterList<RequestTask> tasks = new BetterList<RequestTask>();

    public static void cancelRequest(int handlerid)
    {
        for (int i = 0; i < tasks.size; i++)
        {
            if (tasks[i].taskid == handlerid)
            {
                tasks.RemoveAt(i);
                return;
            }
        }
        for (int i = 0; i < requests.size; i++)
        {
            if (requests[i].taskid == handlerid)
            {
                requests[i].requestBody = null;
                if (requests[i].client != null) requests[i].client.CancelAsync();
                requests[i].dispose();
                return;
            }
        }
    }

	public static int simpleRequest(HttpRequestCallBack callback, RequestType inType, string url, bool nolimit = false, bool noCache = false)
    {
        return newRequest(new SimpleHttpResult(callback), inType, url,0,20,nolimit,noCache);
    }

    private static int reqid = 0;
    //ToDo 之后需要这里添加判别回复类型，现在默认为文字
    public static int newRequest(BGIRequestResult requestResult, RequestType inType, string url, int tag = 0,
                                 float inHttpTimeOut = 20f, bool nolimit = false, bool noCache = false)
    {
        reqid++;
        //Debug.Log(url);
        if (mInstance == null)
        {
            mInstance = new GameObject("_BGHTTP_AGENT");
            GameObject.DontDestroyOnLoad(mInstance);
            UpdateManager.AddLateUpdate(null, 0, OnUpdate);
        }

        //构造requestResult
        if (requestResult == null)
        {
        }

        //队列尚有空余加之
        //if(requests.size<ListLimit)mInstance = go.AddComponent<BGHttpRequest>();
        if (noCache)
        {
            removeCache(url);
        }
        else if (inType == RequestType.IMAGE)
        {

           byte[] res = getCache(url);
           if (res != null)
           {
               //MonoBehaviour.print("cached::::"+url);
               Texture2D imgTexture = new Texture2D(256, 256,TextureFormat.ARGB32,false);
               // imgTexture.anisoLevel = 9;
               imgTexture.wrapMode = TextureWrapMode.Clamp;
               // imgTexture.filterMode = FilterMode.Trilinear;
               // imgTexture.anisoLevel = 3;
               imgTexture.LoadImage(res);
               requestResult.requestSuccessedTexture(imgTexture, tag);
               return reqid;
           }
           // DownloadImgTest.DownloadTexture(requestResult,url,tag);
            // GameObject.Find("LoginSceneManager").GetComponent<LoginSceneManager>().DownloadTexture(requestResult,url,tag);
            // return reqid;
        }


        if (inType == RequestType.TEXT) nolimit = true;

        //add new
        if (requests.size < ListLimit || nolimit)
        {
            BGHttpRequest request = new BGHttpRequest(requestResult, inHttpTimeOut, inType);
            request.requestURLN(url, tag);
            request.taskid = reqid;
            requests.Add(request);
        }
        else
        {
            RequestTask task = new RequestTask();
            task.inHttpTimeOut = inHttpTimeOut;
            task.inType = inType;
            task.requestResult = requestResult;
            task.tag = tag;
            task.url = url;
            task.taskid = reqid;
            tasks.Add(task);
        }
        return reqid;
    }

    private static void OnUpdate(float delta)
    {
        //Debug.Log("gogogoog");
        if (Time.frameCount%5 != 1) return;
        bool needreduce = false;
        if (requests.size > ListLimit)
        {
            needreduce = true;
        }
        foreach (BGHttpRequest request in requests)
        {
            if (request.idle)
            {
                if (needreduce)
                {
                    request.dispose();
                    requests.Remove(request);
                    continue;
                }
                if (tasks.size > 0)
                {
                    RequestTask task = tasks[0];

                    request.requestBody = task.requestResult;
                    request.type = task.inType;
                    request.taskid = task.taskid;
                    request.httpTimeOut = task.inHttpTimeOut;
                    request.requestSuccessed = task.requestResult.requestSuccessed;
                    request.requestSuccessedTexture = task.requestResult.requestSuccessedTexture;

                    request.requestFail = task.requestResult.requestFail;
                    request.requestURLN(task.url, task.tag);

                    tasks.Remove(task);
                }
            }
            else
            {
                request.check(delta);
            }
        }
    }

    //private static Dictionary<string,byte[]>AssetCache=new Dictionary<string, byte[]>();
    private static BetterList<string> AssetCacheURL = new BetterList<string>();
    private static BetterList<LocalData> AssetCacheDATA = new BetterList<LocalData>();

    public static byte[] getCache(string url)
    {
        if (AssetCacheURL.Contains(url))
        {
            int i = AssetCacheURL.IndexOf(url);
            return AssetCacheDATA[i].ReadFile();
        }
        //if(AssetCache.ContainsKey(url)){
        //  return AssetCache[url];
        //}
        return null;
    }

    public static void removeCache(string url)
    {
        if (AssetCacheURL.Contains(url))
        {
            int i = AssetCacheURL.IndexOf(url);
            AssetCacheDATA.RemoveAt(i);
            AssetCacheURL.RemoveAt(i);
        }
    }
	static int i=0;
    public static void saveCache(string url, byte[] data)
    {
		
        if (!AssetCacheURL.Contains(url))
        {
            AssetCacheURL.Add(url);
			var file=new LocalData("cacheFile"+i,false);
			file.SaveFile(data);
			//file.Dispose();
			//file=new LocalData("cacheFile"+i,false);
            AssetCacheDATA.Add(file);
            if (AssetCacheURL.size > 50)
            {
                AssetCacheURL.RemoveAt(0);
                AssetCacheDATA.RemoveAt(0);
            }
			i++;
			i%=50;
        }
    }

    public enum RequestType
    {
        TEXT, //请求的文字
        IMAGE //请求的图片
        ,
        BYTEARRAY
    };

    private enum ClientState
    {
        init = 0,
        fail = 1,
        jump = 2,
        success = 3
    }

    public int taskid;

    public delegate void RequestSuccessed(string responeStr, int requestTag);

    public RequestSuccessed requestSuccessed;

    public delegate void RequestSuccessedTexture(Texture texture, int requestTag);

    public RequestSuccessedTexture requestSuccessedTexture;

    public delegate void RequestFail(int requestTag);

    public RequestFail requestFail;

    protected float httpTimeOut = 5.0f; //http connect time 
    protected RequestType type = RequestType.TEXT;

    protected BGIRequestResult requestBody;

    public BGHttpRequest(BGIRequestResult requestResult)
    {
        init(requestResult, httpTimeOut, type);
    }

    public BGHttpRequest(BGIRequestResult requestResult, float inHttpTimeOut)
    {
        init(requestResult, inHttpTimeOut, type);
    }

    //ToDo 之后需要这里添加判别回复类型，现在默认为文字
    public BGHttpRequest(BGIRequestResult requestResult, RequestType inType)
    {
        init(requestResult, httpTimeOut, inType);
    }

    //ToDo 之后需要这里添加判别回复类型，现在默认为文字
    public BGHttpRequest(BGIRequestResult requestResult, float inHttpTimeOut, RequestType inType)
    {
        init(requestResult, inHttpTimeOut, inType);
    }

    private void init(BGIRequestResult requestResult, float inHttpTimeOut, RequestType inType)
    {
        requestBody = requestResult;
        type = inType;
        httpTimeOut = inHttpTimeOut;
        requestSuccessed = requestResult.requestSuccessed;
        requestSuccessedTexture = requestResult.requestSuccessedTexture;

        requestFail = requestResult.requestFail;
    }

    public bool idle = true;

    public void dispose()
    {
        if (client != null) client.Dispose();
        resultarr = null;
        client = null;
        idle = true;
        requestBody = null;
        requestFail = null;
        requestSuccessedTexture = null;
        requestSuccessed = null;
    }

    private int requestTag;

    private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
    {
        //Debug.Log(e);
        if (e.Cancelled)
        {
            isFinish = true;
            state = ClientState.fail;
        }
        else
        {
            if (e.Result != null)
            {
                isFinish = true;
                resultarr = e.Result;
                state = ClientState.success;
            }
        }
    }

    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        //MonoBehaviour.print("loading:"+e.ProgressPercentage);
    }

    private void check(float delta)
    {
        tempTime += delta;
        //Debug.Log(tempTime);
        if (isFinish || tempTime > httpTimeOut)
        {
            if (requestBody == null)
            {
                dispose();
                return;
            }
            if (requestBody is MonoBehaviour)
            {
                MonoBehaviour mo = requestBody as MonoBehaviour;
                if (mo == null || mo.gameObject == null || !mo.enabled)
                {
                    dispose();
                    return;
                }
            }
            if (state == ClientState.fail)
            {
                requestFail(requestTag);
            }
            else if (state == ClientState.jump)
            {
            }
            else if (state == ClientState.success)
            {
                //              MonoBehaviour.print("yes byte:::"+resultarr);
                try
                {
                    if (resultarr == null)
                    {
                        //requestFail(requestTag);
                    }
                    else if (type == RequestType.BYTEARRAY)
                    {
                     /*   FluorineFx.AMF3.ByteArray ba = new FluorineFx.AMF3.ByteArray();
                        ba.WriteBytes(resultarr, 0, resultarr.Length);
                        ba.Uncompress();

                        string dataStr = System.Text.Encoding.UTF8.GetString(ba.ToArray());
                        ba = null;
                        requestSuccessed(dataStr, requestTag);*/
                    }
                    else if (type == RequestType.TEXT)
                    {
                        //  MonoBehaviour.print("oooooo");
                        string dataStr = System.Text.Encoding.UTF8.GetString(resultarr);
                        requestSuccessed(dataStr, requestTag);
                    }
                    else if (type == RequestType.IMAGE)
                    {
                        saveCache(requesturl, resultarr); 
						
                        Texture2D imgTexture = new Texture2D(256, 256,TextureFormat.ARGB32,false);
						
                        imgTexture.wrapMode = TextureWrapMode.Clamp;
                        imgTexture.LoadImage(getCache(requesturl));
                        // imgTexture.anisoLevel = 9;
                        // imgTexture.filterMode = FilterMode.Trilinear;

                        requestSuccessedTexture(imgTexture, requestTag);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                Debug.LogWarning("http time out:" + requesturl);
                requestFail(requestTag);
            }
            dispose();
        }
    }

    private WebClient client;
    private ClientState state;
    private float tempTime = 0;
    private bool isFinish = false;
    private byte[] resultarr = null;
    private string requesturl = null;

    public void requestURLN(string url, int tag)
    {
        idle = false;
        requestTag = tag;
        isFinish = false;
        requesturl = url;
        state = ClientState.init;
        client = new WebClient();
//		client.Headers.Add("Accept-Encoding: gzip, deflate");
        client.DownloadDataCompleted += DownloadDataCompleted;
        client.DownloadProgressChanged += DownloadProgressChanged;
        client.DownloadDataAsync(new System.Uri(url));
    }
}