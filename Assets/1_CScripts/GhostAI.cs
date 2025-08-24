/*
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// 敵の状態(巡回状態か追跡状態)
public enum State { Patrol, Chase, Investigate }; // Investigate状態を追加

public class GhostAI : MonoBehaviour
{
    #region Variables

    public State currentState;

    private GameObject playerTarget;    // プレイヤーのターゲット
    private NavMeshAgent agent;         // NavMeshAgent
    private Vector3[] DestPos = new Vector3[45]; // 巡回ポイントのリスト
    private int patrolIndex = 0;        // 巡回ポイントのインデックス
    private const float doorCheckDistance = 5.0f; // 開閉する障害物を開けようとする距離
    private const float openingTime = 1.0f; // 開けるのにかかる時間 

    public bool isWaiting = false; // 待機中かどうか

    private GameObject currentTargetDoor; // 現在向かっているドア


    // 巡回・追跡
    private float lostTimer = 0f;       // プレイヤーを見失った時間
    public float lostThreshold = 5f;    // プレイヤーを見失うまでの時間（秒）
    private float visibilityTimer = 0f; // プレイヤーが視界内に留まった時間
    public float visibilityThreshold = 3f; // 遠距離での発見までの時間（秒）
    public float patrolSpeed = 2f;      // 巡回時の速度
    public float chaseSpeed = 5f;       // 追跡時の速度

    // 調査 ( プレイヤーやギミックが音を立てると、調査に来る )
    private Vector3? investigateTarget; // 調査先座標
    private float investigateTimer;      // 調査実行時間
    public float investigateDuration = 3f;  // 調査にかける時間
    public float investigateSpeed = 3f;  // 調査先移動速度
    public AudioClip noticeSound; // 発覚音
    private AudioSource noticeAS; // 発覚音AudioSource


    // 感知ステータス
    public float detectionRadius = 60f; // プレイヤー検知の半径
    public float fieldOfView = 150f;     // 視界角度
    private bool isPlayerVisible = false; // プレイヤーが視界に入っているか

    // その他
    public bool isWallEnemy = false; // 壁の擬態をする敵か
    public float shrinkSpeed = 0.3f; // Z軸を 0 にする速さ


    // 敵の揺れ
    public float swayAmount = 0.5f; // 揺れの幅
    public float swaySpeed = 2f;    // 揺れの速さ

    // サウンド
    [SerializeField] private AudioClip enemyVoiceSound;
    private AudioSource audioSource;
    private bool isPlayingVoiceSound = false;

    // 他クラス
    private CameraSwitcher cameraSwitcher;
    private EnemyManager enemyManager;
    private DoorController doorController; // この参照は使用されていない可能性があります
    private DoorManager doorManager; // この参照は使用されていない可能性があります
    private SoundEventManager soundEventManager;

    #endregion

    void OnDestroy()
    {
        if (SoundEventManager.Instance != null)
            SoundEventManager.Instance.OnSoundEmitted -= OnSoundHeard;
        // FireAlarmManagerのイベント購読解除も追加
        PicturePuzzleManager.OnPuzzleFailedTooManyTimes -= OnAlarmSounded;
    }

    void Start()
    {
        // 必要なコンポーネントを取得
        audioSource = GetComponent<AudioSource>();
        // SoundEventManager の初期化を確実に行うため、Awake()でInstanceが設定されることを前提とするか
        // SoundEventManager.Instance が null の場合に FindObjectOfType<SoundEventManager>() を使う
        if (GameObject.Find("EnemyNoticeAS") != null)
        {
            noticeAS = GameObject.Find("EnemyNoticeAS").GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("EnemyNoticeAS GameObject not found. Creating a new AudioSource.");
            noticeAS = gameObject.AddComponent<AudioSource>();
            // 必要に応じてnoticeSoundをnoticeASに設定
            noticeAS.clip = noticeSound;
            noticeAS.playOnAwake = false;
        }

        agent = GetComponent<NavMeshAgent>();


        // 他クラスを取得
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        enemyManager = FindObjectOfType<EnemyManager>();
        // doorController = FindObjectOfType<DoorController>(); // 使用されていないなら削除検討
        // doorManager = FindObjectOfType<DoorManager>(); // 使用されていないなら削除検討
        soundEventManager = FindObjectOfType<SoundEventManager>(); // これもInstanceで取得する方が良い

        DestinationPosition(); // 巡回ポイントの初期化
        currentState = State.Patrol;    // 初期状態は巡回
        agent.speed = patrolSpeed;      // 初期速度を巡回速度に設定


        // イベント購読
        if (SoundEventManager.Instance != null) // nullチェックを追加
            SoundEventManager.Instance.OnSoundEmitted += OnSoundHeard;
        //PicturePuzzleManager.OnPuzzleFailedTooManyTimes += OnAlarmSounded; // ★追加: アラームイベントを購読

        if (isWallEnemy) agent.enabled = false;
    }

    void Update()
    {
        if (isWaiting) return; // 停止中なら処理しない

        // 一定期間でピッチをランダムに調整
        if (!isPlayingVoiceSound) StartCoroutine(enemyRandomPicthVoice());

        AdjustPositionIfNearWall(); // 壁や障害物との距離を保つ


        // プレイヤーが無効化されている場合でも巡回を継続
        if (playerTarget == null || !playerTarget.activeInHierarchy)
        {
            isPlayerVisible = false;

            // 巡回状態に切り替え
            if (currentState != State.Patrol)
            {
                currentState = State.Patrol;
                agent.ResetPath(); // ナビメッシュの目標をリセット
                agent.speed = patrolSpeed; // 巡回速度に設定
                enemyManager.FindChasingEnemy();
            }
        }

        // 現在の状態に応じて動作を切り替え
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Investigate: // ★追加: Investigate状態の処理
                Investigate();
                break;
        }

        // プレイヤーが有効な場合のみ視認チェックを行う
        if (playerTarget != null && playerTarget.activeInHierarchy)
        {
            CheckPlayerVisibility();
        }

    }

    /// <summary>
    /// 敵の徘徊アルゴリズム
    /// </summary>
    void Patrol()
    {
        if (isWallEnemy)
        {
            float newZ = Mathf.Lerp(gameObject.transform.localScale.z, 0f, Time.deltaTime * shrinkSpeed);
            gameObject.transform.localScale = new Vector3(1, 1, newZ);
        }

        if (agent.enabled == true)
        {
            if (investigateTarget.HasValue) // 調査目標がある場合はInvestigateへ移行
            {
                currentState = State.Investigate;
                agent.speed = investigateSpeed;
                agent.SetDestination(investigateTarget.Value);
                investigateTimer = investigateDuration; // 調査タイマーをリセット
                return; // Patrolの残りの処理は行わない
            }

            // 目標地点にある程度近づけば
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending) // pathPendingもチェック
            {
                patrolIndex = (patrolIndex + 1) % DestPos.Length;
                agent.SetDestination(DestPos[patrolIndex]);
            }

            CheckDoorInPath();
            CheckWindowInPath();
        }

        // プレイヤーが視界に入ったら追跡に移行
        if (isPlayerVisible)
        {
            currentState = State.Chase;
            agent.speed = chaseSpeed; // 追跡速度に設定
            if (ShakeCamera.Instance != null) // nullチェックを追加
                ShakeCamera.Instance.Shake(0.5f, 1f);

            enemyManager.FindChasingEnemy();
            if (noticeAS != null && noticeSound != null) // 発覚音を鳴らす
            {
                noticeAS.PlayOneShot(noticeSound);
            }
        }
    }


    /// <summary>
    /// 敵の追跡アルゴリズム
    /// </summary>
    void Chase()
    {
        if (isWallEnemy)
        {
            float newZ = Mathf.Lerp(gameObject.transform.localScale.z, 1f, Time.deltaTime * shrinkSpeed);
            gameObject.transform.localScale = new Vector3(1, 1, newZ);
            agent.enabled = true;
        }

        if (playerTarget != null && playerTarget.activeInHierarchy) // プレイヤーがアクティブかチェック
        {
            agent.SetDestination(playerTarget.transform.position);
        }
        else
        {
            agent.ResetPath();
            currentState = State.Patrol;
            agent.speed = patrolSpeed; // 巡回速度に設定
            enemyManager.FindChasingEnemy(); // 追跡状態の敵がいなくなったことを通知
            return; // 追跡対象がいないのでここで終了
        }

        CheckDoorInPath();
        CheckWindowInPath();

        if (!isPlayerVisible)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= lostThreshold)
            {
                currentState = State.Patrol;
                lostTimer = 0f;
                agent.ResetPath();
                agent.speed = patrolSpeed; // 巡回速度に設定
                enemyManager.FindChasingEnemy(); // 追跡状態の敵がいなくなったことを通知
            }
        }
        else
        {
            lostTimer = 0f;
        }
    }

    /// <summary>
    /// 調査状態のアルゴリズム
    /// </summary>
    void Investigate()
    {
        if (!investigateTarget.HasValue) // 調査ターゲットがなければPatrolに戻る
        {
            currentState = State.Patrol;
            agent.speed = patrolSpeed;
            return;
        }

        agent.SetDestination(investigateTarget.Value);

        // 目的地に到達した、または調査時間が過ぎた場合
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending || investigateTimer <= 0f)
        {
            Debug.Log("調査終了。巡回に戻ります。");
            investigateTarget = null; // 調査ターゲットをクリア
            currentState = State.Patrol;
            agent.speed = patrolSpeed;
            agent.ResetPath(); // パスをリセットして次の巡回ポイントへ
            patrolIndex = (patrolIndex + 1) % DestPos.Length;
            agent.SetDestination(DestPos[patrolIndex]);
        }
        else
        {
            investigateTimer -= Time.deltaTime; // 調査タイマーを減らす
        }

        // 調査中にプレイヤーを見つけたらChaseへ移行
        if (isPlayerVisible)
        {
            currentState = State.Chase;
            agent.speed = chaseSpeed;
            if (ShakeCamera.Instance != null) // nullチェックを追加
                ShakeCamera.Instance.Shake(0.5f, 1f);
            enemyManager.FindChasingEnemy();
            if (noticeAS != null && noticeSound != null) // 発覚音を鳴らす
            {
                noticeAS.PlayOneShot(noticeSound);
            }
        }
    }

    #region 障害物通過処理

    #region ドア通過処理
    /// <summary>
    /// 移動中に閉まっているドアに直面したら処理を行う
    /// </summary>
    private void CheckDoorInPath()
    {
        RaycastHit hit;
        // agent.velocity.normalized を使うと、NavMeshAgentがまだ動いていない場合にZeroになる可能性があるので注意
        // transform.forward を使うか、agent.desiredVelocity.normalized を使う方が確実な場合も
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance) // パスがあり、目的地に到達していない場合にのみチェック
        {
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, out hit, doorCheckDistance))
            {
                DoorController door = hit.collider.GetComponent<DoorController>();
                if (door != null && !door.isOpen && !isWaiting) // ドアが閉まっていて、かつ待機中でない
                {
                    StartCoroutine(HandleDoorInteraction(door));
                }
            }
        }
    }

    /// <summary>
    /// 閉まっているドアに直面したときの処理
    /// </summary>
    private IEnumerator HandleDoorInteraction(DoorController door)
    {
        isWaiting = true; // 停止状態にする
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // 滑りを防ぐ

        yield return new WaitForSeconds(openingTime); // 1秒間停止

        if (door == null) // ドアが消滅していた場合など
        {
            isWaiting = false;
            agent.isStopped = false;
            yield break;
        }

        if (door.isLockedDoor) // ロックされていたら巡回ルートを変更
        {
            Debug.Log("ドアがロックされているため、巡回ルートを変更します。");
            agent.ResetPath();
            patrolIndex = (patrolIndex + 1) % DestPos.Length; // 次の巡回ポイントへ
            agent.SetDestination(DestPos[patrolIndex]);
        }
        else // ロックされていなければドアを開けて進む
        {
            Debug.Log("ドアを開けて進む！");
            if (!door.isOpen) door.ToggleDoor();
        }

        isWaiting = false; // 停止解除
        agent.isStopped = false;
    }
    #endregion


    #region 窓通過処理

    /// <summary>
    /// 移動中に閉まっている窓に直面したら処理を行う
    /// </summary>
    private void CheckWindowInPath()
    {
        RaycastHit hit;
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance) // パスがあり、目的地に到達していない場合にのみチェック
        {
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, out hit, doorCheckDistance))
            {
                WindowManager window = hit.collider.GetComponent<WindowManager>();
                if (window != null && !window.isOpen && !isWaiting) // 窓が閉まっていて、かつ待機中でない
                {
                    StartCoroutine(HandleWindowInteraction(window));
                }
            }
        }
    }

    /// <summary>
    /// 閉まっている窓に直面したときの処理
    /// </summary>
    private IEnumerator HandleWindowInteraction(WindowManager window)
    {
        isWaiting = true; // 停止状態にする
        agent.isStopped = true;
        agent.velocity = Vector3.zero; // 滑りを防ぐ

        yield return new WaitForSeconds(openingTime); // 1秒間停止

        if (window == null) // 窓が消滅していた場合など
        {
            isWaiting = false;
            agent.isStopped = false;
            yield break;
        }

        if (!window.isOpen) // 窓を開けて進む
        {
            Debug.Log("窓を開けて進む！");
            window.ToggleWindow();
        }

        isWaiting = false; // 停止解除
        agent.isStopped = false;
    }

    #endregion


    #endregion

    // 壁との距離を調整するメソッド
    void AdjustPositionIfNearWall()
    {
        float wallDistance = 4.0f; // 壁との適切な距離
        float doorDistance = 2.0f; // ドアとの適切な距離（少し近めに設定）

        float adjustSpeed = 0.06f; // 壁を回避する速度

        Vector3 adjustDirection = Vector3.zero;

        // 前方向の壁チェック (ドアも含む)
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitF, doorDistance))
        {
            if (hitF.collider.CompareTag("Wall") || hitF.collider.CompareTag("Door"))
            {
                adjustDirection -= transform.forward * adjustSpeed;
            }
        }

        // 後ろ方向の壁チェック
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hitB, wallDistance))
        {
            if (hitB.collider.CompareTag("Wall") || hitB.collider.CompareTag("Door"))
            {
                adjustDirection += transform.forward * adjustSpeed;
            }
        }

        // 右方向の壁チェック
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitR, wallDistance))
        {
            if (hitR.collider.CompareTag("Wall") || hitR.collider.CompareTag("Door"))
            {
                adjustDirection -= transform.right * adjustSpeed;
            }
        }

        // 左方向の壁チェック
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hitL, wallDistance))
        {
            if (hitL.collider.CompareTag("Wall") || hitL.collider.CompareTag("Door"))
            {
                adjustDirection += transform.right * adjustSpeed;
            }
        }

        // NavMeshAgent の移動を微調整
        if (adjustDirection != Vector3.zero)
        {
            // isWaiting中でなければagent.Moveを適用
            if (!isWaiting)
            {
                agent.Move(adjustDirection);
            }
        }
    }



    /// <summary>
    /// 索敵範囲内にプレイヤーがいるか確認するメソッド
    /// </summary>
    void CheckPlayerVisibility()
    {
        if (playerTarget == null)
        {
            isPlayerVisible = false;
            visibilityTimer = 0f; // タイマーをリセット
            return;
        }

        RaycastHit hit;
        Vector3 directionToPlayer = playerTarget.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // プレイヤーが検知範囲にいるかを確認（緑色のレイ範囲に入ると検知可能）
        if (distanceToPlayer <= detectionRadius)
        {
            // 敵とプレイヤーの間にレイを発生させる
            if (Physics.Linecast(transform.position, playerTarget.transform.position, out hit))
            {
                // プレイヤーなら（障害物が無いなら）
                if (hit.collider.gameObject == playerTarget)
                {
                    // プレイヤーが視界範囲内にいるかを確認(黄色のレイ範囲に入ると即発見)
                    if (Vector3.Angle(transform.forward, directionToPlayer) < fieldOfView / 2 && distanceToPlayer <= detectionRadius * 0.5f)
                    {
                        isPlayerVisible = true; // 即座に発見
                        visibilityTimer = 0f;   // タイマーをリセット
                        Debug.Log("目の前でプレイヤーを即座に発見しました。");
                    }
                    else
                    {
                        // **タイマーを使用する条件（後ろや遠距離）**
                        visibilityTimer += Time.deltaTime;
                        if (visibilityTimer >= visibilityThreshold)
                        {
                            isPlayerVisible = true;
                            Debug.Log("視認タイマーでプレイヤーを発見しました。");
                        }
                    }
                }
                else
                {
                    // 障害物がある場合
                    isPlayerVisible = false;
                    visibilityTimer = 0f; // タイマーをリセット
                }
            }
        }
        else // 検出範囲外になった場合
        {
            isPlayerVisible = false;
            visibilityTimer = 0f;
        }

        // 前方向の長い範囲でのチェック
        CheckCustomDetection();
    }

    /// <summary>
    /// 敵の前方向の範囲にプレイヤーが入ると追跡状態になる
    /// </summary>
    void CheckCustomDetection()
    {
        Vector3 forward = transform.forward;
        Vector3 ghostPosition = transform.position;

        RaycastHit hit;

        // 前方向に太い範囲で検出
        float detectionLength = detectionRadius * 1.5f; // 長さ
        float detectionWidth = detectionRadius; // 半径（幅）

        // SphereCastでプレイヤーを検出
        if (Physics.SphereCast(ghostPosition, detectionWidth / 2, forward, out hit, detectionLength)) // 半径を半分にした方が自然な検知範囲になるかも
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("プレイヤーが太い検知範囲内にいます！");
                isPlayerVisible = true;
            }
            // else {
            //     // SphereCastで見つかったがプレイヤーでなければ、視認フラグをリセットする場合
            //     // ただし、他の視認ロジックと競合しないように注意
            //     // isPlayerVisible = false;
            // }
        }
    }


    /// <summary>
    /// SoundEventManager.OnSoundEmitted のハンドラ
    /// </summary>
    private void OnSoundHeard(SoundEvent evt)
    {
        Debug.Log("音を立てました。");
        // すでにプレイヤー追跡中なら無視する
        if (currentState == State.Chase) return;

        // 距離フィルタ（任意）
        float d = Vector3.Distance(transform.position, evt.Position);
        if (d > detectionRadius * 1.5f) return; // 音の検知範囲をdetectionRadiusより少し広くしても良い

        investigateTarget = evt.Position;
        investigateTimer = investigateDuration;

        currentState = State.Investigate; // ★重要: 音を聞いたらInvestigate状態に移行
        agent.speed = investigateSpeed;
        agent.SetDestination(investigateTarget.Value);

        StartCoroutine(LowerPixelsPerUnitMultiplier());
        Debug.Log("音を立てたら感知されました。");
    }

    /// <summary>
    /// 火災報知器が鳴ったときに呼び出されるハンドラ
    /// </summary>
    private void OnAlarmSounded() // ★追加: アラームイベントのハンドラ
    {
        Debug.Log("GhostAI: 火災報知器の音が聞こえた！プレイヤーを探しに行く！");
        // アラームが鳴ったら強制的に追跡状態にする
        currentState = State.Chase;
        agent.speed = chaseSpeed;
        if (playerTarget != null && playerTarget.activeInHierarchy)
        {
            agent.SetDestination(playerTarget.transform.position); // プレイヤーの位置へ向かう
        }
        else
        {
            // プレイヤーが見つからない場合でも、どこか音源の方向へ向かわせるなどの対策も考慮
            // 現状ではplayerTargetがnullの場合Patrolに戻ってしまうので注意
            // 例: アラームが鳴った場所を追跡ターゲットにする
            // investigateTarget = FindObjectOfType<FireAlarmManager>().transform.position;
            // currentState = State.Investigate;
        }
        /*
        // 発覚音を鳴らす
        if (noticeAS != null && noticeSound != null)
        {
            noticeAS.PlayOneShot(noticeSound);
        }
        */
