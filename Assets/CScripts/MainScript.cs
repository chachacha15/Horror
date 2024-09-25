using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    // 位置と回転を持つ構造体
    [System.Serializable] // インスペクターで表示可能にするため
    public struct TransformData
    {
        public Vector3 position; // 位置
        public Quaternion rotation; // 回転
    }

    public static TransformData [] data = new TransformData [16];
    public GameObject RoomsParent; //部屋プリファブたちの親
    private GameObject RoomsChild; //〃の子を格納するためのもの
    private SceneChanger scenechanger;
    private string scenename = "Jikkenyou";
    // Start is called before the first frame update
    void Start()
    {
        scenechanger = FindObjectOfType<SceneChanger>();

        // インスタンス化したオブジェクトの子オブジェクトのデータを取得
        for (int num = 0; num < 16; ++num)
        {
            data[num].position = RoomsParent.transform.GetChild(num).position;
            data[num].rotation = RoomsParent.transform.GetChild(num).rotation;
        }

        scenechanger.ChangeScene(scenename);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
