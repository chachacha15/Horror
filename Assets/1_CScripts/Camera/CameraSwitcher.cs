using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityStandardAssets.Utility; // CurveControlledBob を使用

public class CameraSwitcher : MonoBehaviour, IInteractable
{

    #region Variables

    public static CameraSwitcher Instance;


    [Header("General Settings")]
    public Camera mainCamera;
    public float hideDistance = 5f;    // 隠れられる距離
    [SerializeField] private Vector3 baseLocalRotation; // 隠れ状態カメラの基本向き



    [Header("Hiding Cross Hair Settings")]
    public Image crosshair;   // クロスヘアのImageコンポーネント
    public Sprite normalCrosshair; // 通常時のスプライト
    public Sprite closetCrosshair; // 隠れ状態時のスプライト
    public List<BoolWrapper> activeCrosshairBoolList; // クロスヘアに関わるboolを格納
    public float crosshairDurarion = 8f; // アニメーションスピード
    public float crosshairNormalSize = 10f; // クロスヘアのノーマルサイズ
    public float crosshairActiveSize = 30f; // アクティブサイズ
    private float currentSize; // 現在のサイズ
    public RectTransform crosshairRectTransform; // クロスヘアのRectTransform



    // サウンド
    [Header("Sound Settings")]
    [SerializeField] private AudioClip hideSound;
    private AudioSource audioSource;

    // カメラ揺れ用
    [Header("Hiding Bob Settings")]
    [SerializeField] private CurveControlledBob bob = new CurveControlledBob();


    // 状態管理用
    public bool isPlayerHiding = false; // プレイヤーが隠れているかどうか
    public bool CanControl = true; // プレイヤーがカメラ切り替えをできるかどうか
    private bool isClosetCameraActive = false; // 現在のカメラ状態を追跡
    public bool hasHiddenUnderDesk = false; //一回は隠れ状態に隠れたことがあるか
    public Camera currentClosetCamera; // 現在の隠れ状態カメラを追跡


    // インスタンス保存用
    private GameObject player;
    public Transform closetCameraTransform; // 隠れ状態カメラのTransformを保持


    // 他クラス
    ShakeCamera shakeCamera;
    private GameStateManager gameStateManager;
    private PlayerLook playerLook;
    private PlayerInteractor playerInteractor;


    #endregion

    #region Interactable (IInteractable)

    public string GetInteractText()
    {
        if (!isPlayerHiding) return "隠れる";
        return "";
    }

    public bool ShowInteractText => !isPlayerHiding; // テキスト表示するかどうか
    public bool ActivateCrosshair => !isPlayerHiding;

    /// <summary>
    /// クリック時、隠れる
    /// </summary>
    public void Interact(GameObject targetObject)
    {

        // 左クリックで隠れ状態カメラに切り替える
        if (!isClosetCameraActive)
        {

            // 隠れ状態カメラに切り替え
            Camera targetClosetCamera = FindClosetCamera(targetObject);

            if (targetClosetCamera != null)
            {

                SwitchToHidingCamera(targetClosetCamera);
                targetClosetCamera.transform.localPosition = new Vector3(
                    targetClosetCamera.transform.localPosition.x,
                    0,
                    targetClosetCamera.transform.localPosition.z
                    );

                //カメラ揺れのセットアップ
                bob.Setup(targetClosetCamera, 1.0f);


                if (!hasHiddenUnderDesk && GameStateManager.Instance.HasMetEnemy)
                {
                    StartCoroutine(HidingUnderDesk.Instance.ActivateHidingEvent());

                }

            }
            else
            {
                Debug.LogWarning("対象の隠れ状態にカメラが見つかりません！");
            }


            
        }
    }

    #endregion



    #region Methods

    private void Awake()
    {
        activeCrosshairBoolList = new List<BoolWrapper>();

        shakeCamera = ShakeCamera.Instance;
        Instance = this;

    }

