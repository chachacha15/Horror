using System;
using UnityEngine;
using UnityEngine.AI;

public class CatWander : MonoBehaviour
{
    [Header("徘徊範囲設定")]
    public float wanderRadius = 10f; // 徘徊する範囲の半径
    public float wanderInterval = 5f; // 移動間隔（秒）

    private NavMeshAgent agent;
    private float timer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderInterval;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    /// <summary>
    /// 指定範囲内のランダムなNavMesh上の座標を返す
    /// </summary>
    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * dist;
        randomDirection += origin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, dist, NavMesh.AllAreas))
        {
            return navHit.position;
        }

        return origin;
    }

}
