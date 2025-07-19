using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

        //color = 0;
        if (int.TryParse(message, out color))
        {
            // 成功: color に値が入っている
            color = int.Parse(message);
            Debug.Log("格納："+color);
        }
        else
        {
            Debug.LogWarning($"⚠️ 数値変換に失敗: \"{message}\"");
            //color = 0; // 失敗時のデフォルト値
        }

        Debug.Log($"Color Received: {message}");
    }
}
