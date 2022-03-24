using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    Renderer material;
    public void SetPlane()
    {
        material = GetComponent<Renderer>();
        Vector2 scale = material.material.mainTextureScale;
        scale.x = 0.5f;
        scale.y = transform.localScale.y / (transform.localScale.x * 2f);
        material.material.mainTextureScale = scale;
        //调整墙的上下平面的高度
        transform.Find("Over").position = transform.position + new Vector3(0, transform.localScale.y / 2 + 0.001f, 0);
        transform.Find("Under").position = transform.position + new Vector3(0, -transform.localScale.y / 2 - 0.001f, 0);
    }
}
