using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class CatMoving : MonoBehaviour, IInteractable
{
    #region IInteractable 実装
    public string GetInteractText() => "捕まえる";
    public bool ShowInteractText => true;
    public bool ActivateCrosshair => true;
    public void Interact(GameObject targetObject) { /* 捕獲処理など */ }
    #endregion

    [Header("基本設定")]
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;
    private float timer;

    [Header("プレイヤー検知")]
    public float detectionRadius = 8f;   // 視覚的発見距離
    public float escapeDistance = 12f;   // 逃走距離
    private bool isEscaping = false;

    [Header("鳴き声")]
    public AudioClip normalMeow;
    public AudioClip alertMeow;
    private AudioSource audioSource;
    private float meowTimer;
    private float nextMeowTime;
    [SerializeField] private float meowIntervalMin = 10f;
    [SerializeField] private float meowIntervalMax = 20f;

    private NavMeshAgent agent;
    private Transform player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        timer = wanderInterval;
        meowTimer = 0f;
        nextMeowTime = Random.Range(meowIntervalMin, meowIntervalMax);

        // プレイヤー取得
        player = PlayerMove.Instance.transform;

        // SoundEventManagerに登録（プレイヤーのダッシュ音も拾う用）
        SoundEventManager.OnSoundEmitted += OnSoundHeard;
    }

    private void OnDestroy()
    {
        SoundEventManager.OnSoundEmitted -= OnSoundHeard;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerDetected = distanceToPlayer <= detectionRadius || isEscaping;

        // 鳴き声処理
        meowTimer += Time.deltaTime;
        if (meowTimer >= nextMeowTime)
        {
            if (playerDetected && alertMeow != null)
                audioSource.PlayOneShot(alertMeow);
            else if (normalMeow != null)
                audioSource.PlayOneShot(normalMeow);

            meowTimer = 0f;
            nextMeowTime = Random.Range(meowIntervalMin, meowIntervalMax);
        }

        // 動作切り替え
        if (playerDetected)
            EscapeFromPlayer();
        else
            Wander();
    }

    private void Wander()
    {
        if (isEscaping) return; // 逃走中は徘徊しない

        timer += Time.deltaTime;
        if (timer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            timer = 0f;
        }
    }

    private void EscapeFromPlayer()
    {
        if (!isEscaping)
        {
            isEscaping = true;
            StartCoroutine(EscapeCooldown());
        }

        Vector3 dirFromPlayer = (transform.position - player.position).normalized;
        Vector3 escapePos = transform.position + dirFromPlayer * escapeDistance;

        if (NavMesh.SamplePosition(escapePos, out NavMeshHit navHit, wanderRadius, NavMesh.AllAreas))
            agent.SetDestination(navHit.position);
    }

    IEnumerator EscapeCooldown()
    {
        yield return new WaitForSeconds(5f);  // 5秒間は逃げ続ける
        isEscaping = false;
    }

    // ダッシュ音などの音イベントから反応する
    private void OnSoundHeard(Vector3 soundPosition, float range, string tag)
    {
        if (Vector3.Distance(transform.position, soundPosition) <= range)
        {
            isEscaping = true;
            Debug.Log("猫が音に反応して逃げ始めました！");
        }
    }

    // ランダム移動用
    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist + origin;
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, dist, NavMesh.AllAreas))
            return navHit.position;
        return origin;
    }
}
