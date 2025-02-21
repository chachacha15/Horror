using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacement : MonoBehaviour
{
    private int[] rooms = new int [16];
    
    public GameObject doorPrefab;  // �h�A�̃v���n�u
    public int doorCount = 16;     // �h�A�̐�
    public float a = 25.35f;           // �����a
    public float b = 12.2f;           // �Z���a

    void Start()
    {
        for (int i = 0; i < doorCount; i++)
        {
            // �ȉ~��̓��p�x�ł̓_���v�Z
            float angle = (i / (float)doorCount) * 2 * Mathf.PI;
            float x = a * Mathf.Cos(angle);
            float y = b * Mathf.Sin(angle);

            // �h�A��z�u
            Vector3 position = new Vector3(x, 0, y);  // y�͍��������Ɏg���Ȃ瑼�̎��ɕύX
            //Debug.Log(position);
            //Instantiate(doorPrefab, position, Quaternion.identity);
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
    
    
