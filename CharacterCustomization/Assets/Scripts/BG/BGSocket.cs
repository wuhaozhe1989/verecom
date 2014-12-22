using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
public enum BGSocketState
{
	
        connectFail,
		connecting,
        idle,
        connected ,
        received ,
        disconnected ,
        pleaseDisconnect 
}
public interface BGISocket
{
    void receiveData(byte[] receiveData); //receiver data
    void socketConnected(); //
    void scoketDiconnected();
    void socketConnectFail(); //连接失败
}

public class BGSocketNew
{
    public BGISocket bgISocket = null;
	public int id;	
    public string socketName = "";
	
	
	
	private static int socketid;
    private readonly SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
    private readonly SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();    
	private readonly byte[] receiveBuffer = new byte[1024];
    private bool connected;    
    private int sendfail;
    private Socket socket;
    private Thread socketReceiveThread;
    public BGSocketState state = BGSocketState.idle;
    //private ByteStream stream = new ByteStream();
    private int zerotimes;

    public BGSocketNew(BGISocket inBGISocket)
    {
        ++socketid;
        /*
#if UNITY_ANDROID
		UpdateManager.AddUpdate(inBGISocket as MonoBehaviour,socketid,onUpdate);
#else
		UpdateManager.AddCoroutine(inBGISocket as MonoBehaviour,socketid,onUpdate);
#endif
		*/
        bgISocket = inBGISocket;
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        sendArgs.Completed += sendArg_Completed;

        id = socketid;
    }

