using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class BGSocketReceiveHandler
{
    private readonly BGISocket bgISocket;
    private readonly Socket socket;

    public BGSocketReceiveHandler(Socket inSocket, BGISocket inBGISocket)
    {
        socket = inSocket;
        bgISocket = inBGISocket;
    }


    public void run()
    {
        while (true)
        {
            try
            {
                int dataLength = 0;
                dataLength = socket.Available;
                var dataBuffer = new byte[dataLength];
                if (socket != null) socket.Receive(dataBuffer);
                if (dataLength != 0)
                    bgISocket.receiveData(dataBuffer);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.ToString());
                bgISocket.scoketDiconnected();
            }
            Thread.Sleep(10);
        }
    }
}