using UnityEngine;

public class udp_receiver_imu : MonoBehaviour
{
    
    public  udp_handler_imu udpHandlerIMU;
    public int senser;

    void Start()
    {
        udpHandlerIMU.OnDataReceived += OnDataReceived; // データ受信イベントにハンドラを登録
    }

    void Update()
    {
        if (!udpHandlerIMU.IsOpen)
        {
            Debug.LogWarning("UDP port is not open.");
        }
    }

    void OnDataReceived(string message)
    {
        message = message.Trim();  // 改行などを除去
        senser = int.Parse(message);

        Debug.Log($"IMU Received: {message}");
        //Debug.Log("senser: "+senser);
    }
}