/*
        enemyManager.FindChasingEnemy(); // EnemyManagerに追跡が開始されたことを通知
    }


    // フィッシャー・イェーツのシャッフルアルゴリズムを利用した配列シャッフル
    void Shuffle(Vector3[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // 0からiまでのランダムなインデックスを取得
            Vector3 temp = array[i];                   // 現在の要素を一時保存
            array[i] = array[randomIndex];             // ランダムな位置の要素を現在の位置に
            array[randomIndex] = temp;                 // 一時保存した要素をランダムな位置に
        }
    }

    /// <summary>
    /// 敵の徘徊ポイントを指定した複数ポイントからランダムで決定
    /// </summary>
    void DestinationPosition()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("PatrolPoint");

        if (points.Length == 0)
        {
            Debug.LogWarning("PatrolPointタグの付いたオブジェクトが見つかりませんでした。巡回できません。");
            return; // ポイントがない場合は処理を中断
        }

        if (points.Length < DestPos.Length)
        {
            Debug.LogWarning($"巡回ポイントが不足しています。ポイントの数 ({points.Length}) が配列サイズ ({DestPos.Length}) より少ないです。配列サイズを調整するか、ポイントを増やしてください。");
            // 足りない場合は、取得できたポイントだけを使う
            System.Array.Resize(ref DestPos, points.Length); // 配列サイズを実際に取得できた数に合わせる
        }

        // 巡回ポイントの座標を取得
        for (int i = 0; i < points.Length && i < DestPos.Length; i++)
        {
            DestPos[i] = points[i].transform.position;
        }

        // フィッシャー・イェーツのアルゴリズムでシャッフル
        Shuffle(DestPos);
    }


    /// <summary>
    /// ノイズをアニメーションする処理
    /// </summary>
    /// <returns></returns>
    IEnumerator LowerPixelsPerUnitMultiplier()
    {
        if (soundEventManager == null || soundEventManager.noiseImage == null || soundEventManager.isPlayingNoise) yield break;

        soundEventManager.isPlayingNoise = true;
        soundEventManager.noiseImage.gameObject.SetActive(true);
        soundEventManager.noiseImage.pixelsPerUnitMultiplier = soundEventManager.normalPixelAmount;
        if (noticeAS != null && noticeSound != null) // nullチェックを追加
        {
            noticeAS.PlayOneShot(noticeSound); // 発覚音を鳴らすのはここでも良いかも
        }

        float currentValue = soundEventManager.noiseImage.pixelsPerUnitMultiplier;
        while (currentValue > soundEventManager.targetPixelsAmount)
        {
            currentValue -= soundEventManager.lowerPixelsSpeed * Time.deltaTime;
            soundEventManager.noiseImage.pixelsPerUnitMultiplier = Mathf.Max(currentValue, soundEventManager.targetPixelsAmount);
            yield return null;
        }

        soundEventManager.noiseImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(10.0f);
        soundEventManager.isPlayingNoise = false;
    }

    // レイで索敵範囲を可視化
    void OnDrawGizmos()
    {
        // ギズモ表示の基点を設定
        Vector3 ghostPosition = transform.position;
        Vector3 forward = transform.forward;

        // playerTargetがnullでもギズモは表示したいので、nullチェックは個別に行う
        // 1. 検知範囲を描画（円）
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ghostPosition, detectionRadius);

        // 2. 視界範囲を描画（扇形）
        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * forward;

        Gizmos.DrawRay(ghostPosition, leftBoundary * detectionRadius * 0.5f);
        Gizmos.DrawRay(ghostPosition, rightBoundary * detectionRadius * 0.5f);

        // 扇形を補完するための円弧を描画
        float step = fieldOfView / 20;
        for (float angle = -fieldOfView / 2; angle < fieldOfView / 2; angle += step)
        {
            Vector3 start = Quaternion.Euler(0, angle, 0) * forward * detectionRadius * 0.5f;
            Vector3 end = Quaternion.Euler(0, angle + step, 0) * forward * detectionRadius * 0.5f;
            Gizmos.DrawLine(ghostPosition + start, ghostPosition + end);
        }

        // 3. 検知範囲（前方に長い）
        Gizmos.color = Color.cyan;
        float detectionLength = detectionRadius * 1.5f;
        float detectionWidth = detectionRadius;
        // SphereCastのギズモ表示（視覚化を補助）
        // 中心線
        Gizmos.DrawLine(ghostPosition, ghostPosition + forward * detectionLength);
        // 球体部分の輪郭（前方）
        Gizmos.DrawWireSphere(ghostPosition + forward * detectionLength, detectionWidth / 2);
        // 球体部分の輪郭（後方）
        Gizmos.DrawWireSphere(ghostPosition, detectionWidth / 2);
        // 横の接続線
        Vector3 rightOffset = transform.right * detectionWidth / 2;
        Vector3 upOffset = transform.up * detectionWidth / 2;

        Gizmos.DrawLine(ghostPosition + rightOffset, ghostPosition + rightOffset + forward * detectionLength);
        Gizmos.DrawLine(ghostPosition - rightOffset, ghostPosition - rightOffset + forward * detectionLength);
        Gizmos.DrawLine(ghostPosition + upOffset, ghostPosition + upOffset + forward * detectionLength);
        Gizmos.DrawLine(ghostPosition - upOffset, ghostPosition - upOffset + forward * detectionLength);


        // 4. 視線（Raycast）を描画
        if (playerTarget != null)
        {
            Gizmos.color = isPlayerVisible ? Color.red : Color.blue;
            Gizmos.DrawLine(ghostPosition, playerTarget.transform.position);
        }

        // 巡回ポイントを描画
        Gizmos.color = Color.magenta;
        for (int i = 0; i < DestPos.Length; i++)
        {
            Gizmos.DrawSphere(DestPos[i], 0.5f);
        }

        // 現在の目標地点を描画
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(agent.destination, 0.7f);
            Gizmos.DrawLine(transform.position, agent.destination);
        }

        // 調査ターゲットを描画
        if (investigateTarget.HasValue)
        {
            Gizmos.color = Color.cyan; // 調査ターゲットの色
            Gizmos.DrawWireSphere(investigateTarget.Value, 1.0f); // 調査ターゲットを球体で描画
            Gizmos.DrawLine(transform.position, investigateTarget.Value); // ゴーストから調査ターゲットへの線を描画
        }
    }


    private IEnumerator enemyRandomPicthVoice()
    {
        isPlayingVoiceSound = true;
        yield return new WaitForSeconds(Random.Range(5f, 15f)); // ボイスの間隔をランダムにする

        if (audioSource != null && enemyVoiceSound != null)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f); // ピッチの範囲を調整
            audioSource.PlayOneShot(enemyVoiceSound);
        }

        isPlayingVoiceSound = false;
    }
}
*/
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum State { Patrol, Chase, Investigate };

