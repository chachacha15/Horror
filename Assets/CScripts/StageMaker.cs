using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMaker : MonoBehaviour
{
    
    public GameObject trackPre; //トラックのprefab入れる用
    public GameObject roomPre;  //部屋のprefab入れる用
    public GameObject playerPre;
    public GameObject enemyPre;
    private GameObject [] Rooms = new GameObject [16];
    private List<int> numbers = new List<int>();    // 16個の数字（0～15）を格納するリスト



    void Start()
    {
        // リストに0～15までの数字を追加
        for (int i = 0; i < 16; i++)
        {
            numbers.Add(i);
        }

        // リストをシャッフル
        Shuffle(numbers);

        // シャッフルされたリストを表示
        foreach (int number in numbers)
        {
            Debug.Log("Shuffled number: " + number);
        }


        GameObject Track = Instantiate(trackPre, new Vector3(0,0,0),Quaternion.Euler(90,0,-180)); //トラックを召喚

        for (int num = 0; num < 16; ++num)
        {
            Rooms[num] = Instantiate(roomPre, MainScript.data[numbers[num]].position, MainScript.data[numbers[num]].rotation); //部屋をランダムに召喚
            Rooms[num].name = "Room" + num; //部屋の番号確認用
        }

        GameObject Player = Instantiate(playerPre, playerPre.transform.position, playerPre.transform.rotation);
        GameObject Enemy = Instantiate(enemyPre, enemyPre.transform.position, enemyPre.transform.rotation);



    }

    void Update()
    {
        
    }


    // フィッシャー–イェーツのシャッフルアルゴリズム
    void Shuffle(List<int> list)  //シャッフルするための関数
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // 0からiまでのランダムなインデックスを取得
            int temp = list[i];  // 一時的にリストのi番目を保存
            list[i] = list[randomIndex];  // i番目にランダムな要素をセット
            list[randomIndex] = temp;  // ランダムな位置に元のi番目の要素をセット
        }
    }
}
