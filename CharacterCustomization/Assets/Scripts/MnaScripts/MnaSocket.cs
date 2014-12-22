using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Runtime.Remoting.Messaging;


using System.Net.Sockets;
using System.Net;


public class MnaSocket  {
	private static string VERSION = "1.0.0";

	IntPtr instance = IntPtr.Zero;
		
    #if  UNITY_EDITOR
	  // editor 下使用系统API 不提供加速功能，方便开发商测试
	  private Socket m_kClientSocket = null;
	#elif UNITY_IPHONE
	
         [DllImport ("__Internal")]
	    private static extern IntPtr createMnaInstance();
		
		[DllImport ("__Internal")]
	    private static extern int initMna(IntPtr instance,int os , string osVer, string dataPath);
		
		[DllImport ("__Internal")]
	     private static extern int mnaConnect(IntPtr instance, string domain, int port, int netType, int sp, int timeout);
		
		[DllImport ("__Internal")]
	     private static extern int mnaRecv(IntPtr instance,int bufSize,int flag,IntPtr buffer);
		
		[DllImport ("__Internal")]
	     private static extern int mnaSend(IntPtr instance, IntPtr  buffer, int bufSize,int flag);
		
		[DllImport ("__Internal")]
	     private static extern int closeMnaCon(IntPtr instance);
		
	    [DllImport ("__Internal")]
	     private static extern bool isMnaConnected(IntPtr instance);
	
        [DllImport ("__Internal")]
         private static extern int getMnaUrl( string strReqURL,int eNetType, int eISP,
		                                     StringBuilder strSpeedURL, ref int speedURLSize,ref int usPort, 
		                                     StringBuilder strHost,ref int hostSize);
	
	    [DllImport ("__Internal")]
         private static extern int HttpStatReport( string  strDomain,  int uiMS,  int iSendLen,
		                                           int iRecvLen, bool bFail = false);
     #elif UNITY_ANDROID
	     
		//引用android so中的方法
		
		[DllImport ("qcloud_mna_cs")]
	    private static extern IntPtr createMnaInstance();
		
		[DllImport ("qcloud_mna_cs")]
	    private static extern int initMna(IntPtr instance,int os , string osVer, string dataPath);
		
		[DllImport ("qcloud_mna_cs")]
	     private static extern int mnaConnect(IntPtr instance, string domain, int port, int netType, int sp, int timeout);
		
		[DllImport ("qcloud_mna_cs")]
	     private static extern int mnaRecv(IntPtr instance,int bufSize,int flag,IntPtr buffer);
		
		[DllImport ("qcloud_mna_cs")]
	     private static extern int mnaSend(IntPtr instance, IntPtr  buffer, int bufSize,int flag);
		
		[DllImport ("qcloud_mna_cs")]
	     private static extern int closeMnaCon(IntPtr instance);
	
		[DllImport ("qcloud_mna_cs")]
	     private static extern bool isMnaConnected(IntPtr instance);
		
		[DllImport ("qcloud_mna_cs")]
	     private static extern int getMnaFn(IntPtr instance);
	
	    [DllImport ("qcloud_mna_cs")]
         private static extern int getMnaUrl( string strReqURL,int eNetType, int eISP,
		                                     StringBuilder strSpeedURL, ref int speedURLSize,ref int usPort, 
		                                     StringBuilder strHost,ref int hostSize);
	
	    [DllImport ("qcloud_mna_cs")]
         private static extern int HttpStatReport( string  strDomain,  int uiMS,  int iSendLen,
		                                           int iRecvLen, bool bFail = false);
	

    #endif
	

	 #if  UNITY_EDITOR
	    // editor 下使用系统API 不提供加速功能，方便开发商测试
	
	 	 public MnaSocket(){
	         m_kClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		
		 }
	     public int Init(string osVersion, string storePath){
			 return 0;
	     }

