using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class GhostAI : MonoBehaviour
{
    public enum State { Patrol, Chase };
    public State currentState;

    private GameObject playerTarget;          // プレイヤーのターゲット
    private NavMeshAgent agent;         // NavMeshAgent
    private Vector3[] DestPos = new Vector3[27]; // 巡回ポイントのリスト
    private int patrolIndex = 0;        // 巡回ポイントのインデックス
    private float lostTimer = 0f;       // プレイヤーを見失った時間
    public float lostThreshold = 5f;   // プレイヤーを見失うまでの時間（秒）

    public float detectionRadius = 10f; // プレイヤー検知の半径
    public float fieldOfView = 30f;     // 視界角度

    private bool isPlayerVisible = false; // プレイヤーが視界に入っているか
    private CameraSwitcher cameraSwitcher;



    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();

        DestinationPosition();// 巡回ポイントの初期化
        currentState = State.Patrol; // 初期状態は巡回

        

    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
        }

        CheckPlayerVisibility(); // プレイヤーが視界にいるか確認
    }


    void Patrol()
    {
        // 巡回ポイントを順に移動
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolIndex = (patrolIndex + 1) % DestPos.Length;
            agent.SetDestination(DestPos[patrolIndex]);
        }

        // プレイヤーが視界に入ったら追跡に移行
        if (isPlayerVisible)
        {
            currentState = State.Chase;
        }
    }

    void Chase()
    {
        // プレイヤーの位置を追いかける
        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.transform.position);
        }

        // プレイヤーが視界から外れた場合、タイマーを進める
        if (!isPlayerVisible)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= lostThreshold)
            {
                // 一定時間見失ったら巡回に戻る
                currentState = State.Patrol;
                lostTimer = 0f;
            }
        }
        else
        {
            // プレイヤーが視界にいる間はタイマーをリセット
            lostTimer = 0f;
        }
    }

    void CheckPlayerVisibility()
    {
        if (playerTarget == null)
        {
            Debug.LogWarning("playerTargetが設定されていません");
            return;
        }

        RaycastHit hit;

        // 無視するレイヤーマスクを設定（例: IgnoreRaycast）
        int layerMask = ~LayerMask.GetMask("IgnoreRaycast");

        // Linecastを実行して視線を確認
        if (Physics.Linecast(transform.position, playerTarget.transform.position, out hit, layerMask))
        {
            if (hit.collider.gameObject == playerTarget)
            {
                // プレイヤーに直接ヒットした場合
                Debug.Log("視線はプレイヤーに到達しています");
                isPlayerVisible = true;
            }
            else
            {
                // 障害物にヒットした場合
                Debug.Log($"視線が遮られているオブジェクト: {hit.collider.gameObject.name}");
                isPlayerVisible = false;
            }
        }
        else
        {
            // 視線に遮るものが何もない場合
            Debug.Log("視線が遮られていません");
            isPlayerVisible = true;
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

        Gizmos.DrawRay(ghostPosition, leftBoundary * detectionRadius);
        Gizmos.DrawRay(ghostPosition, rightBoundary * detectionRadius);

        // 扇形を補完するための円弧を描画
        float step = fieldOfView / 20; // 弧を分割するステップ
        for (float angle = -fieldOfView / 2; angle < fieldOfView / 2; angle += step)
        {
            Vector3 start = Quaternion.Euler(0, angle, 0) * forward * detectionRadius;
            Vector3 end = Quaternion.Euler(0, angle + step, 0) * forward * detectionRadius;
            Gizmos.DrawLine(ghostPosition + start, ghostPosition + end);
        }

        if (playerTarget == null)
        {
            Debug.LogWarning("playerTargetがnullです。プレイヤーが設定されていません。");
            return;
        }

        // 3. 視線（Raycast）を描画
        if (playerTarget != null)
        {
            Gizmos.color = isPlayerVisible ? Color.red : Color.blue;
            Gizmos.DrawLine(ghostPosition, playerTarget.transform.position);
        }
    }

}
