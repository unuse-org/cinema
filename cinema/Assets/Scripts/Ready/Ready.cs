// using UnityEngine;

// public class Ready : MonoBehaviour
// {
//     private TextDisplay textDisplay; // TextDisplayスクリプトの参照
//     private int openningFlag = 0;
//     public int changeSceneFlag = 0; // シーン変更フラグ

//     void Start()
//     {
//         // if (textDisplay == null)
//         // {
//         //     textDisplay = FindObjectOfType<TextDisplay>();
//         //     if (textDisplay == null)
//         //     {
//         //         Debug.LogError("TextDisplay script not found in the scene.");
//         //     }
//         // }
//     }


//     // TextDisplayのEndTextDisplayFlagが1になったら、一度だけ音楽を流す
//     void Update()
//     {
//         if (textDisplay != null && textDisplay.EndTextDisplayFlag == 1 && openningFlag == 0)
//         {
//             openningFlag = 1; // 一度だけ音楽を流すためのフラグ
//             Invoke("SetDirectionalLight", 1.5f);
//             PlayAudio();

//             // シーン変更フラグを立てる
//             Invoke("SetChangeSceneFlag", 5f);
//         }
//     }

//     void PlayAudio()
//     {
//         AudioSource audioSource = GetComponent<AudioSource>();
//         if (audioSource != null && !audioSource.isPlaying)
//         {
//             //1秒後に音楽を流す
//             //audioSource.PlayDelayed(2.5f);
//             //audioSource.Play();
//         }
//         else
//         {
//             Debug.LogWarning("AudioSource component not found or already playing.");
//         }
//     }
//     //Directional Lightの設定を行う
//     public void SetDirectionalLight()
//     {
//         Light directionalLight = FindObjectOfType<Light>();
//         if (directionalLight != null && directionalLight.type == LightType.Directional)
//         {
//             directionalLight.intensity = 0f;
//         }
//         else
//         {
//             Debug.LogWarning("Directional Light not found or is not of type Directional.");
//         }
//     }
//     void SetChangeSceneFlag()
//     {
//         changeSceneFlag = 1; // シーン変更フラグを立てる
//     }
// }