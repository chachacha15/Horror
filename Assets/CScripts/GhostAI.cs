using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    NavMeshAgent agent;
    private GameObject mouse;
    public float moveRadius = 10f;
    private Vector3[] DestPos = new Vector3[16];
    private List<int> numbers = new List<int>();    // 16個の数字（0～15）を格納するリスト
    private int i = 0;



    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        DestinationPosition();

        // リストに0～15までの数字を追加
        for (int i = 0; i < 16; i++)
        {
            numbers.Add(i);
        }

        // リストをシャッフル
        Shuffle(numbers);

    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(DestPos[numbers[i]]);
        if (agent.remainingDistance <= agent.stoppingDistance)  // 目的地に到達したら
        {
            ++i;

            if (i > 15)
            {
                i = 0;
            }

            agent.SetDestination(DestPos[numbers[i]]);  // 新しい目的地をセット

        }
    }

    void DestinationPosition()
    {
        DestPos[0] = new Vector3(-20.03f, -1.08f, 5.17f);
        DestPos[1] = new Vector3(-14.34f, 1.08f, 8.35f);
        DestPos[2] = new Vector3(-6.99f, 1.08f, 10.18f);
        DestPos[3] = new Vector3(-0.18f, 1.10f, 10.60f);
        DestPos[4] = new Vector3(6.44f, 1.08f, 10.10f);
        DestPos[5] = new Vector3(13.73f, 1.08f, 8.47f);
        DestPos[6] = new Vector3(20.50f, 1.08f, 5.02f);
        DestPos[7] = new Vector3(24.05f, 1.08f, 0.28f);
        DestPos[8] = new Vector3(20.82f, 1.08f, -4.80f);
        DestPos[9] = new Vector3(14.36f, 1.08f, -8.12f);
        DestPos[10] = new Vector3(7.19f, 1.08f, -9.97f);
        DestPos[11] = new Vector3(0.13f, 1.12f, -10.64f);
        DestPos[12] = new Vector3(-6.60f, 1.08f, -9.66f);
        DestPos[13] = new Vector3(-13.93f, 1.08f, -7.96f);
        DestPos[14] = new Vector3(-20.00f, 1.08f, -4.61f);
        DestPos[15] = new Vector3(-23.44f, 1.11f, -0.17f);
    }


    Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        // 指定した半径内でランダムな座標を生成
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        // NavMesh上の近くのポイントをサンプリング
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;  // NavMesh上の有効な地点を返す
        }

        return center;  // NavMesh上の地点が見つからなかった場合、元の位置を返す
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

    //使いませんが、参考のためとっておく
    void MouseCilck()
    {
        if (Input.GetMouseButtonDown(0))  // 左クリック
        {
            Vector3 screenPosition = Input.mousePosition;  // スクリーン座標
            screenPosition.z = 10f;  // カメラからの距離を指定（Z座標）

            // スクリーン座標をワールド座標に変換
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Debug.Log("World Position: " + worldPosition);

            agent.SetDestination(worldPosition);

        }

        if (agent.remainingDistance <= agent.stoppingDistance)  // 目的地に到達したら
        {
            Vector3 randomPoint = GetRandomPoint(transform.position, moveRadius);  // ランダムな地点を取得
            agent.SetDestination(randomPoint);  // 新しい目的地をセット
        }
    }
}
