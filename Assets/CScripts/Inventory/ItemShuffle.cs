using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShuffle : MonoBehaviour
{
    #region Variables
    public GameObject[] item;
    public GameObject red;
    public GameObject yellow;
    public GameObject blue;
    public GameObject battery;

    public GameObject posGameobject;

    int i = 0;

    Vector3[] Pos = new Vector3[5];
    #endregion

    void Start()
    {
        
        foreach (Transform child in transform)
        {
            Pos[i++] = child.localPosition;
        }
        Shuffle(Pos);

        for (int i = 0; i < item.Length; i++)
        {
            GameObject instance = Instantiate(item[i], Pos[i], Quaternion.identity);
            instance.name = item[i].name;
        }
    }

    void Update()
    {
        
    }

    // フィッシャー・イェーツのシャッフルアルゴリズムを利用した配列シャッフル
    void Shuffle(Vector3[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // 0からiまでのランダムなインデックスを取得
            Vector3 temp = array[i];                 // 現在の要素を一時保存
            array[i] = array[randomIndex];           // ランダムな位置の要素を現在の位置に
            array[randomIndex] = temp;               // 一時保存した要素をランダムな位置に
        }
    }
}