public class GhostAI : MonoBehaviour
{
    #region Variables

    public State currentState;

    private GameObject playerTarget;
    private NavMeshAgent agent;
    private Vector3[] DestPos = new Vector3[45];
    private int patrolIndex = 0;
    private const float doorCheckDistance = 5.0f;
    private const float openingTime = 1.0f;

    public bool isWaiting = false;

    private GameObject currentTargetDoor;

    private float lostTimer = 0f;
    public float lostThreshold = 5f;
    private float visibilityTimer = 0f;
    public float visibilityThreshold = 3f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    private Vector3? investigateTarget;
    private float investigateTimer;
    public float investigateDuration = 3f;
    public float investigateSpeed = 3f;
    public AudioClip noticeSound;
    private AudioSource noticeAS;


    public float detectionRadius = 60f;
    public float fieldOfView = 150f;
    private bool isPlayerVisible = false;


    public bool isWallEnemy = false;
    public float shrinkSpeed = 0.3f;

    public float swayAmount = 0.5f;
    public float swaySpeed = 2f;

    [SerializeField] private AudioClip enemyVoiceSound;
    private AudioSource audioSource;
    private bool isPlayingVoiceSound = false;

    private CameraSwitcher cameraSwitcher;
    private EnemyManager enemyManager;
    private SoundEventManager soundEventManager;

