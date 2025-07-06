using UnityEngine;

public class UdpReceiver : MonoBehaviour
{
    
    public  UdpHandler udpHandler; // UdpHandlerのインスタンス
    public int color;

    void Start()
    {
        udpHandler.OnDataReceived += OnDataReceived; // データ受信イベントにハンドラを登録
    }

    void Update()
    {
        if (!udpHandler.IsOpen)
        {
            Debug.LogWarning("UDP port is not open.");
        }
    }

    void OnDataReceived(string message)
    {
        message = message.Trim();  // 改行などを除去
        color = int.Parse(message);

        //Debug.Log($"Color Received: {message}");
    }
}
