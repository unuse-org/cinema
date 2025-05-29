using UnityEngine;
using System.Collections;

public class PostEffect : MonoBehaviour {
    [SerializeField] Material _material;
    void OnRenderImage(RenderTexture source, RenderTexture dest) {
        Graphics.Blit(source, dest, _material);
    }
}