    // ★修正: パズル追跡関連の公開変数
    [SerializeField] private Transform puzzlePointTransform;
    private bool puzzleFailed = false;

    #endregion

    void OnDestroy()
    {
        if (SoundEventManager.Instance != null)
            SoundEventManager.Instance.OnSoundEmitted -= OnSoundHeard;
    }

    private void OnEnable()
    {
        PicturePuzzleManager.OnPuzzleFailedTooManyTimes += OnPuzzleFailed;
    }

    private void OnDisable()
    {
        PicturePuzzleManager.OnPuzzleFailedTooManyTimes -= OnPuzzleFailed;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (GameObject.Find("EnemyNoticeAS") != null)
        {
            noticeAS = GameObject.Find("EnemyNoticeAS").GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("EnemyNoticeAS GameObject not found. Creating a new AudioSource.");
            noticeAS = gameObject.AddComponent<AudioSource>();
            noticeAS.clip = noticeSound;
            noticeAS.playOnAwake = false;
        }

        agent = GetComponent<NavMeshAgent>();

        playerTarget = GameObject.FindGameObjectWithTag("Player");
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        enemyManager = FindObjectOfType<EnemyManager>();
        soundEventManager = FindObjectOfType<SoundEventManager>();

        DestinationPosition();
        currentState = State.Patrol;
        agent.speed = patrolSpeed;


        if (SoundEventManager.Instance != null)
            SoundEventManager.Instance.OnSoundEmitted += OnSoundHeard;

        if (isWallEnemy) agent.enabled = false;
    }

