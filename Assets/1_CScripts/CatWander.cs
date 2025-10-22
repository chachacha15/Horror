using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CatWander : MonoBehaviour, IInteractable
{
    public enum CatState
    {
        Wandering, // 徘徊・逃走
        Eating,    // 魚を食べる
        Down,      // 昏倒
        GiantEnemy // 巨大エネミー
    }

    private CatState currentState = CatState.Wandering; // 現在の状態
    private bool keyItemSecured = false; // ★ 追加: キーアイテム回収フラグ

    #region Interactable (IInteractable)

    private float wanderTimer;
    private float meowTimer;
    private float nextMeowTime;

    [SerializeField] private float meowIntervalMin = 10f;
    [SerializeField] private float meowIntervalMax = 20f;
    [SerializeField] private Transform player;

    // ★ 修正: 状態に応じてテキストを切り替える
    public string GetInteractText()
    {
        if (currentState == CatState.Down && !keyItemSecured)
        {
            return "キーアイテムを確保";
        }
        return "捕まえる (今は無理だ)";
    }
    public bool ShowInteractText => true;
    public bool ActivateCrosshair => true;

    // ★ 修正: インタラクト処理
    public void Interact(GameObject targetObject)
    {
        if (currentState == CatState.Down && !keyItemSecured)
        {
            // --- 重要なキーアイテム回収処理 ---

            // 例: プレイヤーのインベントリにキーを追加する処理をここに記述
            // PlayerInventory.Instance.AddItem("CatKeyItem");

            keyItemSecured = true;

            // キーアイテムの見た目を非表示にする
            // transform.Find("KeyItemModel").gameObject.SetActive(false); 

            // 回収完了メッセージ
            UnityEngine.Debug.Log("キーアイテムを確保しました。");
        }
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

    [Header("昏倒設定")] // ★ 追加: 昏倒に関する設定
    public float eatingDuration = 3f; // 食べる時間 (安堵の演出時間)
    public float comaDuration = 5f;   // 昏倒時間 (キーアイテム取得の猶予)
    public AudioClip agonizingMeow; // 苦悶の鳴き声 (演出用)

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Debug.Log(agent);
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
        }
    }

    // 外部から魚の位置を伝えるメソッド
    public void SetTargetFish(Vector3 fishPosition)
    {
        // 既に食事中や昏倒中なら無視
        if (currentState != CatState.Wandering) return;

        currentState = CatState.Eating;

        agent = GetComponent<NavMeshAgent>();
        Debug.Log(agent);

        agent.isStopped = false;
        agent.SetDestination(fishPosition);

        // アニメーターがあれば、移動時のアニメーションを Eating に切り替える
    }

    private void Update()
    {
        // プレイヤー検知
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerDetected = distanceToPlayer <= detectionRadius;

        switch (currentState)
        {
            case CatState.Wandering:
                // 徘徊・逃走ロジック
                MeowLogic(playerDetected);
                if (playerDetected)
                {
                    EscapeFromPlayer();
                }
                else
                {
                    Wander();
                }
                break;

            case CatState.Eating:
                // 魚の目的地にほぼ到着したら、コルーチンを開始
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    StartCoroutine(ComaSequence());
                }
                break;

            case CatState.Down:
                // 昏倒中は完全に動きを止める
                agent.isStopped = true;
                break;

            case CatState.GiantEnemy:
                // 巨大エネミー化後は、このスクリプトは役割を終えるか、
                // GiantCatEnemyAIなどの別コンポーネントに制御を移す
                break;
        }
    }

    private void MeowLogic(bool playerDetected)
    {
        // 既存の鳴き声処理 (変更なし)
        meowTimer += Time.deltaTime;
        if (meowTimer >= nextMeowTime)
        {
            if (playerDetected && alertMeow != null)
            {
                audioSource.PlayOneShot(alertMeow);
            }
            else if (!playerDetected && normalMeow != null)
            {
                audioSource.PlayOneShot(normalMeow);
            }

            meowTimer = 0f;
            nextMeowTime = UnityEngine.Random.Range(meowIntervalMin, meowIntervalMax);
        }
    }

    // ★ 追加: 昏倒シーケンスを制御するコルーチン
    IEnumerator ComaSequence()
    {
        // 食べるアニメーション、不協和音の再生など
        audioSource.PlayOneShot(agonizingMeow);

        currentState = CatState.Eating;
        agent.isStopped = true; // 動きを止める

        // 演出時間
        yield return new WaitForSeconds(eatingDuration);

        // --- 昏倒 (Down) ---
        currentState = CatState.Down;
        // 例: モデルを横倒しにするアニメーションを再生 (Animator.SetTrigger("Down"))
        // BGMを静かな不協和音に変更する

        // キーアイテム取得の猶予時間
        yield return new WaitForSeconds(comaDuration);

        // キーアイテムを回収していなければ、即座に巨大化する
        if (!keyItemSecured)
        {
            UnityEngine.Debug.Log("キーアイテムが回収されなかったため、即座に巨大化！");
            StartGiantTransformation();
        }
        else
        {
            // 猶予期間終了後、巨大化へ
            StartGiantTransformation();
        }
    }

    // ★ 追加: 巨大エネミー化の開始メソッド
    private void StartGiantTransformation()
    {
        currentState = CatState.GiantEnemy;

        // 1. 巨大化演出 (画面シェイク、大音量SFX)
        // CameraShake.Instance.Shake(2f, 0.5f);
        // audioSource.PlayOneShot(giantTransformationSound);

        // 2. モデルの切り替えまたはスケール変更
        // 例: transform.localScale = new Vector3(3f, 3f, 3f);

        // 3. CatWanderの役割を終了し、GiantCatEnemyAIに制御を移す
        agent.enabled = false; // NavMeshAgentを無効化
        this.enabled = false;  // このスクリプトのUpdateを停止

        // 巨大エネミーAIコンポーネントを追加・起動 (例として)
        // gameObject.AddComponent<GiantCatEnemyAI>().Initialize();
    }

    private void Wander()
    {
        // 既存の Wander ロジック (変更なし)
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
        // 既存の EscapeFromPlayer ロジック (変更なし)
        Vector3 dirFromPlayer = (transform.position - player.position).normalized;
        Vector3 escapePos = transform.position + dirFromPlayer * escapeDistance;

        if (NavMesh.SamplePosition(escapePos, out NavMeshHit navHit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
    }

    /// <summary>
    /// 指定範囲内のランダムなNavMesh上の座標を返す (変更なし)
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