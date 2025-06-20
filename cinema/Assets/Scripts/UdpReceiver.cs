using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using TMPro;

public class UdpReceiver : MonoBehaviour
{
    [Tooltip("受信待ちするUDPポート番号")]
    public int listenPort = 12345; // M5AtomのtargetPortと一致させる

    [Tooltip("受信したメッセージを表示するText UI (TextMeshProUI推奨)")]
    public TextMeshProUGUI receivedTextUI; 

    private UdpClient udpClient;
    private Thread receiveThread;
    // スレッド間で受信メッセージを安全に受け渡すためのキュー
    private ConcurrentQueue<string> receivedMessages = new ConcurrentQueue<string>();

    private bool isReceiving = false;

    void Start()
    {
        Debug.Log($"UDP Receiver starting on port {listenPort}...");
        InitializeUdpListener();
    }

    void InitializeUdpListener()
    {
        try
        {
            // UdpClientを特定のポートで初期化
            udpClient = new UdpClient(listenPort);

            // 受信処理用の新しいスレッドを作成し開始
            isReceiving = true;
            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true; // アプリケーション終了時に自動で終了するように設定
            receiveThread.Start();

            Debug.Log("UDP Listener initialized and thread started.");

        }
        catch (Exception e)
        {
            Debug.LogError($"UDP Initialization Error: {e.Message}");
            // エラー発生時は受信を停止
            isReceiving = false;
        }
    }

    // 受信スレッドで実行される関数
    private void ReceiveData()
    {
        Debug.Log("ReceiveData thread started.");
        while (isReceiving)
        {
            try
            {
                // データを受信するまでここでブロックされる
                // IPAdress.AnyはどのIPからでも受け付けるようにする
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref anyIP);

                // 受信したバイト列を文字列に変換
                string message = Encoding.UTF8.GetString(data);

                // 受信したメッセージをキューに追加
                receivedMessages.Enqueue(message);

            }
            catch (SocketException e)
            {
                // SocketException は、UdpClientが閉じられたときなどにも発生する
                if (!isReceiving) // アプリケーション終了処理中であれば正常な終了
                {
                    Debug.Log("UDP Receive thread shutting down normally.");
                }
                else // それ以外の場合はエラー
                {
                     Debug.LogError($"UDP Socket Exception: {e.Message}");
                }
            }
            catch (Exception e)
            {
                // その他のエラー
                Debug.LogError($"UDP Receive Thread Error: {e.Message}");
            }
        }
        Debug.Log("ReceiveData thread finished.");
    }

    // Unityのメインスレッドで実行されるUpdateメソッド
    void Update()
    {
        // キューからメッセージを取り出し、メインスレッドで処理
        while (receivedMessages.TryDequeue(out string message))
        {
            Debug.Log($"Received message in Unity: {message}");

            if (receivedTextUI != null)
            {
                receivedTextUI.text = $"Last Msg: {message}";
            }
        }
    }

    // アプリケーション終了時やオブジェクト破棄時に呼び出される
    void OnApplicationQuit()
    {
        Debug.Log("Application quitting. Stopping UDP receiver.");
        StopUdpListener();
    }

     void OnDestroy()
    {
        Debug.Log("Object being destroyed. Stopping UDP receiver.");
        StopUdpListener();
    }


    // UDPリスナーとスレッドを停止するメソッド
    void StopUdpListener()
    {
        if (isReceiving)
        {
            isReceiving = false; // 受信ループを終了させるためのフラグを設定

            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null; // 念のためnullクリア
                 Debug.Log("UdpClient closed.");
            }

        }
         Debug.Log("UDP Receiver stopped.");
    }
}