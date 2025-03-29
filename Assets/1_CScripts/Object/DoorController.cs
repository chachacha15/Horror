using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask layerMask; // レイキャストの対象レイヤー

    public Animator animator; // ドアのAnimator
    public Transform player; // プレイヤーのTransform
    private float interactionDistance = 6f; // ドアとのインタラクション距離

    public bool isOpen = false;
    private bool isLookingAtDoor = false; // クローゼットを見ている状態か

    public GameObject interactCanvas;
    public GameObject doortext;
    TextMeshProUGUI doorGUI;

    public Image crosshair;   // クロスヘアのImageコンポーネント
    private float currentSize; // 現在のサイズ
    CameraSwitcher cameraSwitcher;

    public bool isLockedDoor = true; // ドアがしまっているか
    private string requiredKeyName; // 必要なカギの名前

    private AudioSource audioSource; // 音を再生するAudioSource
    public AudioClip UnLockSound; // 開錠音
    public AudioClip CardKeySound; // ピッというカードキー認証音
    public AudioClip LockedSound; // ガチャガチャという開けられない音

    public Inventory inventory; // プレイヤーのインベントリ

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        inventory = FindObjectOfType<Inventory>();


        // 自分の親オブジェクトを取得
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // 親の子供の中からCanvasを探す
            Transform canvasTransform = parentTransform.Find("Canvas");
            interactCanvas = canvasTransform.gameObject;
            if (canvasTransform != null)
            {
                // Canvasの子供の中から開閉Textを探す
                Transform textTransform = canvasTransform.Find("開閉Text");

                if (textTransform != null)
                {
                    // Textコンポーネントを取得
                    doortext = textTransform.gameObject;
                }
            }
        }

        // TextMeshProUGUIへの参照
        doorGUI = doortext.GetComponent<TextMeshProUGUI>();
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        animator = GetComponent<Animator>();

        // AnimatorのisOpenパラメータを初期状態に同期
        if (animator != null)  animator.SetBool("isOpen", isOpen);

        //MainCameraをタグで動的に取得
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


        // オブジェクト名から数字を抽出して必要なカギを設定
        requiredKeyName = GetRequiredKeyNameFromObjectName(gameObject.name);
        if(requiredKeyName == null) isLockedDoor= false;
    }
    void Update()
    {

        // クローゼットにカーソルがあるかを判定
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // デバッグ用：レイキャストの可視化
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.magenta);

        // プレイヤーが近づいたらClickで開閉
        if (Physics.Raycast(ray, out hit, interactionDistance, layerMask))
        {
            if (hit.collider.gameObject == gameObject) // 現在のドアに一致する場合
            {
                isLookingAtDoor = true;
                if (doortext != null) interactCanvas.SetActive(true); // 開閉Textを表示

            }
            else
            {
                isLookingAtDoor = false;
                if (doortext != null) interactCanvas.SetActive(false); // 開閉Textを非表示

            }
        }
        else
        {
            isLookingAtDoor = false;
            if (doortext != null) interactCanvas.SetActive(false); // 開閉Textを非表示

            cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingAtDoor);
        }

        cameraSwitcher.ClosshairAnimation(10f, 500f, 0.5f, cameraSwitcher.crosshairRectTransform, isLookingAtDoor);

        // 左クリック時にドアを開閉
        if (Input.GetMouseButtonDown(0) && isLookingAtDoor)
        {
            if (isLockedDoor)
            {
                if (HasRequiredKey())
                {
                    isLockedDoor = false;
                    audioSource.PlayOneShot(CardKeySound);
                    StartCoroutine(PlaySoundWithDelay(UnLockSound, 0.35f));
                }
                else
                {
                    audioSource.PlayOneShot(LockedSound);
                    StartCoroutine(DelayText());
                }
            }
            else
            {
                ToggleDoor();
            }

        }



    }

    public void ToggleDoor()
    {

        isOpen = !isOpen;

        animator.SetBool("isOpen", isOpen);


        if (doorGUI != null) doorGUI.text = isOpen ? "閉める" : "開ける";

    }


    // 指定した音を指定した遅延時間後に再生
    private IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay); // 指定した秒数だけ待つ
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // 音を再生
        }
    }

    // オブジェクト名から必要なカギの名前を取得
    private string GetRequiredKeyNameFromObjectName(string objectName)
    {
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(objectName, @"\d+");
        Debug.Log(objectName);
        Debug.Log(match.Value);
        if (match.Success)
        {
            return $"カードキー({match.Value}号室)"; // 必要なカギの名前を生成
        }
        else
        {
            return null; // 数字がない場合はカギ不要
        }
    }

    // 必要なカギを持っているか確認
    private bool HasRequiredKey()
    {
        if (inventory.selectedItem != null && inventory.selectedItem.item.name == requiredKeyName)
        {
            return true; // カギを持っている
        }
        
        return false; // カギがない
    }

    IEnumerator DelayText()
    {
        doorGUI.text = "開かない";
        yield return new WaitForSeconds(1.0f);
        doorGUI.text = "開ける";
    }
}


