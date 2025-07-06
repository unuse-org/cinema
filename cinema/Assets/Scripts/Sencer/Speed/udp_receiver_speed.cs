using UnityEngine;

public class udp_receiver_speed : MonoBehaviour
{
    
    public  udp_handle_speed udp_handle_speed; // udp_handle_speedのインスタンス
    public int Speed;

    void Start()
    {
        udp_handle_speed.OnDataReceived += OnDataReceived; // データ受信イベントにハンドラを登録
    }

    void Update()
    {
        if (!udp_handle_speed.IsOpen)
        {
            Debug.LogWarning("UDP port is not open.");
        }
    }

    void OnDataReceived(string message)
    {
        message = message.Trim();  // 改行などを除去
        Speed = int.Parse(message);

        Debug.Log($"Speed Received: {message}");
    }
}