    void Update()
    {
        if (isWaiting) return;

        if (!isPlayingVoiceSound) StartCoroutine(enemyRandomPicthVoice());

        AdjustPositionIfNearWall();

        if (playerTarget == null || !playerTarget.activeInHierarchy)
        {
            isPlayerVisible = false;
            if (currentState != State.Patrol)
            {
                currentState = State.Patrol;
                agent.ResetPath();
                agent.speed = patrolSpeed;
                enemyManager.FindChasingEnemy();
            }
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Investigate:
                Investigate();
                break;
        }

        if (playerTarget != null && playerTarget.activeInHierarchy)
        {
            CheckPlayerVisibility();
        }
    }

    void Patrol()
    {
        if (isWallEnemy)
        {
            float newZ = Mathf.Lerp(gameObject.transform.localScale.z, 0f, Time.deltaTime * shrinkSpeed);
            gameObject.transform.localScale = new Vector3(1, 1, newZ);
        }

        if (agent.enabled == true)
        {
            if (investigateTarget.HasValue)
            {
                currentState = State.Investigate;
                agent.speed = investigateSpeed;
                agent.SetDestination(investigateTarget.Value);
                investigateTimer = investigateDuration;
                return;
            }

            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                patrolIndex = (patrolIndex + 1) % DestPos.Length;
                agent.SetDestination(DestPos[patrolIndex]);
            }

            CheckDoorInPath();
            CheckWindowInPath();
        }

