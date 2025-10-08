using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class CatWander : MonoBehaviour, IInteractable
{
    #region Interactable (IInteractable)

    private float wanderTimer;
    private float meowTimer;
    private float nextMeowTime;

    [SerializeField] private float meowIntervalMin = 10f;
    [SerializeField] private float meowIntervalMax = 20f;
    [SerializeField] private Transform player;

    public string GetInteractText() => "捕まえる";
    public bool ShowInteractText => true;
    public bool ActivateCrosshair => true;

    public void Interact(GameObject targetObject)
    {
        // 今回は未使用
    }

    #endregion

    [Header("徘徊設定")]
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;
    private float timer;

    [Header("プレイヤー検知設定")]
    public float detectionRadius = 8f;
    public float escapeDistance = 12f;

    [Header("鳴き声設定")]
    public AudioClip normalMeow;
    public AudioClip alertMeow;
    private AudioSource audioSource;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderInterval;
        wanderTimer = wanderInterval;

        meowTimer = 0f;
        nextMeowTime = UnityEngine.Random.Range(meowIntervalMin, meowIntervalMax);
        audioSource = GetComponent<AudioSource>();

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            else
            {
                //Debug.LogError("CatWander：Playerが見つかりません。タグ 'Player' を付けてください。");
            }
        }
    }

    private void Update()
    {
        // プレイヤー検知
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerDetected = distanceToPlayer <= detectionRadius;

        // 鳴き声処理
        meowTimer += Time.deltaTime;
        if (meowTimer >= nextMeowTime)
        {
            // プレイヤーが検知された場合
            if (playerDetected && alertMeow != null)
            {
                // 既に警戒音が鳴っていなければ再生
                if (audioSource.clip != alertMeow || !audioSource.isPlaying)
                {
                    //audioSource.clip = alertMeow;
                    audioSource.PlayOneShot(alertMeow);
                }
            }
            // プレイヤーが検知されていない場合
            else if (!playerDetected && normalMeow != null)
            {
                // 既に通常音が鳴っていなければ再生
                if (audioSource.clip != normalMeow || !audioSource.isPlaying)
                {
                    //audioSource.clip = normalMeow;
                    audioSource.PlayOneShot(normalMeow);
                }
            }

            // タイマーをリセットし、次の鳴き声までの時間を再設定
            meowTimer = 0f;
            nextMeowTime = UnityEngine.Random.Range(meowIntervalMin, meowIntervalMax);
        }

        // 行動切り替え
        if (playerDetected)
        {
            EscapeFromPlayer();
        }
        else
        {
            Wander();
        }
    }

    // -------------------- 修正箇所：以下のメソッドを追加 --------------------

    private void Wander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    private void EscapeFromPlayer()
    {
        Vector3 dirFromPlayer = (transform.position - player.position).normalized;
        Vector3 escapePos = transform.position + dirFromPlayer * escapeDistance;

        if (NavMesh.SamplePosition(escapePos, out NavMeshHit navHit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
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