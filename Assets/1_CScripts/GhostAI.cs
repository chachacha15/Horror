using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// 敵の状態(巡回状態か追跡状態)
public enum State { Patrol, Chase }; // **他のクラスからもアクセスできる**

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
    public float lostThreshold = 5f;   // プレイヤーを見失うまでの時間（秒）
    private float visibilityTimer = 0f; // プレイヤーが視界内に留まった時間
    public float visibilityThreshold = 3f; // 遠距離での発見までの時間（秒）
    public float patrolSpeed = 2f;      // 巡回時の速度
    public float chaseSpeed = 5f;       // 追跡時の速度

    // 調査 ( プレイヤーやギミックが音を立てると、調査に来る )
    private Vector3? investigateTarget;　// 調査先座標
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
    private DoorController doorController;
    private DoorManager doorManager;
    private SoundEventManager soundEventManager;

    #endregion

    void OnDestroy()
    {
        if (SoundEventManager.Instance != null)
            SoundEventManager.Instance.OnSoundEmitted -= OnSoundHeard;
    }

    void Start()
    {
        // 必要なコンポーネントを取得
        audioSource = GetComponent<AudioSource>();
        noticeAS = GameObject.Find("EnemyNoticeAS").GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();


        // 他クラスを取得
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        enemyManager = FindObjectOfType<EnemyManager>();
        doorController = FindObjectOfType<DoorController>();
        doorManager = FindObjectOfType<DoorManager>();
        soundEventManager = FindObjectOfType<SoundEventManager>();

        DestinationPosition(); // 巡回ポイントの初期化
        currentState = State.Patrol;    // 初期状態は巡回
        agent.speed = patrolSpeed;      // 初期速度を巡回速度に設定


                SoundEventManager.Instance.OnSoundEmitted += OnSoundHeard;

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
            // Z軸を徐々に 2D に近づける
            float newZ = Mathf.Lerp(gameObject.transform.localScale.z, 0f, Time.deltaTime * shrinkSpeed);
            gameObject.transform.localScale = new Vector3(1, 1, newZ);
        }

        if (agent.enabled == true)
        {
            // 目標地点にある程度近づけば
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 調査状態 時 、 到着した or タイムアップしたら調査終了
                if (investigateTarget.HasValue || investigateTimer <= 0f)
                {
                    // 元の状態に戻す
                    investigateTarget = null;
                    agent.speed = (currentState == State.Chase) ? chaseSpeed : patrolSpeed;

                }
                patrolIndex = (patrolIndex + 1) % DestPos.Length;
                agent.SetDestination(DestPos[patrolIndex]);
            }

            // --- 調査フロー ---
            if (investigateTarget.HasValue)
            {
                // 調査モード中は調査速度で目的地へ移動
                agent.speed = investigateSpeed;
                agent.SetDestination(investigateTarget.Value);

                investigateTimer -= Time.deltaTime;            
                
            }

            // 開閉する障害物があるかチェック
            CheckDoorInPath();
            CheckWindowInPath();
        }

        // プレイヤーが視界に入ったら追跡に移行
        if (isPlayerVisible)
        {
            currentState = State.Chase;
            agent.speed = chaseSpeed; // 追跡速度に設定
            ShakeCamera.Instance.Shake(0.5f, 1f);

            enemyManager.FindChasingEnemy();

        }
    }


    /// <summary>
    /// 敵の追跡アルゴリズム
    /// </summary>
    void Chase()
    {
        if (isWallEnemy)
        {
            // 顔を徐々に 3D にする
            float newZ = Mathf.Lerp(gameObject.transform.localScale.z, 1f, Time.deltaTime * shrinkSpeed);
            gameObject.transform.localScale = new Vector3(1, 1, newZ);

            // 追跡を開始する
            agent.enabled = true;

        }


        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.transform.position);
        }
        else
        {
            agent.ResetPath();
            currentState = State.Patrol;
            agent.speed = patrolSpeed; // 巡回速度に設定
            enemyManager.FindChasingEnemy();

        }

        // 開閉する障害物があるかチェック
        CheckDoorInPath();
        CheckWindowInPath();

        if (!isPlayerVisible)
        {
            // ある時間プレイヤーが見えないと、追跡を停止
            lostTimer += Time.deltaTime;
            if (lostTimer >= lostThreshold)
            {
                currentState = State.Patrol;
                lostTimer = 0f;
                agent.ResetPath();
                agent.speed = patrolSpeed; // 巡回速度に設定

                enemyManager.FindChasingEnemy();

            }
        }
        else
        {
            lostTimer = 0f;
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
        if (Physics.Raycast(transform.position, agent.velocity.normalized, out hit, doorCheckDistance))
        {
            DoorController door = hit.collider.GetComponent<DoorController>();
            if (door != null && !door.isOpen) // ドアが閉まっている
            {
                //currentTargetDoor = door;
                StartCoroutine(HandleDoorInteraction(door));
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
        if (Physics.Raycast(transform.position, agent.velocity.normalized, out hit, doorCheckDistance))
        {
            WindowManager window = hit.collider.GetComponent<WindowManager>();
            if (window != null && !window.isOpen) // ドアが閉まっている
            {
                StartCoroutine(HandleWindowInteraction(window));
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

        if (!window.isOpen) // ドアを開けて進む
        {
            Debug.Log("ドアを開けて進む！");
            if (!window.isOpen) window.ToggleWindow();
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
        float doorDistance = 2.0f; // 壁との適切な距離

        float adjustSpeed = 0.06f; // 壁を回避する速度

        Vector3 adjustDirection = Vector3.zero;

        // 前方向の壁チェック
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitF, doorDistance))
        {
            if (hitF.collider.CompareTag("Door"))
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
            agent.Move(adjustDirection);
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
        if(distanceToPlayer <= detectionRadius)
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

        if (Physics.SphereCast(ghostPosition, detectionWidth, forward, out hit, detectionLength))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("プレイヤーが太い検知範囲内にいます！");
                isPlayerVisible = true;

            }
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
        if (d > detectionRadius) return;


        // evt.Position を調査ターゲットにセットするなどの処理
        investigateTarget = evt.Position;
        investigateTimer = investigateDuration;

        // 追跡中でなければ、ここで調査モードに入ります
        StartCoroutine(LowerPixelsPerUnitMultiplier());
        Debug.Log("音を立てたら感知されました。");

    }


    // フィッシャー・イェーツのシャッフルアルゴリズムを利用した配列シャッフル
    void Shuffle(Vector3[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // 0からiまでのランダムなインデックスを取得
            Vector3 temp = array[i];                 // 現在の要素を一時保存
            array[i] = array[randomIndex];           // ランダムな位置の要素を現在の位置に
            array[randomIndex] = temp;               // 一時保存した要素をランダムな位置に
        }
    }

    /// <summary>
    /// 敵の徘徊ポイントを指定した複数ポイントからランダムで決定
    /// </summary>
    void DestinationPosition()
    {
        

        GameObject[] points = GameObject.FindGameObjectsWithTag("PatrolPoint");

        if (points.Length < DestPos.Length)
        {
            Debug.LogWarning("巡回ポイントが不足しています。ポイントの数を増やすか、配列サイズを調整してください。");
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
        // 重複して発動しないように
        if (soundEventManager.noiseImage == null || soundEventManager.isPlayingNoise) yield break;

        // ノイズオン
        soundEventManager.isPlayingNoise = true;
        soundEventManager.noiseImage.gameObject.SetActive(true);
        soundEventManager.noiseImage.pixelsPerUnitMultiplier = soundEventManager.normalPixelAmount;
        noticeAS.Play();

        // ノイズアニメーション
        float currentValue = soundEventManager.noiseImage.pixelsPerUnitMultiplier;
        while (currentValue > soundEventManager.targetPixelsAmount)
        {
            currentValue -= soundEventManager.lowerPixelsSpeed * Time.deltaTime;
            soundEventManager.noiseImage.pixelsPerUnitMultiplier = Mathf.Max(currentValue, soundEventManager.targetPixelsAmount);
            yield return null;
        }


        // ノイズオフ
        soundEventManager.noiseImage.gameObject.SetActive(false);
        
        // 向こう１０秒間はノイズ演出を発動しないようにする
        yield return new WaitForSeconds(10.0f);
        soundEventManager.isPlayingNoise = false;
    }

    // レイで索敵範囲を可視化
    void OnDrawGizmos()
    {
        if (playerTarget == null)
            return;

        // ゴーストの位置
        Vector3 ghostPosition = transform.position;

        // 1. 検知範囲を描画（円）
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ghostPosition, detectionRadius);

        // 2. 視界範囲を描画（扇形）
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * forward;

        Gizmos.DrawRay(ghostPosition, leftBoundary * detectionRadius * 0.5f);
        Gizmos.DrawRay(ghostPosition, rightBoundary * detectionRadius * 0.5f);

        // 扇形を補完するための円弧を描画
        float step = fieldOfView / 20; // 弧を分割するステップ
        for (float angle = -fieldOfView / 2; angle < fieldOfView / 2; angle += step)
        {
            Vector3 start = Quaternion.Euler(0, angle, 0) * forward * detectionRadius * 0.5f;
            Vector3 end = Quaternion.Euler(0, angle + step, 0) * forward * detectionRadius * 0.5f;
            Gizmos.DrawLine(ghostPosition + start, ghostPosition + end);
        }

        if (playerTarget == null)
        {
            Debug.LogWarning("playerTargetがnullです。プレイヤーが設定されていません。");
            return;
        }


        // 3. 検知範囲（前方に長い）
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(ghostPosition, ghostPosition + forward * detectionRadius * 1.5f);

        // 幅を表す線
        Gizmos.DrawLine(ghostPosition - transform.right * detectionRadius / 2,
                        ghostPosition + transform.forward * detectionRadius * 1.5f - transform.right * detectionRadius / 2);
        Gizmos.DrawLine(ghostPosition + transform.right * detectionRadius / 2,
                        ghostPosition + transform.forward * detectionRadius * 1.5f + transform.right * detectionRadius / 2);

        // 4. 視線（Raycast）を描画
        if (playerTarget != null)
        {
            Gizmos.color = isPlayerVisible ? Color.red : Color.blue;
            Gizmos.DrawLine(ghostPosition, playerTarget.transform.position);
        }

        // 巡回ポイントを描画
        Gizmos.color = Color.magenta; // 巡回ポイントの色
        for (int i = 0; i < DestPos.Length; i++)
        {
            Gizmos.DrawSphere(DestPos[i], 0.5f); // 巡回ポイントを球体で描画
        }

        // 現在の目標地点を描画
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.red; // 現在の目標地点の色
            Gizmos.DrawSphere(agent.destination, 0.7f); // 現在の目標地点を少し大きめの球体で描画
            Gizmos.DrawLine(transform.position, agent.destination); // ゴーストから目標地点への線を描画
        }
    }


   private IEnumerator enemyRandomPicthVoice()
    {
        isPlayingVoiceSound = true;
        yield return new WaitForSeconds(1);

        audioSource.pitch = Random.RandomRange(0.1f, 0.5f);

        isPlayingVoiceSound = false;

    }





}