/*
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    #region ドア関連
    [Header("Door Settings")]
    [SerializeField] private Animator doorAnimator; // ドアのAnimator
    public bool isOpen = false;                   // ドアの開閉状態
    [SerializeField] private AudioSource audioSource; // ドア操作時のサウンド用
    [SerializeField] private AudioClip unlockSound; // 正解時のサウンド
    [SerializeField] private AudioClip lockedSound; // 間違い時のサウンド（またはロック時のサウンド）
    #endregion

    #region キーパッドUI関連
    [Header("Keypad UI Settings")]
    [SerializeField] private GameObject keypadPanel; // キーパッドのUIパネル
    [SerializeField] private TMP_Text inputText;       // 入力された番号を表示するText
    [SerializeField] private TMP_Text messageText;     // メッセージ表示用Text（正解、不正解など）
    [SerializeField] private string correctNumber = "1234"; // このドアを開くための正解番号
    #endregion


    public bool isLockedDoor = true; // ドアがしまっているか


    #region その他
    private string currentInput = "";     // 現在の入力状態
    private bool isKeypadActive = false;  // キーパッドが表示中か
    #endregion

    void Start()
    {
        // キーパッドは初期状態非表示
        keypadPanel.SetActive(false);
        messageText.text = "";
    }

    // ドアオブジェクトがクリックされたとき（Collider付きのオブジェクトにこのスクリプトを付ける）
    private void OnMouseDown()
    {
        // キーパッドが非表示なら表示する
        if (!isKeypadActive)
        {
            ToggleKeypad();
        }
    }

    /// <summary>
    /// キーパッドパネルの表示/非表示を切り替え、時間停止やカーソル表示も切り替える
    /// </summary>
    public void ToggleKeypad()
    {
        isKeypadActive = !isKeypadActive;
        keypadPanel.SetActive(isKeypadActive);

        if (isKeypadActive)
        {
            // キーパッド表示中は時間停止、カーソル表示
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // キーパッド非表示時は通常状態に戻す
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            messageText.text = "";
            currentInput = "";
            inputText.text = "";
        }
    }

    /// <summary>
    /// キーパッドの数字ボタンが押されたときに呼ばれる
    /// </summary>
    /// <param name="key">押された数字（文字列）</param>
    public void PressKey(string key)
    {
        if (!isKeypadActive) return;
        if (currentInput.Length < 4)
        {
            currentInput += key;
            inputText.text = currentInput;
            // オプション：ボタン音を再生するなど
        }
    }

    /// <summary>
    /// バックスペースボタンが押されたとき
    /// </summary>
    public void Backspace()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            inputText.text = currentInput;
        }
    }

    /// <summary>
    /// 「Call」ボタンが押され、入力番号をチェックする
    /// </summary>
    public void Call()
    {
        if (!isKeypadActive) return;

        if (currentInput == correctNumber)
        {
            messageText.text = "正しい番号です。";
            if (unlockSound != null)
                audioSource.PlayOneShot(unlockSound);

            // 正解の場合、ドアを開く
            ToggleDoor();
        }
        else
        {
            messageText.text = "番号が間違っています。";
            if (lockedSound != null)
                audioSource.PlayOneShot(lockedSound);
        }
        StartCoroutine(ClearMessageAfterDelay(2.0f));
        currentInput = "";
        inputText.text = "";
    }

    /// <summary>
    /// 指定秒数後にメッセージをクリアするコルーチン
    /// </summary>
    /// <param name="delay">待機時間（秒）</param>
    /// <returns></returns>
    IEnumerator ClearMessageAfterDelay(float delay)
    {
        // 時間停止中でも進むように unscaledDeltaTime を使う
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        messageText.text = "";
    }

    /// <summary>
    /// ドアの開閉を切り替え、アニメーションを再生する
    /// </summary>
    public void ToggleDoor()
    {
        isOpen = !isOpen;
        doorAnimator.SetBool("isOpen", isOpen);
        // UI上に「開ける」「閉める」などの表示を更新する場合はここで行う

        // ドア操作後、キーパッドを閉じ、時間を再開
        ToggleKeypad();
    }
}
*/