		 public int Connect(string domain, int port,int netWork, int spType,int timeOut){
		   //  IPAddress ip = IPAddress.Parse(domain);
			// IPEndPoint kAddress = new IPEndPoint( ip, port);
		      m_kClientSocket.Connect(domain,port);
			  return 0;
		 } 
		 public IAsyncResult  BeginConnect(string domain, int port,int netWork, int spType,int timeOut,AsyncCallback callback,System.Object  state){
			   if(domain==null||domain.Length<=0)
				   throw new ArgumentException ("The length of the domain  should not be null");
			   if (port <= 0 || port > 65535)
				   throw new ArgumentOutOfRangeException ("port", "Must be > 0 and < 65536");
		    // IPAddress ip = IPAddress.Parse(domain);
			// IPEndPoint kAddress = new IPEndPoint( ip, port);
		    return  m_kClientSocket.BeginConnect(domain,port, callback, null); //建立异步连接服务 , Connected 进行监听
	     }
	     public int EndConnect(IAsyncResult iar){
		        m_kClientSocket.EndConnect(iar);
		       	return 0;
	     }

		public int Send(byte[] bytes, Int32 length ){
		 	
		        int sendCode = m_kClientSocket.Send(bytes,length,SocketFlags.None);
		        return sendCode;
		}
	

	    public IAsyncResult  BeginSend (byte[] bytes, Int32 length,AsyncCallback callback,System.Object  state){
		    return  m_kClientSocket.BeginSend(bytes, 0, length, SocketFlags.None, callback, m_kClientSocket); 
		}

	    public int EndSend(IAsyncResult iar){
		        return m_kClientSocket.EndSend(iar);
		 }

	     public int Receive(byte[] buffer,	int size){


			    int recCode = m_kClientSocket.Receive(buffer,size,SocketFlags.None);
		        return recCode;
	     }

	     public IAsyncResult  BeginReceive (byte[] buffer, Int32 size,AsyncCallback callback,System.Object  state){
		     return  m_kClientSocket.BeginReceive(buffer, 0, size, SocketFlags.None, callback, m_kClientSocket); 

		 }

		 public int EndReceive(IAsyncResult iar){
              return   m_kClientSocket.EndReceive(iar);
		    
		 }
	     public bool isConnected(){
		        return m_kClientSocket.Connected;
	     }
	     public int Close(){
		        m_kClientSocket.Close();
		        return 0;
	     }
	    /* 
		 * edtor 下不提供，只是填充接口
		 * */
	     public  int getBetterUrl( string strReqURL,int eNetType, int eISP,
		                                StringBuilder strSpeedURL,ref int speedURLSize, ref int usPort, 
		                                StringBuilder strHost,ref int hostSize){
                  return 0 ;
	     }
		/* 
		 * edtor 下不提供，只是填充接口
		 * */
	     public  int HttpReport(string  strDomain,  int uiMS,  int iSendLen,
		                                           int iRecvLen, bool bFail = false){
		       return 0 ;
	     }
				
	#elif UNITY_ANDROID || UNITY_IPHONE
	   /**
	     * 构造方法
	     */
		public MnaSocket(){
			instance = createMnaInstance();
			Debug.Log("istance="+instance);
			if(instance==IntPtr.Zero){
			throw new ArgumentNullException ("MnaSocket : createMnaInstance failed");
			}
		}
	
	    public int Init(string osVersion, string storePath){
			   if(osVersion==null||osVersion.Length<=0){
			 		Debug.Log("MnaSocket : osVersion should not be null");
					throw new ArgumentNullException ("osVersion");
				 }
				if(storePath==null||storePath.Length<=0){
					Debug.Log("MnaSocket : storePath should not be null");
					throw new ArgumentNullException ("storePath");
				 }
				
		        #if UNITY_IPHONE
				int initCode =  initMna(instance,(int)MnaConst.ENUM_MNA_MOBILE_OS.E_MNA_OS_U3D_IOS, osVersion, storePath);
                #elif UNITY_ANDROID
		        int initCode =  initMna(instance,(int)MnaConst.ENUM_MNA_MOBILE_OS.E_MNA_OS_U3D_ANDRIOD, osVersion, storePath); 
                #endif
				if(initCode!=0){
				  throw new ArgumentException ("MnaSocket: init failed");
				}
		        return initCode;
		
	     }
	    /* 
		 * 为实现异步调用的委托
		 * */
		public delegate int DelegateForConnect(string domain, int port , int netWork,int spType, int timeOut);
		public delegate int DelegateForSend(byte[] bytes, Int32 length );
		public delegate int DelegateForReceive(byte[] buffer,	int size);
		
	  /**
	   *  同步连接
	   */
	 	 public int Connect(string domain, int port,int netWork, int spType,int timeOut){
		   int connectCode =  mnaConnect(instance, domain, port, netWork, spType, timeOut);
		   return connectCode;
		 } 
		 
