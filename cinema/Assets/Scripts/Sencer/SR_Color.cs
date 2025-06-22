// using UnityEngine;

// public class SR_Color : MonoBehaviour
// {
    
//     public SH_Color serialHandler;
//     public int color;

//     void Start()
//     {
//         serialHandler.OnDataReceived += OnDataReceived;
//     }

//     void Update()
//     {
//         if (!serialHandler.IsOpen)
//         {
//             Debug.LogWarning("Serial port is not open.");
//         }
//     }

//     void OnDataReceived(string message)
//     {
//         message = message.Trim();  // 改行などを除去
//         color = int.Parse(message);

//         Debug.Log($"Received: {message}");
//     }
// }
