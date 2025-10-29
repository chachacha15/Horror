using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;



public class CatWander : MonoBehaviour, IInteractable
{
    public enum CatState
    {
        Wandering, // 徘徊・逃走
        SeekingBait, // ★ 追加: 餌へ移動中 (Eatingと分離)
        Eating,    // 魚を食べる
        Down,      // 昏倒
        GiantEnemy // 巨大エネミー
    }

    private CatState currentState = CatState.Wandering; // 現在の状態
    private bool keyItemSecured = false;

    #region Interactable (IInteractable)

    private float wanderTimer;
    private float meowTimer;
    private float nextMeowTime;

    [SerializeField] private float meowIntervalMin = 10f;
    [SerializeField] private float meowIntervalMax = 20f;
    [SerializeField] private Transform player;

    // ★ 修正: 昏倒中 かつ 鍵がまだある場合
    public string GetInteractText()
    {
        if (currentState == CatState.Down && !keyItemSecured)
        {
            return "キーアイテムを確保";
        }
        return "（今は何もできない）"; // Wandering中はテキストを変更
    }

    // ★ 修正: 昏倒中のみテキスト表示（お好みで変更可）
    public bool ShowInteractText => (currentState == CatState.Down && !keyItemSecured);
    public bool ActivateCrosshair => (currentState == CatState.Down && !keyItemSecured);

    public void Interact(GameObject targetObject)
    {
        if (currentState == CatState.Down && !keyItemSecured)
        {
            // --- 重要なキーアイテム回収処理 ---
            // (例: PlayerInventory.Instance.AddItem("CatKeyItem");)

            keyItemSecured = true;

            // ★ 追加: キーアイテムの見た目を非表示にする
            if (keyItemModel != null)
            {
                keyItemModel.SetActive(false);
            }

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

    [Header("昏倒設定")]
    public float eatingDuration = 3f; // 食べる時間
    public float comaDuration = 5f;   // 昏倒時間 (キーアイテム取得の猶予)
    public AudioClip agonizingMeow; // 苦悶の鳴き声

    // ★ 追加: 制御するコンポーネント
    [Header("関連コンポーネント")]
    [SerializeField] private Animator animator; // 猫のアニメーター
    [SerializeField] private GameObject keyItemModel; // 猫が持つ鍵のモデル

    [Header("移動速度")] // ★追加
    [SerializeField] private float normalSpeed = 3.5f; // 徘徊・餌へ向かう速度
    [SerializeField] private float escapeSpeed = 20f;  // 逃走時の速度

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
        timer = wanderInterval;
        wanderTimer = wanderInterval;

        meowTimer = 0f;
        nextMeowTime = UnityEngine.Random.Range(meowIntervalMin, meowIntervalMax);
        audioSource = GetComponent<AudioSource>();

        // ★ 追加: アニメーターが設定されていなければ取得
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // ★ 追加: ゲーム開始時、鍵は非表示にしておく
        if (keyItemModel != null)
        {
            keyItemModel.SetActive(false);
        }

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
        // 徘徊中のみ反応
        if (currentState != CatState.Wandering) return;

        // ★ 修正: 状態を「餌へ移動中」に変更
        currentState = CatState.SeekingBait;
        Debug.Log("猫：餌に向かいます。");

        // (Start()で取得済みにしたのでGetComponentは不要)
        // agent = GetComponent<NavMeshAgent>(); 
        // Debug.Log(agent);

        agent.isStopped = false;

        // ★ 修正: NavMeshの安全チェック（推奨）
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fishPosition, out hit, 1.5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogError("SetTargetFish: 目的地がNavMesh上に見つかりません！");
            currentState = CatState.Wandering; // 移動できないので徘徊に戻す
            return;
        }

        // ★ 追加: 歩くアニメーション
        if (animator != null)
        {
            animator.SetBool("IsWalking", true); // (アニメーターのパラメータ名に合わせてください)
        }
    }

    private void Update()
    {
        // プレイヤー検知 (Wandering中のみ)
        if (currentState == CatState.Wandering)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            bool playerDetected = distanceToPlayer <= detectionRadius;
            MeowLogic(playerDetected);
            if (playerDetected)
            {
                EscapeFromPlayer();
            }
            else
            {
                Wander();
            }
        }