	  /**
	   *  异步开始连接
	   */
	     public IAsyncResult  BeginConnect(string domain, int port,int netWork, int spType,int timeOut,AsyncCallback callback,System.Object  state){
			   if(domain==null||domain.Length<=0)
				   throw new ArgumentException ("The length of the domain  should not be null");
			   if (port <= 0 || port > 65535)
				   throw new ArgumentOutOfRangeException ("port", "Must be > 0 and < 65536");
		       DelegateForConnect delCon  = new  DelegateForConnect(Connect);
		       return delCon.BeginInvoke(domain, port,netWork , spType ,timeOut,callback, state);
	     }
	  /**
	   *  异步结束连接
	   */
	     public int EndConnect(IAsyncResult iar){
		       DelegateForConnect delCon = (iar as AsyncResult).AsyncDelegate as  DelegateForConnect;
		        int result = -1;
			     try {
			        result = delCon.EndInvoke(iar);		       
			    }
			    catch( Exception ex ) {		       
				    Debug.LogError(ex);
		        }
		       	return result;
	     }
	
	  /**
	   *  同步发送
	   */
		public int Send(byte[] bytes, Int32 length ){
		 		GCHandle hObject = GCHandle.Alloc(bytes, GCHandleType.Pinned);
          		IntPtr pObject = hObject.AddrOfPinnedObject();
				if(hObject.IsAllocated)
				    hObject.Free();
		        int sendCode = mnaSend (instance,pObject,(int)length,0);
		        return sendCode;
		}
	
	  /**
	   *  异步开始发送
	   */
	    public IAsyncResult  BeginSend (byte[] bytes, Int32 length,AsyncCallback callback,System.Object  state){
		        DelegateForSend delSend = new DelegateForSend(Send);
		       return delSend.BeginInvoke(bytes, length,callback, state);
		}
	  /**
	   *  异步结束发送
	   */
	    public int EndSend(IAsyncResult iar){
		        DelegateForSend delSend = (iar as AsyncResult).AsyncDelegate as DelegateForSend;
		        int result = -1;
			     try {
			        result = delSend.EndInvoke(iar);		       
			    }
			    catch( Exception ex ) {		       
				    Debug.LogError(ex);
		        }
		     	return result;
		 }
	  /**
	   *  同步接收
	   */
	     public int Receive(byte[] buffer,	int size){
		        GCHandle gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                IntPtr recPtr = gcHandle.AddrOfPinnedObject();
				if(gcHandle.IsAllocated)
				    gcHandle.Free();

			    int recCode = mnaRecv( instance, size, 0,recPtr);
		        return recCode;
	     }
	
	  /**
	   *  异步开始接收
	   */
	     public IAsyncResult  BeginReceive (byte[] buffer, Int32 size,AsyncCallback callback,System.Object  state){
		       DelegateForReceive delReceive = new DelegateForReceive(Receive);
		       return delReceive.BeginInvoke(buffer, size,callback, state);
		 }
	  /**
	   *  异步结束接收
	   */
		 public int EndReceive(IAsyncResult iar){
		        DelegateForReceive delReceive   = (iar as AsyncResult).AsyncDelegate as DelegateForReceive;
		        int result = -1;
			    try {
			        result = delReceive.EndInvoke(iar);		       
			    }
			    catch( Exception ex ) {		       
				    Debug.LogError(ex);
		        }
		        return result;
		 }
	  /**
	   *  shifoulianjie
	   */
	     public bool isConnected(){
		        return isMnaConnected(instance);
	     }
	  /**
	   *  断开连接
	   */
	     public int Close(){
		        int closeCode = closeMnaCon(instance);
		        return closeCode;
	     }
	
	     public  int getBetterUrl( string strReqURL,int eNetType, int eISP,
		                                StringBuilder strSpeedURL,ref int speedURLSize, ref int usPort, 
		                                StringBuilder strHost,ref int hostSize){
		        return getMnaUrl(strReqURL,eNetType,eISP,strSpeedURL,ref speedURLSize,ref usPort,strHost,ref hostSize);
	     }
	
	     public  int HttpReport(string  strDomain,  int uiMS,  int iSendLen,
		                                           int iRecvLen, bool bFail = false){
		        return HttpStatReport(strDomain,uiMS,iSendLen,iRecvLen,bFail);
	     }
	   
	
     #endif
	
	


}
