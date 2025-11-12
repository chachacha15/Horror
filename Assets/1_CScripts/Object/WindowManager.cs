using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class WindowManager : MonoBehaviour, IInteractable
{
    #region

    public Camera mainCamera;
    public Transform player; // プレイヤーのTransform

    public bool isOpen = false; // 窓が開いているか
    private bool isMoving = false; // 開閉中かどうか

    public bool isWindow1 = true; // どっちの窓か

    // サウンド
    [SerializeField] private AudioClip windowSound;
    private AudioSource windowAS;

    // アニメーション目標移動座標
    private float openTargetX;
    private float closeTargetX;
    private float moveDuration = 0.9f; // 移動にかかる時間


    // 他クラス

    #endregion




    #region Interactable (IInteractable)

    public string GetInteractText()
    {
        if (!isOpen) return "開ける";
        return "閉める";
    }

    public bool ShowInteractText => true; // テキスト表示するかどうか
    public bool ActivateCrosshair => true;

    /// <summary>
    /// クリック時、開閉
    /// </summary>
    public void Interact(GameObject targetObject)
    {
        ToggleWindow();
    }

    #endregion




    // Start is called before the first frame update
    void Start()
    {
        // 他クラスを取得

        //MainCameraをタグで動的に取得
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // AS
        windowAS = GetComponent<AudioSource>();


        // 窓の種類によって移動先を変える
        openTargetX = isWindow1 ? 2.5f : -2.8f;
        closeTargetX = isWindow1 ? -2.8f : 2.5f;

    }



    /// <summary>
    /// 窓にインタラクトしたときに呼ばれる。開閉を操作する
    /// </summary>
    public void ToggleWindow()
    {
        if (isMoving) return;

        isOpen = !isOpen;

        // 窓のアニメーション
        StartCoroutine(MoveWindow(isOpen ? openTargetX : closeTargetX));

        // サウンド
        windowAS.PlayOneShot(windowSound);
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
