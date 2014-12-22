using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public class BGSocketNew2
{
	public BGISocket bgISocket = null;
	public int id;	
	public string socketName = "";
	
	
	
	private static int socketid; 
	private readonly byte[] receiveBuffer = new byte[1024];
	private bool connected=false;    
	private int sendfail;
	private MnaSocket socket;
	private Thread socketReceiveThread;
	public BGSocketState state = BGSocketState.idle;
	//private ByteStream stream = new ByteStream();
	private int zerotimes;
	
	public BGSocketNew2(BGISocket inBGISocket)
	{
		++socketid;
	 
		bgISocket = inBGISocket;
		socket = new MnaSocket(); 
		int initCode = socket.Init("14",Application.persistentDataPath);
		id = socketid;
	}
	
	public bool Connected
	{
		get
		{ 
			return connected;
		}
	}
	
	public void release()
	{
		socket = null;
		bgISocket = null;
		socketReceiveThread = null;
	}
	
	/// <summary>
	///     连接远程服务器
	///     <para>
	///         connect the remote serverr
	///     </para>
	/// </summary>
	/// <param name="ip">
	///     服务器IP
	///     IP address of the server
	/// </param>
	/// <param name="port">
	///     端口号
	///     port
	/// </param>
	public void connect(string ipAddress, int port)
	{
		zerotimes = 0; 
		state=BGSocketState.connecting;
		IAsyncResult result = socket.BeginConnect(ipAddress,port,
		                                         (int)( BGTools.Is3G?MnaConst.ENUM_MNA_NETTYPE.E_MNA_NETTYPE_3G:MnaConst.ENUM_MNA_NETTYPE.E_MNA_NETTYPE_WIFI),
		                                        (int)(  MnaConst.ENUM_MNA_MOBILE_ISP.E_MNA_MOBILE_UNKNOWN),5000,new AsyncCallback(connectCallback),null);
		//这里做一个超时的监测，当连接超过5秒还没成功表示超时   
		
	}
	
	private void connectCallback(IAsyncResult asyncConnect)
	{
		Debug.Log("connectResultBack");
		try{
			int code=socket.EndConnect(asyncConnect);
			if(code==0){

				Debug.Log("connectSuccess!!!!!!");
				connected = true;
				state = BGSocketState.connected;
				socketAbort=false; 
//				asyncReceive();
//				FrameDelayCall.Add(()=>{
					socketReceiveThread = new Thread(ReceiveSocket);
					socketReceiveThread.IsBackground = true;
					socketReceiveThread.Start();
//				},1);
			}else{
				state=BGSocketState.connectFail;
				disconnect();
				connected=false;
			}
			


			
		}catch(Exception e){
			Debug.LogException(e);
			Debug.Log("connect Time Out");
			state=BGSocketState.connectFail; 
			disconnect();
		}
		
	}
	void asyncReceive(){

		socket.BeginReceive(buffer,1024,new AsyncCallback(recved),null);
	}
	static byte[] buffer = new byte[1024];
	void recved(IAsyncResult ar) {
		try
		{
		
			int BytesRead = socket.EndReceive(ar);
			if (BytesRead > 0)
			{
				byte[] tmp = new byte[BytesRead];
				Array.ConstrainedCopy(buffer, 0, tmp, 0, BytesRead);
				if (bgISocket != null)
				{
					bgISocket.receiveData(tmp);
				}
				asyncReceive();
			}
			else//对端gracefully关闭一个连接
			{
				disconnect();
			}

		}
		catch (Exception ex)
		{
			disconnect();
		}
	}
	
	
	bool socketAbort=false;
	private void ReceiveSocket()
	{
		Thread.Sleep(100);
		//在这个线程中接受服务器返回的数据   
		while (true)
		{
			//			Thread.Sleep(100);
			if(socketAbort)break;
			if (!connected)
			{
				//与服务器断开连接跳出循环   
				Debug.Log("Failed to clientSocket server.");
				//socket.Close();
				//disconnect();
				state = BGSocketState.disconnected;
				
				break;
			}
			try
			{
				//接受数据保存至bytes当中   
				var tbytes = new byte[16384];
				//Receive方法中会一直等待服务端回发消息   
				//如果没有回发会一直在这里等着。   
				int i = socket.Receive(tbytes,16384);
				//Debug.Log(i);
				if (i <= 0)
				{
					Debug.Log("Thread receive i invalid.");
					disconnect();
					//socket.Close();   
					break;
				}
				
				var dataBuffer = new byte[i];
				
				Array.Copy(tbytes, 0, dataBuffer, 0, i);
				
				
				bgISocket.receiveData(dataBuffer);
				
				//GC.Collect();
			}
			catch (Exception e)
			{
				Debug.LogWarning("Failed to clientSocket error." + e);
				
				disconnect();
				break;
			}
		}
	}
 
	public void onUpdate(float delta)
	{
		 
		if (state > BGSocketState.idle)
		{
			Debug.Log(state);
			switch (state)
			{
			case BGSocketState.connected:
				connected = true;
				bgISocket.socketConnected();
				break;
			case BGSocketState.connectFail:
			case BGSocketState.disconnected:
				Debug.Log("#@@$$BGSocketState.disconnected");
				bgISocket.scoketDiconnected();
				
				break;
			case BGSocketState.received:
				break;
			}
			state = BGSocketState.idle;
			if (connected && connected != Connected)
			{
				Debug.LogWarning("Where socket disconned not send by function???");
				if (bgISocket != null) bgISocket.scoketDiconnected();
			}
		}
	}
	
	/// <summary>
	///     向连接服务器发送消息
	///     send message to the remote server
	/// </summary>
	/// <param name="buffer">
	///     消息的字节流
	///     message bytes
	/// </param> 
	
	//send data 
	public void sendData(byte[] data)
	{
		if (socket != null && connected)
		{
			IAsyncResult asyncSend = socket.BeginSend(data,  data.Length, new AsyncCallback(sendCallback), socket);

			bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
			if (!success)
			{
				socket.Close();
				Debug.Log("Failed to SendMessage server.");
				sendfail++;
				if (sendfail >= 1)
				{
					Debug.Log("SendFail Disconnect");
					disconnect();
				}
			}
			else
			{
				sendfail = 0;
			}
		}
	}
	
	private void sendCallback(IAsyncResult asyncSend)
	{
		socket.EndSend(asyncSend);
	}
	
	private void sendArg_Completed(object sender, SocketAsyncEventArgs e)
	{
		//Debug.Log(e);
		Debug.Log(string.Format("send::{2}::sucess:{0},len:{1}", e.SocketError == SocketError.Success, e, id));
		if (e.SocketError != SocketError.Success)
		{
			Debug.LogWarning(e.ToString());
			sendfail++;
			if (sendfail >= 1)
			{
				Debug.Log("SendFail Disconnect");
				disconnect();
			}
		}
		else
		{
			sendfail = 0;
		}
	}
	
	public void disconnect()
	{
		Debug.Log("please disconnect!!!!!!!!!!!!!!!!!!!!!!!!!!@@@@@@");

		if (connected)
		{
			connected = false;
			state = BGSocketState.disconnected;
			try
			{
				if (socketReceiveThread != null)
				{
					if(socketReceiveThread.ThreadState!=System.Threading.ThreadState.Aborted){
						socketAbort=true;
						socketReceiveThread.Abort();
						socketReceiveThread.Join();
						
					}
					//socketReceiveThread = null;
				}
				
				Closed();
			}
			catch (Exception e)
			{
				Debug.LogWarning("Close error:::" + e);
			}
		}
	}
	
	
	//关闭Socket   
	public void Closed()
	{
		if (socket != null )
		{
			//socket.Shutdown(SocketShutdown.Both);   
			socket.Close();
		}
		socket = null;
	}
	
	
	public void sendString(string strData)
	{
		try
		{
			byte[] convertData = Encoding.UTF8.GetBytes(strData);
			sendData(convertData);
		}
		catch (Exception e)
		{
			Debug.LogWarning(e.ToString());
		}
	}
	
}
