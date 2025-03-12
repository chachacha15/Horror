using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class WindowManager : MonoBehaviour
{
    #region

    public Camera mainCamera;
    public Transform player; // プレイヤーのTransform
    private float interactionDistance = 6f; // インタラクション距離

    public bool isOpen = false; // 窓が開いているか
    private bool isLookingAtWindow = false; // インタラクトできるかどうか
    private bool isMoving = false; // 開閉中かどうか
    public GameObject interactCanvas;
    public TextMeshProUGUI windowText; // 開ける閉めるなどのテキスト

    public bool isWindow1 = true; // どっちの窓か

    // アニメーション目標移動座標
    private float openTargetX;
    private float closeTargetX;
    private float moveDuration = 0.9f; // 移動にかかる時間


    // 他クラス
    private CameraSwitcher cameraSwitcher;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // 他クラスを取得
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();

        //MainCameraをタグで動的に取得
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


        // 自分の親オブジェクトを取得
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // 親の子供の中からCanvasを探す
            Transform canvasTransform = parentTransform.Find("WindowCanvas");
            interactCanvas = canvasTransform.gameObject;

            if (canvasTransform != null)
            {
                // Canvasの子供の中から開閉Textを探す
                Transform textTransform = canvasTransform.Find("開閉Text");
                if (textTransform != null)
                {
                    windowText = textTransform.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        interactCanvas.SetActive(false);


        // 窓の種類によって移動先を変える
        openTargetX = isWindow1 ? 2.5f : -2.8f;
        closeTargetX = isWindow1 ? -2.8f : 2.5f;

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
                if (hit.collider.gameObject == gameObject) // 現在のドアに一致する場合
                {
                    isLookingAtWindow = true;
                    interactCanvas.SetActive(true);
                    cameraSwitcher.ClosshairAnimation(10f, 100f, 0.5f, cameraSwitcher.crosshairRectTransform, isLookingAtWindow);
                }
                else
                {
                    isLookingAtWindow = false;
                    interactCanvas.SetActive(false);
                }
            }
        }
        else
        {
            isLookingAtWindow = false;
            interactCanvas.SetActive(false);
            cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingAtWindow);
        }


        if (isLookingAtWindow && Input.GetMouseButtonDown(0))
        {
            ToggleWindow();
        }
    }

    // 窓にインタラクトしたときに呼ばれる。開閉を操作する
    public void ToggleWindow()
    {
        if (isMoving) return;

        isOpen = !isOpen;
        windowText.text = isOpen ? "閉める" : "開ける";

        // 窓のアニメーション
        StartCoroutine(MoveWindow(isOpen ? openTargetX : closeTargetX));
    }

    /// <summary>
    /// 窓を目標のX座標に向けて滑らかに移動させる
    /// </summary>
    private IEnumerator MoveWindow(float targetX)
    {
        isMoving = true;
        float elapsedTime = 0f;
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = new Vector3(targetX, startPos.y, startPos.z);

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // 次のフレームまで待機
        }

        transform.localPosition = targetPos; // 最終位置をセット

        isMoving = false;
    }
}
