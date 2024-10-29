using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera closetCamera;
    public MonoBehaviour playerLookScript; // PlayerLookスクリプトを参照
    public LayerMask layerMask; // レイキャストの対象レイヤー
    private bool isClosetCameraActive = false; // 現在のカメラ状態を追跡

    private void Start()
    {
        mainCamera = Camera.main; // メインカメラを動的に取得
        closetCamera = FindClosetCamera(); // クローゼットカメラを動的に取得
        playerLookScript = mainCamera.GetComponent<PlayerLook>(); // PlayerLookスクリプトを動的に取得
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左クリックを検出
        {
            if (isClosetCameraActive)
            {
                SwitchToMainCamera();
            }
            else
            {
                Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider.CompareTag("Closet"))
                    {
                        SwitchToClosetCamera();
                    }
                }
            }
        }
    }

    void SwitchToClosetCamera()
    {
        mainCamera.gameObject.SetActive(false);
        closetCamera.gameObject.SetActive(true);
        playerLookScript.enabled = false; // PlayerLookスクリプトを無効化
        isClosetCameraActive = true;
    }

    void SwitchToMainCamera()
    {
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

