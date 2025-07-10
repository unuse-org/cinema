using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

public class UdpHandler : MonoBehaviour
{
    // デリゲート型を宣言し、データ受信イベントを定義します
    public delegate void UdpDataReceivedEventHandler(string message);
    public event UdpDataReceivedEventHandler OnDataReceived;

    [Tooltip("受信待ちするポート番号")]
    public int listenPort = 12345;

    private Thread receiveThread;
    private UdpClient client;
    private bool isRunning = false;
    private readonly ConcurrentQueue<string> receivedMessages = new ConcurrentQueue<string>();
    public static UdpHandler instance;

    private void Awake()
    {
        CheckInstance();
    }

    private void OnApplicationQuit()
    {
        Close();
    }
    
    void Update()
    {
        while (receivedMessages.TryDequeue(out string message))
        {
            OnDataReceived?.Invoke(message);
        }
    }

    public bool IsOpen
    {
        get { return isRunning && client != null; }
    }

    private void Open()
    {
        try
        {
            client = new UdpClient(listenPort);
            receiveThread = new Thread(new ThreadStart(Read));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            isRunning = true;
            Debug.Log($"UDPリスナーを開始しました。ポート: {listenPort}");
        }
        catch (Exception e)
        {
            Debug.LogError($"UDPリスナーの開始に失敗: {e.Message}");
        }
    }

    private void Close()
    {
        isRunning = false;
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }
        if (client != null)
        {
            client.Close();
            client = null;
        }
        Debug.Log("UDPリスナーを停止しました。");
    }

    private void Read()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
        while (isRunning && client != null)
        {
            try
            {
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                if (!string.IsNullOrEmpty(text))
                {
                    receivedMessages.Enqueue(text);
                }
            }
            catch (Exception e)
            {
                if(isRunning)
                {
                    Debug.LogWarning($"データ受信エラー: {e.Message}");
                }
            }
        }
    }

    void CheckInstance()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Open();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}