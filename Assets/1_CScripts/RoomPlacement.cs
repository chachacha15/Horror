using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacement : MonoBehaviour
{
    private int[] rooms = new int [16];
    
    public GameObject doorPrefab;  // ドアのプレハブ
    public int doorCount = 16;     // ドアの数
    public float a = 25.35f;           // 長半径
    public float b = 12.2f;           // 短半径

    void Start()
    {
        for (int i = 0; i < doorCount; i++)
        {
            // 楕円上の等角度での点を計算
            float angle = (i / (float)doorCount) * 2 * Mathf.PI;
            float x = a * Mathf.Cos(angle);
            float y = b * Mathf.Sin(angle);

            // ドアを配置
            Vector3 position = new Vector3(x, 0, y);  // yは高さ方向に使うなら他の軸に変更
            //Debug.Log(position);
            //Instantiate(doorPrefab, position, Quaternion.identity);
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
    
    
