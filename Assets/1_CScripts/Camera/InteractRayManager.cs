using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class InteractRayManager : MonoBehaviour
{

    #region

    public Camera mainCamera;
    public Transform player; // プレイヤーのTransform
    private float interactionDistance = 6f; // インタラクション距離

    private WindowManager windowManager;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //MainCameraをタグで動的に取得
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // クローゼットにカーソルがあるかを判定
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // デバッグ用：レイキャストの可視化
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.magenta);

        // プレイヤーが近づいたらClickで開閉
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.CompareTag("Window"))
            {

            }
        }
    }
}