        if (isPlayerVisible)
        {
            currentState = State.Chase;
            agent.speed = chaseSpeed;
            if (ShakeCamera.Instance != null)
                ShakeCamera.Instance.Shake(0.5f, 1f);

            enemyManager.FindChasingEnemy();
            if (noticeAS != null && noticeSound != null)
            {
                noticeAS.PlayOneShot(noticeSound);
            }
        }
    }

    void Chase()
    {
        if (isWallEnemy)
        {
            float newZ = Mathf.Lerp(gameObject.transform.localScale.z, 1f, Time.deltaTime * shrinkSpeed);
            gameObject.transform.localScale = new Vector3(1, 1, newZ);
            agent.enabled = true;
        }

        if (playerTarget != null && playerTarget.activeInHierarchy)
        {
            if (puzzleFailed)
            {
                if (puzzlePointTransform != null && Vector3.Distance(transform.position, puzzlePointTransform.position) <= agent.stoppingDistance)
                {
                    Debug.Log("GhostAI: 絵画の座標に到達しました。巡回に戻ります。");
                    puzzleFailed = false;
                    currentState = State.Patrol;
                    agent.speed = patrolSpeed;
                    agent.ResetPath();
                }
                else if (puzzlePointTransform != null)
                {
                    // 絵画の座標に向かう
                    agent.SetDestination(puzzlePointTransform.position);
                }
                else
                {
                    // 座標が設定されていない場合は、追跡を諦めて巡回に戻る
                    Debug.LogWarning("GhostAI: パズルポイントが設定されていません。巡回に戻ります。");
                    puzzleFailed = false;
                    currentState = State.Patrol;
                    agent.speed = patrolSpeed;
                    agent.ResetPath();
                }
            }
            else
            {
                agent.SetDestination(playerTarget.transform.position);
            }
        }
        else
        {
            agent.ResetPath();
            currentState = State.Patrol;
            agent.speed = patrolSpeed;
            enemyManager.FindChasingEnemy();
            return;
        }

        CheckDoorInPath();
        CheckWindowInPath();

        if (!isPlayerVisible)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= lostThreshold)
            {
                currentState = State.Patrol;
                lostTimer = 0f;
                agent.ResetPath();
                agent.speed = patrolSpeed;
                enemyManager.FindChasingEnemy();
            }
        }
        else
        {
            lostTimer = 0f;
        }
    }

    void Investigate()
    {
        if (!investigateTarget.HasValue)
        {
            currentState = State.Patrol;
            agent.speed = patrolSpeed;
            return;
        }

        agent.SetDestination(investigateTarget.Value);

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending || investigateTimer <= 0f)
        {
            Debug.Log("調査終了。巡回に戻ります。");
            investigateTarget = null;
            currentState = State.Patrol;
            agent.speed = patrolSpeed;
            agent.ResetPath();
            patrolIndex = (patrolIndex + 1) % DestPos.Length;
            agent.SetDestination(DestPos[patrolIndex]);
        }
        else
        {
            investigateTimer -= Time.deltaTime;
        }

        if (isPlayerVisible)
        {
            currentState = State.Chase;
            agent.speed = chaseSpeed;
            if (ShakeCamera.Instance != null)
                ShakeCamera.Instance.Shake(0.5f, 1f);
            enemyManager.FindChasingEnemy();
            if (noticeAS != null && noticeSound != null)
            {
                noticeAS.PlayOneShot(noticeSound);
            }
        }
    }

    #region 障害物通過処理
    private void CheckDoorInPath()
    {
        RaycastHit hit;
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, out hit, doorCheckDistance))
            {
                DoorController door = hit.collider.GetComponent<DoorController>();
                if (door != null && !door.isOpen && !isWaiting)
                {
                    StartCoroutine(HandleDoorInteraction(door));
                }
            }
        }
    }

    private IEnumerator HandleDoorInteraction(DoorController door)
    {
        isWaiting = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        yield return new WaitForSeconds(openingTime);

        if (door == null)
        {
            isWaiting = false;
            agent.isStopped = false;
            yield break;
        }

        if (door.isLockedDoor)
        {
            Debug.Log("ドアがロックされているため、巡回ルートを変更します。");
            agent.ResetPath();
            patrolIndex = (patrolIndex + 1) % DestPos.Length;
            agent.SetDestination(DestPos[patrolIndex]);
        }
        else
        {
            Debug.Log("ドアを開けて進む！");
            if (!door.isOpen) door.ToggleDoor();
        }

        isWaiting = false;
        agent.isStopped = false;
    }
    #endregion

    #region 窓通過処理
    private void CheckWindowInPath()
    {
        RaycastHit hit;
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, out hit, doorCheckDistance))
            {
                WindowManager window = hit.collider.GetComponent<WindowManager>();
                if (window != null && !window.isOpen && !isWaiting)
                {
                    StartCoroutine(HandleWindowInteraction(window));
                }
            }
        }
    }

    private IEnumerator HandleWindowInteraction(WindowManager window)
    {
        isWaiting = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        yield return new WaitForSeconds(openingTime);

        if (window == null)
        {
            isWaiting = false;
            agent.isStopped = false;
            yield break;
        }

        if (!window.isOpen)
        {
            Debug.Log("窓を開けて進む！");
            window.ToggleWindow();
        }

        isWaiting = false;
        agent.isStopped = false;
    }
    #endregion

    void AdjustPositionIfNearWall()
    {
        float wallDistance = 4.0f;
        float doorDistance = 2.0f;

        float adjustSpeed = 0.06f;

        Vector3 adjustDirection = Vector3.zero;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitF, doorDistance))
        {
            if (hitF.collider.CompareTag("Wall") || hitF.collider.CompareTag("Door"))
            {
                adjustDirection -= transform.forward * adjustSpeed;
            }
        }

        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hitB, wallDistance))
        {
            if (hitB.collider.CompareTag("Wall") || hitB.collider.CompareTag("Door"))
            {
                adjustDirection += transform.forward * adjustSpeed;
            }
        }

        if (Physics.Raycast(transform.position, transform.right, out RaycastHit hitR, wallDistance))
        {
            if (hitR.collider.CompareTag("Wall") || hitR.collider.CompareTag("Door"))
            {
                adjustDirection -= transform.right * adjustSpeed;
            }
        }

        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit hitL, wallDistance))
        {
            if (hitL.collider.CompareTag("Wall") || hitL.collider.CompareTag("Door"))
            {
                adjustDirection += transform.right * adjustSpeed;
            }
        }

        if (adjustDirection != Vector3.zero)
        {
            if (!isWaiting)
            {
                agent.Move(adjustDirection);
            }
        }
    }

    void CheckPlayerVisibility()
    {
        if (playerTarget == null)
        {
            isPlayerVisible = false;
            visibilityTimer = 0f;
            return;
        }

        RaycastHit hit;
        Vector3 directionToPlayer = playerTarget.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRadius)
        {
            if (Physics.Linecast(transform.position, playerTarget.transform.position, out hit))
            {
                if (hit.collider.gameObject == playerTarget)
                {
                    if (Vector3.Angle(transform.forward, directionToPlayer) < fieldOfView / 2 && distanceToPlayer <= detectionRadius * 0.5f)
                    {
                        isPlayerVisible = true;
                        visibilityTimer = 0f;
                        Debug.Log("目の前でプレイヤーを即座に発見しました。");
                    }
                    else
                    {
                        visibilityTimer += Time.deltaTime;
                        if (visibilityTimer >= visibilityThreshold)
                        {
                            isPlayerVisible = true;
                            Debug.Log("視認タイマーでプレイヤーを発見しました。");
                        }
                    }
                }
                else
                {
                    isPlayerVisible = false;
                    visibilityTimer = 0f;
                }
            }
        }
        else
        {
            isPlayerVisible = false;
            visibilityTimer = 0f;
        }

        CheckCustomDetection();
    }

    void CheckCustomDetection()
    {
        Vector3 forward = transform.forward;
        Vector3 ghostPosition = transform.position;

        RaycastHit hit;

        float detectionLength = detectionRadius * 1.5f;
        float detectionWidth = detectionRadius;

        if (Physics.SphereCast(ghostPosition, detectionWidth / 2, forward, out hit, detectionLength))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("プレイヤーが太い検知範囲内にいます！");
                isPlayerVisible = true;
            }
        }
    }

    private void OnSoundHeard(SoundEvent evt)
    {
        Debug.Log("音を立てました。");
        if (currentState == State.Chase) return;

        float d = Vector3.Distance(transform.position, evt.Position);
        if (d > detectionRadius * 1.5f) return;

        investigateTarget = evt.Position;
        investigateTimer = investigateDuration;

        currentState = State.Investigate;
        agent.speed = investigateSpeed;
        agent.SetDestination(investigateTarget.Value);

        // StartCoroutine(LowerPixelsPerUnitMultiplier());
        Debug.Log("音を立てたら感知されました。");
    }

    private void OnPuzzleFailed()
    {
        Debug.Log("GhostAI: パズル失敗を検知！絵画の座標へ向かいます。");

        if (currentState != State.Chase)
        {
            currentState = State.Chase;
            agent.speed = chaseSpeed;

            // パズル失敗時にPicturePuzzleManagerの座標を取得して設定
            if (puzzlePointTransform != null)
            {
                agent.SetDestination(puzzlePointTransform.position);
                puzzleFailed = true;
            }
            else
            {
                Debug.LogError("GhostAI: パズルポイントが設定されていません。巡回に戻ります。");
                currentState = State.Patrol;
                agent.speed = patrolSpeed;
            }
        }

        if (noticeAS != null && noticeSound != null)
        {
            noticeAS.PlayOneShot(noticeSound);
        }
        if (ShakeCamera.Instance != null)
        {
            ShakeCamera.Instance.Shake(0.5f, 1f);
        }
        if (enemyManager != null)
        {
            enemyManager.FindChasingEnemy();
        }
    }

    void Shuffle(Vector3[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector3 temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    void DestinationPosition()
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag("PatrolPoint");

        if (points.Length == 0)
        {
            Debug.LogWarning("PatrolPointタグの付いたオブジェクトが見つかりませんでした。巡回できません。");
            return;
        }

        if (points.Length < DestPos.Length)
        {
            Debug.LogWarning($"巡回ポイントが不足しています。ポイントの数 ({points.Length}) が配列サイズ ({DestPos.Length}) より少ないです。配列サイズを調整するか、ポイントを増やしてください。");
            System.Array.Resize(ref DestPos, points.Length);
        }

        for (int i = 0; i < points.Length && i < DestPos.Length; i++)
        {
            DestPos[i] = points[i].transform.position;
        }

        Shuffle(DestPos);
    }

    private IEnumerator enemyRandomPicthVoice()
    {
        isPlayingVoiceSound = true;
        yield return new WaitForSeconds(Random.Range(5f, 15f));

        if (audioSource != null && enemyVoiceSound != null)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(enemyVoiceSound);
        }

        isPlayingVoiceSound = false;
    }

    void OnDrawGizmos()
    {
        Vector3 ghostPosition = transform.position;
        Vector3 forward = transform.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ghostPosition, detectionRadius);

        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * forward;

        Gizmos.DrawRay(ghostPosition, leftBoundary * detectionRadius * 0.5f);
        Gizmos.DrawRay(ghostPosition, rightBoundary * detectionRadius * 0.5f);

        float step = fieldOfView / 20;
        for (float angle = -fieldOfView / 2; angle < fieldOfView / 2; angle += step)
        {
            Vector3 start = Quaternion.Euler(0, angle, 0) * forward * detectionRadius * 0.5f;
            Vector3 end = Quaternion.Euler(0, angle + step, 0) * forward * detectionRadius * 0.5f;
            Gizmos.DrawLine(ghostPosition + start, ghostPosition + end);
        }

        Gizmos.color = Color.cyan;
        float detectionLength = detectionRadius * 1.5f;
        float detectionWidth = detectionRadius;

        Gizmos.DrawLine(ghostPosition, ghostPosition + forward * detectionLength);

        Gizmos.DrawWireSphere(ghostPosition + forward * detectionLength, detectionWidth / 2);
        Gizmos.DrawWireSphere(ghostPosition, detectionWidth / 2);

        Vector3 rightOffset = transform.right * detectionWidth / 2;
        Vector3 upOffset = transform.up * detectionWidth / 2;

        Gizmos.DrawLine(ghostPosition + rightOffset, ghostPosition + rightOffset + forward * detectionLength);
        Gizmos.DrawLine(ghostPosition - rightOffset, ghostPosition - rightOffset + forward * detectionLength);
        Gizmos.DrawLine(ghostPosition + upOffset, ghostPosition + upOffset + forward * detectionLength);
        Gizmos.DrawLine(ghostPosition - upOffset, ghostPosition - upOffset + forward * detectionLength);


        if (playerTarget != null)
        {
            Gizmos.color = isPlayerVisible ? Color.red : Color.blue;
            Gizmos.DrawLine(ghostPosition, playerTarget.transform.position);
        }

        Gizmos.color = Color.magenta;
        for (int i = 0; i < DestPos.Length; i++)
        {
            Gizmos.DrawSphere(DestPos[i], 0.5f);
        }

        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(agent.destination, 0.7f);
            Gizmos.DrawLine(transform.position, agent.destination);
        }

        if (investigateTarget.HasValue)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(investigateTarget.Value, 1.0f);
            Gizmos.DrawLine(transform.position, investigateTarget.Value);
        }
    }

    private IEnumerator LowerPixelsPerUnitMultiplier()
    {
        return null;
    }
}