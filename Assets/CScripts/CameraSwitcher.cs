using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityStandardAssets.Utility; // CurveControlledBob を使用

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    //public Camera closetCamera;

    public MonoBehaviour playerLookScript; // PlayerLookスクリプトを参照

    public LayerMask layerMask; // レイキャストの対象レイヤー
    public float hideDistance = 5f;    // 隠れられる距離

    private bool isClosetCameraActive = false; // 現在のカメラ状態を追跡
    private Camera currentClosetCamera; // 現在のクローゼットカメラを追跡

    public Image crosshair;   // クロスヘアのImageコンポーネント
    public Sprite normalCrosshair; // 通常時のスプライト
    public Sprite closetCrosshair; // クローゼット時のスプライト

    public GameObject hideText;       // 隠れるTextオブジェクト
    public GameObject player;
    public bool isPlayerHiding = false;

    public RectTransform crosshairRectTransform; // クロスヘアのRectTransform

    private float currentSize; // 現在のサイズ
    private bool isLookingAtCloset = false; // クローゼットを見ている状態か
    public bool hasHiddenUnderDesk = false; //一回はクローゼットに隠れたことがあるか
    private Vector3 targetCameraBaseLocalPosition;

    // カメラ揺れ用
    [SerializeField] private CurveControlledBob bob = new CurveControlledBob();
    private Transform closetCameraTransform;

    private void Start()
    {
        // 隠れるTextを動的に取得（オブジェクト名が "隠れるText" の場合）
        hideText = GameObject.Find("隠れるText");
        // 初期状態で非表示に
        hideText.SetActive(false);

        mainCamera = Camera.main; // メインカメラを動的に取得
        player = GameObject.FindWithTag("Player");

        playerLookScript = mainCamera.GetComponent<PlayerLook>(); // PlayerLookスクリプトを動的に取得

        targetCameraBaseLocalPosition = this.transform.localPosition;

    }

    void Update()
    {
        // クローゼットにカーソルがあるかを判定
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hideDistance, layerMask))
        {
            if (hit.collider.CompareTag("Closet"))
            {
                isLookingAtCloset = true;
                hideText.SetActive(true); // 隠れるTextをONに

                // 左クリックでカメラを切り替える
                if (Input.GetMouseButtonDown(0))
                {
                    if (isClosetCameraActive)
                    {
                        // クローゼットカメラがアクティブならメインカメラに戻る
                        SwitchToMainCamera();
                    }
                    else
                    {
                        // クローゼットカメラに切り替え
                        Camera targetClosetCamera = FindClosetCamera(hit.collider.gameObject);
                        if (targetClosetCamera != null)
                        {
                            SwitchToClosetCamera(targetClosetCamera);
                            targetClosetCamera.transform.localPosition = 
                                new Vector3( targetClosetCamera.transform.localPosition.x,
                                             0,
                                             targetClosetCamera.transform.localPosition.z);

                            // カメラ揺れのセットアップ
                            bob.Setup(targetClosetCamera, 1.0f); 

                        }
                        else
                        {
                            Debug.LogWarning("対象のクローゼットにカメラが見つかりません！");
                        }

                        hasHiddenUnderDesk = true;
                    }
                }
            }
            else
            {
                // 他のオブジェクトの場合
                isLookingAtCloset = false;
                hideText.SetActive(false); // 隠れるTextをOFFに
            }
        }
        else
        {
            // 何もヒットしていない場合
            isLookingAtCloset = false;
            hideText.SetActive(false); // 隠れるTextをOFFに
        }

        ClosshairAnimation(10f, 500f, 0.5f, crosshairRectTransform, isLookingAtCloset);

        // 隠れている間のカメラ揺れ
        if (isClosetCameraActive && closetCameraTransform != null)
        {
            Vector3 bobOffset = bob.DoHeadBob(0.15f); // 揺れの計算
            closetCameraTransform.localPosition = bobOffset; // クローゼットカメラを揺らす
        }
    }

    public void ClosshairAnimation(float normalSize, float targetSize, float animationSpeed,
        RectTransform chRectTransform, bool isLooking)
    {
        //crosshairRectTransform.sizeDelta = new Vector2(normalSize, normalSize);
        // サイズをアニメーションで変更
        float target = isLooking ? targetSize : normalSize;
        currentSize = Mathf.Lerp(currentSize, target, animationSpeed * Time.deltaTime);
        chRectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }

    void SwitchToClosetCamera(Camera targetCamera)
    {
        isPlayerHiding = true;
        mainCamera.gameObject.SetActive(false);
        targetCamera.gameObject.SetActive(true); // 指定されたカメラをアクティブに
        playerLookScript.enabled = false; // PlayerLookスクリプトを無効化
        isClosetCameraActive = true;

        currentClosetCamera = targetCamera; // 現在のクローゼットカメラを保持
        closetCameraTransform = targetCamera.transform; // クローゼットカメラのTransformを取得

        // クロスヘアと隠れるテキストを非表示にする
        crosshair.gameObject.SetActive(false);
        hideText.SetActive(false);

        // プレイヤーのオブジェクトを無効化
        player.SetActive(false);
    }

    void SwitchToMainCamera()
    {
        isPlayerHiding = false;
        mainCamera.gameObject.SetActive(true);

        if (currentClosetCamera != null)
        {
            currentClosetCamera.gameObject.SetActive(false); // 現在のクローゼットカメラを無効化
            currentClosetCamera = null; // 保持するカメラをリセット
        }

        playerLookScript.enabled = true; // PlayerLookスクリプトを有効化
        isClosetCameraActive = false;

        // クロスヘアを再表示
        crosshair.gameObject.SetActive(true);

        // プレイヤーオブジェクトを有効化
        player.SetActive(true);
    }

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
}
