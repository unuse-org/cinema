using UnityEngine;
using System.Collections;

// <summary>
// PostEffect.cs
// ポストエフェクトを適用するためのスクリプト
// Unityのカメラにアタッチして使用
// </summary>

public class PostEffect : MonoBehaviour
{
    [SerializeField] Material _material;
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(source, dest, _material);
    }
}