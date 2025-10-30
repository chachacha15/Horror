using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // EventSystemに必要
using System.Collections;
using TMPro; // TextMeshProに必要

// 金庫にCollider（BoxColliderなど）が必須であることを示す
[RequireComponent(typeof(Collider))]
public class Safe_Manager : MonoBehaviour
{
    // [Header("金庫の設定")]
    // public string correctCode; // BathTub側から設定されるため、Inspectorでの設定は不要

    [Tooltip("金庫が開くアニメーションの最終的な扉のY軸角度")]
    public float openDoorAngle = -90f;
    [Tooltip("金庫が開くスピード（度/秒）")]
    public float doorOpenSpeed = 90f;

    // [Header("UIの関連付け")]
    [Tooltip("金庫の番号入力UIのCanvas全体")]
    public GameObject safeUiCanvas;
    [Tooltip("番号入力欄のInput Field (TMP)")]
    public TMP_InputField inputField;

    // [Header("参照するオブジェクト")]
    [Tooltip("金庫の扉パーツ")]
    public GameObject door;
    [Tooltip("金庫のダイヤルパーツ")]
    public GameObject dial;
    [Tooltip("金庫のハンドルパーツ")]
    public GameObject handle;

    [Tooltip("一時的に無効化したいプレイヤーの操作スクリプトなど（例: PlayerMovement）")]
    public MonoBehaviour otherInputManager;

    // --- 内部変数 ---
    [HideInInspector]
    public string correctCode; // BathTubからアクセスされるためpublic, Inspectorからは非表示
    private bool isOpen = false;


    void Start()
    {
        // 1. UI全体を非表示にする
        if (safeUiCanvas != null)
        {
            safeUiCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("[Safe_Manager] 'Safe Ui Canvas'が設定されていません。", this.gameObject);
        }

        // 2. InputFieldのリスナーを設定
        if (inputField != null)
        {
            inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputField.interactable = true;
            // 入力完了時（Enterキーか、フォーカスが外れた時）にValidateCodeを呼ぶ
            inputField.onEndEdit.AddListener(ValidateCode);
        }
        else
        {
            Debug.LogError("[Safe_Manager] 'Input Field'が設定されていません。", this.gameObject);
        }
    }

    // 金庫本体がクリックされた時
    void OnMouseDown()
    {
        // UIが表示されておらず、金庫もまだ開いていない場合
        if (!isOpen && safeUiCanvas != null && !safeUiCanvas.activeSelf)
        {
            OpenSafeUI();
        }
    }

    // 金庫の入力UIを開く処理
    // 金庫の入力UIを開く処理
    private void OpenSafeUI()
    {
        safeUiCanvas.SetActive(true); // UI全体を表示

        // 他の入力処理を無効化
        if (otherInputManager != null)
        {
            otherInputManager.enabled = false;
        }

        // --- ▼ 変更点 ここから ▼ ---
        // ゲーム全体の動作を一時停止
        Time.timeScale = 0f;
        // --- ▲ 変更点 ここまで ▲ ---

        // マウスカーソルを表示し、ロックを解除（FPSなどで使用している場合）
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // InputFieldに自動的にフォーカスする
        if (inputField != null)
        {
            EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            inputField.text = ""; // 以前の入力をクリア
        }
    }

    // UIを閉じて、ゲームの操作を元に戻す処理
    private void CloseSafeUIAndRestoreGame()
    {
        if (safeUiCanvas != null)
        {
            safeUiCanvas.SetActive(false); // UIを閉じる
        }

        // 他の入力処理を有効化
        if (otherInputManager != null)
        {
            otherInputManager.enabled = true;
        }

        // --- ▼ 変更点 ここから ▼ ---
        // ゲームの動作を再開
        Time.timeScale = 1f;
        // --- ▲ 変更点 ここまで ▲ ---

        // マウスカーソルを非表示にし、ロックする（FPSゲームの場合）
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 入力が完了した時 (Input Field の onEndEdit から呼ばれる)
    public void ValidateCode(string enteredCode)
    {
        // UIを閉じ、操作を元に戻す
        CloseSafeUIAndRestoreGame();

        // 1. 正解コードが設定されているか（空っぽでないか）をチェック
        // (BathTubを掃除する前は correctCode は空 なので、絶対に通らない)
        if (string.IsNullOrEmpty(correctCode))
        {
            Debug.LogWarning("[Safe_Manager] 正解のコードがまだ設定されていません。 (BathTubのイベント未発生)");
            return; // 処理を中断
        }

        // 2. 入力された値が空っぽでないかをチェック
        if (string.IsNullOrEmpty(enteredCode))
        {
            Debug.Log("コードが入力されていません。");
            return; // 処理を中断
        }

        // 3. 判定
        if (enteredCode == correctCode)
        {
            // 正解！
            StartCoroutine(OpenSafeAnimation());
        }
        else
        {
            // 不正解
            Debug.Log("不正解の番号です: " + enteredCode);
            // ここに「カチッ」などの不正解音を入れると良い
        }
    }

    // 金庫が開くアニメーション処理
    IEnumerator OpenSafeAnimation()
    {
        isOpen = true; // 一度開いたら、もうクリックしてもUIは開かない
        Debug.Log("金庫が開きます...");

        // (ドア、ダイヤル、ハンドルの回転アニメーション)
        if (door != null)
        {
            float currentY = door.transform.eulerAngles.y;
            if (currentY > 180f)
                currentY -= 360f;

            float targetY = openDoorAngle;

            while (currentY > targetY)
            {
                currentY -= doorOpenSpeed * Time.deltaTime;
                if (currentY < targetY)
                    currentY = targetY;

                door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, currentY, door.transform.eulerAngles.z);
                yield return null;
            }
        }

        if (dial != null)
        {
            dial.transform.localEulerAngles = new Vector3(dial.transform.localEulerAngles.x, dial.transform.localEulerAngles.y, -30f);
        }

        if (handle != null)
        {
            handle.transform.localEulerAngles = new Vector3(handle.transform.localEulerAngles.x, handle.transform.localEulerAngles.y, -15f);
        }

        Debug.Log("金庫が開きました！");
    }
}