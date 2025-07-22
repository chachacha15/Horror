using TMPro;
using UnityEngine;
using System.Collections;

public class PillowMover : MonoBehaviour, IInteractable // IInteractableを実装
{
    [SerializeField] private float moveDistance = 0.5f; // 枕を動かす距離
    [SerializeField] private float moveSpeed = 2.0f;    // 枕が動く速度
    [SerializeField] private Vector3 moveDirection = Vector3.forward; // 枕が動く方向（ローカル座標）
    [SerializeField] private bool canBeMoved = true;    // 枕が動かせる状態か
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool hasMoved = false; // 一度動かしたかどうか

    public GameObject hiddenItem; // 枕の下に隠すアイテム（オプション）
    public AudioClip moveSound; // 枕を動かす音
    private AudioSource audioSource; // 音を再生するAudioSource

    // オプション：テキスト表示用
    public TextMeshProUGUI interactTextDisplay; // UIのテキスト要素 (LockedTextのようなもの)

    void Start()
    {
        originalPosition = transform.position;
        // hiddenItem が設定されていれば、最初は非表示にする
        if (hiddenItem != null)
        {
            hiddenItem.SetActive(false);
        }

        // AudioSource コンポーネントを取得 (枕オブジェクトにアタッチしておく)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // LockedTextのようなUIテキスト表示があれば、初期化
        if (interactTextDisplay == null)
        {
            // シーン内のTextMeshProUGUIを探すか、手動でアタッチするように設定
            // 簡単にするため、DoorControllerのようにFindObjectOfTypeで探す例
            interactTextDisplay = FindObjectOfType<DoorController>()?.lockedText; // DoorControllerのlockedTextを流用する想定
            if (interactTextDisplay == null)
            {
                Debug.LogWarning("PillowMover: interactTextDisplayが設定されていません。手動で設定するか、適切なTextMeshProUGUIをアタッチしてください。");
            }
        }
    }

    void Update()
    {
        // 枕が動いている途中の処理
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

            // 目標位置に十分に近づいたら移動完了
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
                canBeMoved = false; // 一度動かしたら動かせなくする

                // アイテムを出現させる
                if (hiddenItem != null)
                {
                    hiddenItem.SetActive(true);
                    Debug.Log("アイテムが出現しました！");
                    // アイテム出現時の効果音や演出を追加することもできます
                    // 例: audioSource.PlayOneShot(itemAppearSound);
                }
                Debug.Log("枕が動きました！");
            }
        }
    }

    // --- IInteractableの実装 ---

    public string GetInteractText()
    {
        if (hasMoved) return "動かせない"; // 一度動かしたらテキストを変える
        return "動かす";
    }

    public bool ShowInteractText => true; // テキスト表示を有効にする
    public bool ActivateCrosshair => true; // クロスヘアーを有効にする

    /// <summary>
    /// プレイヤーが枕をクリックしたときに呼ばれるメソッド
    /// </summary>
    public void Interact(GameObject targetObject)
    {
        Debug.Log("PillowMover: Interactメソッドが呼ばれました！");
        if (canBeMoved && !isMoving && !hasMoved)
        {
            // 動かす方向をローカル座標からワールド座標に変換
            Vector3 worldMoveDirection = transform.TransformDirection(moveDirection.normalized);
            targetPosition = originalPosition + worldMoveDirection * moveDistance;
            isMoving = true;
            hasMoved = true; // 動いたことを記録

            // 枕を動かす時の効果音を再生
            if (audioSource != null && moveSound != null)
            {
                audioSource.PlayOneShot(moveSound);
            }
            else
            {
                Debug.LogWarning("PillowMover: moveSoundが設定されていないか、AudioSourceが見つかりません。");
            }

            // オプション: "動かせない" テキストを一時的に表示
            StartCoroutine(DelayText("動かした"));

        }
        else if (hasMoved)
        {
            Debug.Log("この枕はもう動かせません。");
            StartCoroutine(DelayText("動かせない"));
        }
    }

    // DoorControllerのDelayTextを参考に、一時的なテキスト表示を追加
    private IEnumerator DelayText(string message)
    {
        if (interactTextDisplay != null)
        {
            string originalText = interactTextDisplay.text; // 元のテキストを保持
            interactTextDisplay.text = message;
            yield return new WaitForSeconds(1.0f); // 1秒間表示
            interactTextDisplay.text = originalText; // 元に戻す
        }
    }
}