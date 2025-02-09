using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class ItemChecker : MonoBehaviour
{
    #region Variables
    public float interactDistance = 3f; // インタラクト可能な距離

    // アイテム用
    public LayerMask itemLayer; // アイテムに使用するレイヤー
    public GameObject interactText; // UI テキスト (拾うメッセージを表示)
    public ItemDataBase itemDataBase; // アイテムデータベースを参照
    public Inventory inventory; // プレイヤーのインベントリを管理するスクリプト
    public ItemDisplay itemDisplay;

    // 表示するUI用
    private TextMeshProUGUI interactTextComponent; // TextMeshProの参照
    private bool isLookingItem = false;

    //その他・他クラス
    private TutorialManager tutorialManager;
    private CameraSwitcher cameraSwitcher;

    // 手持ちライト用
    [SerializeField] GameObject flashLightSystem;
    [SerializeField] GameObject flashlightTutorial;

    // 血・スポンジギミック用
    public Material bloodMaterial; // 血のマテリアル
    public float fadeDuration = 1.5f; // フェードアウトの時間
    public bool hasSponge { get; private set; }  // 読み取り専用のプロパティにする


    #endregion



    private void Start()
    {
        interactTextComponent = interactText.GetComponent<TextMeshProUGUI>();

        if (interactTextComponent == null)
        {
            Debug.LogError("interactTextにTextMeshProUGUIコンポーネントが見つかりません！");
        }

        tutorialManager = flashlightTutorial.GetComponent<TutorialManager>();
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();

       
    }

    private void Update()
    {
        // レイキャストでアイテムを検出
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, itemLayer))
        {
            GameObject hitItem = hit.collider.gameObject;

            if (hitItem.CompareTag("Item"))
            {
                interactTextComponent.text = $"取る";
                interactText.SetActive(true);
                isLookingItem = true;
                cameraSwitcher.ClosshairAnimation(10f, 500f, 0.5f, cameraSwitcher.crosshairRectTransform, isLookingItem);

                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                {
                    PickupItem(hitItem);
                    itemDisplay.ToggleItemDisplay();
                    inventory.UpdateInventoryUI();
                }
            }
        }
        else
        {
            interactText.SetActive(false);
            isLookingItem = false;
            cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingItem);
        }

    }

    private void PickupItem(GameObject item)
    {
        PocketItem itemData = itemDataBase.itemList.Find(i => i.item.name == item.name);

        if (itemData != null)
        {
            inventory.AddItem(itemData);

            // フラッシュライト取得時
            if (item.name == "Flashlight")
            {
                if (flashLightSystem != null)
                {
                    flashLightSystem.SetActive(true);

                    if (tutorialManager != null)
                    {
                        StartCoroutine(tutorialManager.ShowTutorial());
                    }

                    flashlightTutorial.SetActive(true);
                    flashlightTutorial.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
            // スポンジ取得時
            if (item.name == "sponge")
            {
                hasSponge = true; // スポンジ取得フラグを立てる
            }
        }
        else
        {
            Debug.LogWarning("データベースにこのアイテムが存在しません！");
        }

        Destroy(item);
    }

    // スポンジを持っているか確認するメソッド
    public bool HasSponge()
    {
        return hasSponge;
    }
}

