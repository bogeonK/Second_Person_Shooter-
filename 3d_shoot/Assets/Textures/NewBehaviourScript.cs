using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTilingController : MonoBehaviour
{
    // 타일링 비율을 조절할 변수
    public float tileX = 1.0f;
    public float tileY = 1.0f;

    void Start()
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            // 큐브의 크기에 따라 타일링 조정
            Vector3 scale = transform.localScale;
            meshRenderer.material.mainTextureScale = new Vector2(scale.x / tileX, scale.y / tileY);
        }
    }
}