    public bool Connected
    {
        get
        {
            if (socket == null)
            {
                MonoBehaviour.print("Socket NULL????################?");
                return false;
            }
            return socket.Connected;
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
 

		MonoBehaviour.print(ipAddress.ToString());
 
		state=BGSocketState.connecting;
		socket.BeginConnect(ipAddress,port,new AsyncCallback(connectCallback),socket);
 
 
    }

    private void connectCallback(IAsyncResult asyncConnect)
    {
        Debug.Log("connectSuccess");
		try{
			socket.EndConnect(asyncConnect);
			 Debug.Log("connectSuccess!!!!!!");
            connected = true;
            state = BGSocketState.connected;
			socketAbort=false;
//            socketReceiveThread = new Thread(ReceiveSocket);
//            socketReceiveThread.IsBackground = true;
//            socketReceiveThread.Start();
				asyncReceive();
			
		}catch(Exception e){
			Debug.Log("connect Time Out");
			state=BGSocketState.connectFail;
            //state = BGSocketState.disconnected;
			
		}finally{
			
		}
       
    }

	void asyncReceive(){
//		(byte[] buffer, int offset, int size, SocketFlags socket_flags, AsyncCallback callback, object state)
		socket.BeginReceive(buffer,0,1024,SocketFlags.None,new AsyncCallback(recved),socket);
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
        //在这个线程中接受服务器返回的数据   
        while (true)
        {
//			Thread.Sleep(100);
			if(socketAbort)break;
            if (!socket.Connected || !connected)
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
                int i = socket.Receive(tbytes);
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
	/*
    private void ReceiveAllTime()
    {
		Debug.Log("ReceiveAllTime");
        if (Connected && connected)
        {
            try
            {
                int dataLength = 0;
                dataLength = socket.Available;

                if (dataLength != 0)
                {
                    var dataBuffer = new byte[dataLength];
                    socket.Receive(dataBuffer);
                    bgISocket.receiveData(dataBuffer);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
                Debug.LogWarning("ReceiveAllTime Disconnect");
                if (connected) disconnect();
            }
        }
    }
*/
    public void onUpdate(float delta)
    {
        //if (socketReceiveThread == null) ReceiveAllTime();
        if (state > BGSocketState.idle)
        {
            switch (state)
            {
                case BGSocketState.connected:
                    connected = true;
                    bgISocket.socketConnected();
                    break;

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
    public void sendData2(byte[] buffer)
    {
        sendArgs.SetBuffer(buffer, 0, buffer.Length);

        if (socket.Connected)
        {
            socket.SendAsync(sendArgs);
        }
    }

    //send data 
    public void sendData(byte[] data)
    {
        if (socket != null && socket.Connected && connected)
        {
            IAsyncResult asyncSend = socket.BeginSend(data, 0, data.Length, SocketFlags.None, sendCallback, socket);
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

        //if(socketReceiveThread!=null&&socketReceiveThread.ThreadState!=ThreadState.Suspended&&socketReceiveThread.ThreadState!=ThreadState.SuspendRequested)socketReceiveThread.Suspend();
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
        if (socket != null && socket.Connected)
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


public class BGSocket
{
    private static int id;
    public BGISocket bgISocket = null;
    private FrameDelayCall.CallObj checkcall;
    public bool connected = false;
    private Socket socket;
    public string socketName = "";
    private BGSocketReceiveHandler socketReceiveHandler;
    private Thread socketReceiveThread;
    public BGSocketState state = BGSocketState.idle;

    public BGSocket(BGISocket inBGISocket)
    {
        ++id;
        bgISocket = inBGISocket;
        //UpdateManager.AddUpdate(bgISocket as MonoBehaviour,id,onUpdate);
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    public bool Connected
    {
        get { return socket != null && socket.Connected; }
    }

    public void release()
    {
        socket = null;
        bgISocket = null;
        socketReceiveHandler = null;
        socketReceiveThread = null;
    }

    private void ReceiveAllTime()
    {
        if (connected && Connected)
        {
            try
            {
                int dataLength = 0;
                dataLength = socket.Available;

                if (dataLength != 0)
                {
                    var dataBuffer = new byte[dataLength];
                    socket.Receive(dataBuffer);
                    bgISocket.receiveData(dataBuffer);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
                disconnect();
            }
        }
    }

    public void onUpdate(float delta)
    {
        ReceiveAllTime();
        switch (state)
        {
            case BGSocketState.disconnected:

                if (connected)
                {
                    FrameDelayCall.remove(checkcall);
                     
                    if (socket != null) socket.Disconnect(true);
                    //socket=null;
                    //socketReceiveHandler=null;
                    //socketReceiveThread=null;
                    bgISocket.scoketDiconnected();
                }
                connected = false;


                break;
        }
        state = BGSocketState.idle;
    }

    public void connect(string ipAddress, int port)
    {
        //yield return 0;

        Debug.Log("#######Connect to" + ipAddress);

        try
        {
            connected = true;
            IPAddress ip = Dns.GetHostAddresses(ipAddress)[0];
            var ipe = new IPEndPoint(ip, port);

            socket.Connect(ipe);

            //socketReceiveHandler = new BGSocketReceiveHandler(socket,bgISocket);  
            //socketReceiveThread = new Thread(new ThreadStart(socketReceiveHandler.run));  
            //socketReceiveThread.Start();  
            checkConnected();
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            disconnect();
        }
    }

    public void checkConnected()
    {
        if (Connected)
        {
            checkcall = null;
            bgISocket.socketConnected();
        }
        else
        {
            checkcall = FrameDelayCall.Add(checkConnected, 5, bgISocket as MonoBehaviour, true);
        }
    }

    public void disconnect()
    {
        if (socket != null) socket.Disconnect(true);
        state = BGSocketState.disconnected;
    }


    //send data 
    public void sendData(byte[] data)
    {
        try
        {
            if (connected) socket.Send(data);
            else MonoBehaviour.print("#####NOCONNECT SEND FAIL");
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());

            disconnect();
        }
    }


    public void sendString(string strData)
    {
        try
        {
            byte[] convertData = Encoding.UTF8.GetBytes(strData);
            if (socket != null) socket.Send(convertData);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            disconnect();
        }
    }
 
}