using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScrol : MonoBehaviour
{
    
    public Material waterMaterial;
    public float speedX = 0.05f;
    public float speedY = 0.05f;

    void Update()
    {
        float offsetX = Time.time * speedX;
        float offsetY = Time.time * speedY;
        waterMaterial.SetTextureOffset("_NormalMap", new Vector2(offsetX, offsetY));
    }
}