        switch (currentState)
        {
            case CatState.Wandering:
                // 上のif文で処理済み
                break;

            // ★ 修正: Eating -> SeekingBait に変更
            case CatState.SeekingBait:
                // 魚の目的地にほぼ到着したら、コルーチンを開始
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    // ★ 修正: 既にEating状態なので、ここでコルーチンを開始
                    StartCoroutine(ComaSequence());
                }
                break;

            case CatState.Eating:
                // ComaSequenceコルーチン内で処理中 (Updateでの処理は不要)
                break;

            case CatState.Down:
                // 昏倒中は完全に動きを止める
                if (agent.enabled)
                {
                    agent.isStopped = true;
                }
                break;

            case CatState.GiantEnemy:
                // 巨大エネミー化後は、このスクリプトは役割を終える
                break;
        }
    }

    private void MeowLogic(bool playerDetected)
    {
        // 既存の鳴き声処理 (変更なし)
        // ... (省略) ...
    }

    // ★ 修正: 昏倒シーケンス
    // ★ 修正: 昏倒シーケンス
    IEnumerator ComaSequence()
    {
        // --- 到着 → 食べる (Eating) ---
        currentState = CatState.Eating;
        agent.isStopped = true; // 動きを止める
        Debug.Log("猫：到着。食べます。");

        // ... (アニメーションや効果音の処理) ...

        // 演出時間
        yield return new WaitForSeconds(eatingDuration);

        // --- 昏倒 (Down) ---
        currentState = CatState.Down;
        Debug.Log("猫：ウッ...（昏倒）。キーアイテムを取得可能です。");

        // ... (昏倒アニメーション) ...

        // ★ 鍵を表示する
        if (keyItemModel != null)
        {
            keyItemModel.SetActive(true);
        }

        // ★ 修正: タイマー起動の行を削除。
        // これで、昏倒状態のまま Interact メソッドの呼び出しを待つようになります。
    }

    // ★ 追加: 巨大エネミー化の開始メソッド
    private void StartGiantTransformation()
    {
        // (元のコードと同じ)
        currentState = CatState.GiantEnemy;

        // 1. 巨大化演出 (画面シェイク、大音量SFX)
        // CameraShake.Instance.Shake(2f, 0.5f);
        // audioSource.PlayOneShot(giantTransformationSound);

        // 2. モデルの切り替えまたはスケール変更
        // 例: transform.localScale = new Vector3(3f, 3f, 3f);

        // 3. CatWanderの役割を終了し、GiantCatEnemyAIに制御を移す
        if (agent.enabled) agent.enabled = false; // NavMeshAgentを無効化
        this.enabled = false;  // このスクリプトのUpdateを停止

        // 巨大エネミーAIコンポーネントを追加・起動 (例として)
        // gameObject.AddComponent<GiantCatEnemyAI>().Initialize();
    }

    // (以下、Wander, EscapeFromPlayer, RandomNavSphere は変更なし)
    private void Wander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderInterval)
        {
            // ... (ログやタイマーリセットのコード) ...

            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);

            if (Vector3.Distance(newPos, transform.position) < 0.1f)
            {
                // ... (停止時のコード) ...
            }
            else
            {
                // ... (ログのコード) ...

                agent.speed = normalSpeed; // ★追加: 徘徊速度に戻す
                agent.isStopped = false;
                agent.SetDestination(newPos);

                if (animator != null) animator.SetBool("IsWalking", true);
            }
        }
    }

    private void EscapeFromPlayer()
    {
        Debug.Log("EscapeFromPlayer: プレイヤーから逃げます！");

        Vector3 dirFromPlayer = (transform.position - player.position).normalized;
        Vector3 escapePos = transform.position + dirFromPlayer * escapeDistance;

        if (NavMesh.SamplePosition(escapePos, out NavMeshHit navHit, wanderRadius, NavMesh.AllAreas))
        {
            agent.speed = escapeSpeed; // ★追加: 逃走速度に変更
            agent.isStopped = false;
            agent.SetDestination(navHit.position);
            if (animator != null) animator.SetBool("IsWalking", true);
        }
        else
        {
            // ... (逃げ場がない時のコード) ...
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        // ... (既存の RandomNavSphere ロジック) ...
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * dist;
        randomDirection += origin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, dist, NavMesh.AllAreas))
        {
            // 見つかった場合：NavMesh上の座標を返す
            return navHit.position;
        }

        // ★ 追加：見つからなかった場合
        // 元の位置(origin)をそのまま返す
        return origin;
    }
}