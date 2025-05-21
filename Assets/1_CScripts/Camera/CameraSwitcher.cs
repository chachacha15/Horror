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

    public Camera mainCamera;

    public MonoBehaviour playerLookScript; // PlayerLookスクリプトを参照

    public LayerMask layerMask; // レイキャストの対象レイヤー
    public float hideDistance = 5f;    // 隠れられる距離

    private bool isClosetCameraActive = false; // 現在のカメラ状態を追跡
    public Camera currentClosetCamera; // 現在のクローゼットカメラを追跡

    public Image crosshair;   // クロスヘアのImageコンポーネント
    public Sprite normalCrosshair; // 通常時のスプライト
    public Sprite closetCrosshair; // クローゼット時のスプライト
    public List<BoolWrapper> activeCrosshairBoolList; // クロスヘアに関わるboolを格納
    public float crosshairDurarion = 8f; // アニメーションスピード
    public float crosshairNormalSize = 10f; // クロスヘアのノーマルサイズ
    public float crosshairActiveSize = 30f; // アクティブサイズ

    public GameObject hideText;       // 隠れるTextオブジェクト
    public GameObject player;
    public bool isPlayerHiding = false;

    public RectTransform crosshairRectTransform; // クロスヘアのRectTransform

    private float currentSize; // 現在のサイズ
    public bool hasHiddenUnderDesk = false; //一回はクローゼットに隠れたことがあるか
    private Vector3 targetCameraBaseLocalPosition;

    // サウンド
    [SerializeField] private AudioClip hideSound;
    private AudioSource audioSource;

    // カメラ揺れ用
    [SerializeField] private CurveControlledBob bob = new CurveControlledBob();
    private Transform closetCameraTransform;

    // 他クラス
    ShakeCamera shakeCamera;


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

        // 左クリックでクローゼットカメラに切り替える
        if (!isClosetCameraActive)
        {

            // クローゼットカメラに切り替え
            Camera targetClosetCamera = FindClosetCamera(targetObject);

            if (targetClosetCamera != null)
            {

                SwitchToClosetCamera(targetClosetCamera);
                targetClosetCamera.transform.localPosition =
                    new Vector3(targetClosetCamera.transform.localPosition.x,
                                    0,
                                    targetClosetCamera.transform.localPosition.z);

                //カメラ揺れのセットアップ
                bob.Setup(targetClosetCamera, 1.0f);


            }
            else
            {
                Debug.LogWarning("対象のクローゼットにカメラが見つかりません！");
            }

            hasHiddenUnderDesk = true;

        }
    }

    #endregion



    #region Methods

    private void Awake()
    {
        activeCrosshairBoolList = new List<BoolWrapper>();

        shakeCamera = ShakeCamera.Instance;

    }

    private void Start()
    {


        mainCamera = Camera.main; // メインカメラを動的に取得
        player = GameObject.FindWithTag("Player");


        // サウンド初期設定
        audioSource = GetComponent<AudioSource>();


        // 他クラスを取得
        playerLookScript = mainCamera.GetComponent<PlayerLook>();

        targetCameraBaseLocalPosition = this.transform.localPosition;




    }

    void Update()
    {


        // 右クリックでメインカメラに切り替える
        if (Input.GetMouseButtonDown(1) && isClosetCameraActive)
        {
            // クローゼットカメラがアクティブならメインカメラに戻る
            SwitchToMainCamera();
        }

       
       
        // 隠れている間のカメラ揺れ
        if (isClosetCameraActive && closetCameraTransform != null && !shakeCamera.isShaking)
        {
            Vector3 bobOffset = bob.DoHeadBob(0.15f); // 揺れの計算
            closetCameraTransform.localPosition = bobOffset; // クローゼットカメラを揺らす
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
    /// クローゼットカメラに切り替えるメソッド
    /// </summary>
    void SwitchToClosetCamera(Camera targetCamera)
    {
        isPlayerHiding = true;
        mainCamera.gameObject.SetActive(false);
        targetCamera.gameObject.SetActive(true); // 指定されたカメラをアクティブに
        isClosetCameraActive = true;

        currentClosetCamera = targetCamera; // 現在のクローゼットカメラを保持
        closetCameraTransform = targetCamera.transform; // クローゼットカメラのTransformを取得

        // クロスヘアと隠れるテキストを非表示にする
        crosshair.gameObject.SetActive(false);

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
        isPlayerHiding = false;
        mainCamera.gameObject.SetActive(true);

        if (currentClosetCamera != null)
        {
            currentClosetCamera.gameObject.SetActive(false); // 現在のクローゼットカメラを無効化
            currentClosetCamera = null; // 保持するカメラをリセット
        }

        isClosetCameraActive = false;

        // クロスヘアを再表示
        crosshair.gameObject.SetActive(true);

        // プレイヤーオブジェクトを有効化
        player.SetActive(true);

        // サウンド
        audioSource.PlayOneShot(hideSound);
    }


    /// <summary>
    /// クローゼットカメラを取得するメソッド
    /// </summary>
    Camera FindClosetCamera(GameObject closetObject)
    {
        Transform[] children = closetObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.CompareTag("ClosetCamera")) // クローゼットカメラを探す
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

