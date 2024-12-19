using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera closetCamera;
   
    public MonoBehaviour playerLookScript; // PlayerLookスクリプトを参照
    public LayerMask layerMask; // レイキャストの対象レイヤー
    public float hideDistance = 5f;    // 隠れられる距離

    private bool isClosetCameraActive = false; // 現在のカメラ状態を追跡


    public Image crosshair;   // クロスヘアのImageコンポーネント
    public Sprite normalCrosshair; // 通常時のスプライト
    public Sprite closetCrosshair; // クローゼット時のスプライト

    public GameObject hideText;       // 隠れるTextオブジェクト


    public GameObject player;
    public bool isPlayerHiding = false;

    public RectTransform crosshairRectTransform; // クロスヘアのRectTransform
   

    private float currentSize; // 現在のサイズ
    private bool isLookingAtCloset = false; // クローゼットを見ている状態か

    private void Start()
    {   

        // 隠れるTextを動的に取得（オブジェクト名が "隠れるText" の場合）
        hideText = GameObject.Find("隠れるText");
        // 初期状態で非表示に
        hideText.SetActive(false);

        mainCamera = Camera.main; // メインカメラを動的に取得
        closetCamera = FindClosetCamera(); // クローゼットカメラを動的に取得
        player = GameObject.FindWithTag("Player");
        
        playerLookScript = mainCamera.GetComponent<PlayerLook>(); // PlayerLookスクリプトを動的に取得
    }
    void Update()
    {
        // クローゼットにカーソルがあるかを判定
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hideDistance, layerMask))
        {
            if (hit.collider.CompareTag("Closet") && !isClosetCameraActive)
            {
                // クローゼットを見ている場合
                isLookingAtCloset = true;
                hideText.SetActive(true); // 隠れるTextをONに

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

        ClosshairAnimation(10f, 35f, 5f,crosshairRectTransform, isLookingAtCloset);
        
        

        if (Input.GetMouseButtonDown(0)) // 左クリックを検出
        {
            if (isClosetCameraActive)
            {
                SwitchToMainCamera();
            }
            else
            {
                

                if (Physics.Raycast(ray, out hit, hideDistance, layerMask))
                {
                    if (hit.collider.CompareTag("Closet"))
                    {
                        SwitchToClosetCamera();
                    }
                }
            }
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

    void SwitchToClosetCamera()
    {
        isPlayerHiding = true;
        mainCamera.gameObject.SetActive(false);
        closetCamera.gameObject.SetActive(true);
        playerLookScript.enabled = false; // PlayerLookスクリプトを無効化
        isClosetCameraActive = true;
        
    }

    void SwitchToMainCamera()
    {
        isPlayerHiding = false;
        mainCamera.gameObject.SetActive(true);
        closetCamera.gameObject.SetActive(false);
        playerLookScript.enabled = true; // PlayerLookスクリプトを有効化
        isClosetCameraActive = false;
        
    }

    Camera FindClosetCamera()
    {
        GameObject closet = GameObject.FindGameObjectWithTag("Closet");
        if (closet != null)
        {
            Transform[] children = closet.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.CompareTag("ClosetCamera"))
                {
                    return child.GetComponent<Camera>();
                }
            }
        }
        return null;
    }
}