    private void Start()
    {
        // 他クラスを取得
        gameStateManager = GameStateManager.Instance;
        playerLook = PlayerLook.Instance;
        playerInteractor = PlayerInteractor.Instance;

        mainCamera = Camera.main; // メインカメラを動的に取得
        player = GameObject.FindWithTag("Player");


        // サウンド初期設定
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        // ゲーム状態が隠れている状態でなければ何もしない
        if (gameStateManager.CurrentGameState != GameState.Hiding)
            return;

        // 隠れている間のカメラ揺れ
        if (isClosetCameraActive && closetCameraTransform)
        {
            Vector3 bobOffset = bob.DoHeadBob(0.15f); // 揺れの計算
            closetCameraTransform.localPosition = bobOffset; // 隠れ状態カメラを揺らす
        }



        // カメラ操作可能かどうか
        if (!CanControl) return;

        // 右クリックでメインカメラに切り替える
        if (Input.GetMouseButtonDown(1))
        {
            // 隠れ状態カメラがアクティブならメインカメラに戻る
            if (!hasHiddenUnderDesk)
            {
                TutorialManager.Instance.DisappearTutorialText();
            }
            SwitchToMainCamera();
        }

       
       
        
        

    }


    /// <summary>
    /// 指定のものを見たときに、クロスヘアをアニメ－ション
    /// </summary>
    public void ClosshairAnimation(float normalSize, float targetSize, float animationSpeed,
        RectTransform chRectTransform)
    {

        //crosshairRectTransform.sizeDelta = new Vector2(normalSize, normalSize);
        // サイズをアニメーションで変更
        currentSize = Mathf.Lerp(currentSize, targetSize, animationSpeed * Time.deltaTime);
        chRectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }


    /// <summary>
    /// 隠れ状態カメラに切り替えるメソッド
    /// </summary>
    void SwitchToHidingCamera(Camera targetCamera)
    {

        // プレイヤーが隠れている状態に設定
        gameStateManager.SetGameState(GameState.Hiding);
        playerInteractor.ClearInteractUI(); // インタラクトUIを「一時的に」クリア

        mainCamera.gameObject.SetActive(false);
        targetCamera.gameObject.SetActive(true); // 指定されたカメラをアクティブに
        isClosetCameraActive = true;

        currentClosetCamera = targetCamera; // 現在の隠れ状態カメラを保持
        closetCameraTransform = targetCamera.transform; // 隠れ状態カメラのTransformを取得
        closetCameraTransform.localRotation = Quaternion.Euler(baseLocalRotation); // 基本向きに設定

        playerLook.ResetHidingCameraRotation(); // プレイヤーの視点回転をリセット


        // クロスヘアと隠れるテキストを非表示にする
        //crosshair.gameObject.SetActive(false);

        // プレイヤーのオブジェクトを無効化
        player.SetActive(false);

        // サウンド
        audioSource.PlayOneShot(hideSound);
    }

    /// <summary>
    /// メインカメラに戻すメソッド
    /// </summary>
    void SwitchToMainCamera()
    {
        // プレイヤーが隠れていない状態に設定
        gameStateManager.SetGameState(GameState.Playing);

        mainCamera.gameObject.SetActive(true);

        if (currentClosetCamera != null)
        {
            currentClosetCamera.gameObject.SetActive(false); // 現在の隠れ状態カメラを無効化
            currentClosetCamera = null; // 保持するカメラをリセット
        }

        isClosetCameraActive = false;

        // クロスヘアを再表示
        //crosshair.gameObject.SetActive(true);

        // プレイヤーオブジェクトを有効化
        player.SetActive(true);

        // サウンド
        audioSource.PlayOneShot(hideSound);
    }


    /// <summary>
    /// 隠れ状態カメラを取得するメソッド
    /// </summary>
    Camera FindClosetCamera(GameObject closetObject)
    {
        Transform[] children = closetObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.CompareTag("ClosetCamera")) // 隠れ状態カメラを探す
            {
                return child.GetComponent<Camera>();
            }
        }
        return null; // カメラが見つからない場合
    }

    #endregion

}

// Bool型を包む（ラップする）クラスを作成
public class BoolWrapper
{
    public bool Value; // 状態を管理するプロパティ
}

