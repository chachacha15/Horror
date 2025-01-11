using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class GhostAI : MonoBehaviour
{
    public enum State { Patrol, Chase };
    public State currentState;

    private GameObject playerTarget;    // プレイヤーのターゲット
    private NavMeshAgent agent;         // NavMeshAgent
    private Vector3[] DestPos = new Vector3[27]; // 巡回ポイントのリスト
    private int patrolIndex = 0;        // 巡回ポイントのインデックス
    private float lostTimer = 0f;       // プレイヤーを見失った時間
    public float lostThreshold = 5f;   // プレイヤーを見失うまでの時間（秒）
    private float visibilityTimer = 0f; // プレイヤーが視界内に留まった時間
    public float visibilityThreshold = 3f; // 遠距離での発見までの時間（秒）
    public float patrolSpeed = 2f;      // 巡回時の速度
    public float chaseSpeed = 5f;       // 追跡時の速度

    public float detectionRadius = 20f; // プレイヤー検知の半径
    public float fieldOfView = 150f;     // 視界角度

    private bool isPlayerVisible = false; // プレイヤーが視界に入っているか

    //敵の揺れ
    public float swayAmount = 0.5f; // 揺れの幅
    public float swaySpeed = 2f;    // 揺れの速さ
    private float swayOffset = 0f;

    private CameraSwitcher cameraSwitcher;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();

        DestinationPosition(); // 巡回ポイントの初期化
        currentState = State.Patrol;    // 初期状態は巡回
        agent.speed = patrolSpeed;      // 初期速度を巡回速度に設定
    }

    void Update()
    {
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

    void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolIndex = (patrolIndex + 1) % DestPos.Length;
            agent.SetDestination(DestPos[patrolIndex]);
        }

        // プレイヤーが視界に入ったら追跡に移行
        if (isPlayerVisible)
        {
            currentState = State.Chase;
            agent.speed = chaseSpeed; // 追跡速度に設定
        }
    }

    void Chase()
    {
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.transform.position);
        }
        else
        {
            agent.ResetPath();
            currentState = State.Patrol;
            agent.speed = patrolSpeed; // 巡回速度に設定
        }

        if (!isPlayerVisible)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= lostThreshold)
            {
                currentState = State.Patrol;
                lostTimer = 0f;
                agent.ResetPath();
                agent.speed = patrolSpeed; // 巡回速度に設定
            }
        }
        else
        {
            lostTimer = 0f;
        }
    }

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
            // ここに走ると発見されるコードを書く

            //

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
        //CheckCustomDetection();
    }

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


    void DestinationPosition()
    {
        //DestPos[0] = new Vector3(-20.03f, -1.08f, 5.17f);
        //DestPos[1] = new Vector3(-14.34f, 1.08f, 8.35f);
        //DestPos[2] = new Vector3(-6.99f, 1.08f, 10.18f);
        //DestPos[3] = new Vector3(-0.18f, 1.10f, 10.60f);
        //DestPos[4] = new Vector3(6.44f, 1.08f, 10.10f);
        //DestPos[5] = new Vector3(13.73f, 1.08f, 8.47f);
        //DestPos[6] = new Vector3(20.50f, 1.08f, 5.02f);
        //DestPos[7] = new Vector3(24.05f, 1.08f, 0.28f);
        //DestPos[8] = new Vector3(20.82f, 1.08f, -4.80f);
        //DestPos[9] = new Vector3(14.36f, 1.08f, -8.12f);
        //DestPos[10] = new Vector3(7.19f, 1.08f, -9.97f);
        //DestPos[11] = new Vector3(0.13f, 1.12f, -10.64f);
        //DestPos[12] = new Vector3(-6.60f, 1.08f, -9.66f);
        //DestPos[13] = new Vector3(-13.93f, 1.08f, -7.96f);
        //DestPos[14] = new Vector3(-20.00f, 1.08f, -4.61f);
        //DestPos[15] = new Vector3(-23.44f, 1.11f, -0.17f);


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
    }
    

}


