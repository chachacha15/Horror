using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class randomScript : MonoBehaviour
{
    /*
    void Start()
    {
        // navMeshAgent変数にNavMeshAgentコンポーネントを入れる
        navMeshAgent = GetComponent<NavMeshAgent>();
        // 最初の目的地を入れる
        navMeshAgent.SetDestination(waypointArray[currentWaypointIndex].position);
        StartCoroutine(Warp());
    }

    [SerializeField]
    [Tooltip("巡回する地点の配列")]
    private Transform[] waypointArray;

    // NavMeshAgentコンポーネントを入れる変数
    private NavMeshAgent navMeshAgent;
    // 現在の目的地
    private int currentWaypointIndex = 0;
    private IEnumerator Warp()
    {
        while (true)
        {
            // 目的地点までの距離(remainingDistance)が目的地の手前までの距離(stoppingDistance)以下になったら
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                // 目的地の番号を１更新（右辺を剰余演算子にすることで目的地をループさせれる）
                currentWaypointIndex = (currentWaypointIndex + 1) % waypointArray.Length;
                // 目的地を次の場所に設定
                navMeshAgent.SetDestination(waypointArray[currentWaypointIndex].position);
            }
        }
    }
    */
    void Start()
    {
        StartCoroutine(Warp());
    }

    private IEnumerator Warp()
    {
        while (true)
        {
            // 10秒後ごとにワープ移動する。
            yield return new WaitForSeconds(10f);

            // ランダムな値を取得する。
            float posX = Random.Range(-120, 120);
            float posZ = Random.Range(-200, 200);

            transform.position = new Vector3(posX, 0, posZ);
        }
    }
}
