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
    public float interactionDistance = 6f; // ドアとのインタラクション距離

    private bool isOpen = false;
    private bool isLookingAtDoor = false; // クローゼットを見ている状態か

    public GameObject doortext;
    TextMeshProUGUI doorGUI;

    public Image crosshair;   // クロスヘアのImageコンポーネント
    private float currentSize; // 現在のサイズ
    CameraSwitcher cameraSwitcher;
    void Start()
    {
        


        // 自分の親オブジェクトを取得
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // 親の子供の中からCanvasを探す
            Transform canvasTransform = parentTransform.Find("Canvas");

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

    }
    void Update()
    {

        // クローゼットにカーソルがあるかを判定
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // プレイヤーが近づいたらClickで開閉
        if (Physics.Raycast(ray, out hit, interactionDistance, layerMask))
        {
            if (hit.collider.gameObject == gameObject) // 現在のドアに一致する場合
            {
                isLookingAtDoor = true;
                if (doortext != null) doortext.SetActive(true); // 開閉Textを非表示

            }
            else
            {
                isLookingAtDoor = false;
                if (doortext != null) doortext.SetActive(false); // 開閉Textを非表示

            }
        }
        else
        {
            isLookingAtDoor = false;
            if (doortext != null) doortext.SetActive(false); // 開閉Textを非表示

            cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingAtDoor);
        }

        cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingAtDoor);

        // 左クリック時にドアを開閉
        if (Input.GetMouseButtonDown(0) && isLookingAtDoor)
        {
            ToggleDoor();
        }



    }

    public void ToggleDoor()
    {

        isOpen = !isOpen;

        animator.SetBool("isOpen", isOpen);


        if (doorGUI != null) doorGUI.text = isOpen ? "閉める" : "開ける";

    }
}
