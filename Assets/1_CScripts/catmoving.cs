using System;
using UnityEngine;
using UnityEngine.AI;

public class CatWander : MonoBehaviour, IInteractable
{
    #region Interactable (IInteractable)

    // 追加すべき変数（catmoving.cs のクラス内、Start()やUpdate()の外に記述）
    private float wanderTimer;

    private float meowTimer;
    private float nextMeowTime;

    [SerializeField] private float meowIntervalMin = 10f;
    [SerializeField] private float meowIntervalMax = 20f;
    [SerializeField] private Transform player;


    public string GetInteractText()
    {

        return "捕まえる";
    }

    public bool ShowInteractText => true; // テキスト表示するかどうか
    public bool ActivateCrosshair => true;

    /// <summary>
    /// クリック時、開閉
    /// </summary>
    public void Interact(GameObject targetObject)
    {


    }

    #endregion

    [Header("徘徊範囲設定")]
    public float wanderRadius = 10f; // 徘徊する範囲の半径
    public float wanderInterval = 5f; // 移動間隔（秒）

    private NavMeshAgent agent;
    private float timer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderInterval;
        wanderTimer = wanderInterval;

        meowTimer = 0f;
        nextMeowTime = UnityEngine.Random.Range(meowIntervalMin, meowIntervalMax);

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            else
            {
                Debug.LogError("catmoving.cs：Playerが見つかりません。タグ 'Player' を付けてください。");
            }
        }
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
