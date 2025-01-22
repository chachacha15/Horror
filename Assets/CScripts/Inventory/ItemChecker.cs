using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemChecker : MonoBehaviour
{
    #region Variables
    public float interactDistance = 3f; // インタラクト可能な距離
    public LayerMask itemLayer; // アイテムに使用するレイヤー
    public GameObject interactText; // UI テキスト (拾うメッセージを表示)
    public ItemDataBase itemDataBase; // アイテムデータベースを参照
    public Inventory inventory; // プレイヤーのインベントリを管理するスクリプト
    public ItemDisplay itemDisplay;


    private TextMeshProUGUI interactTextComponent; // TextMeshProの参照

    [SerializeField] GameObject flashLightSystem;
    [SerializeField] GameObject flashlightTutorial;
    private TutorialManager tutorialManager;


    #endregion


    private void Start()
    {
        interactTextComponent = interactText.GetComponent<TextMeshProUGUI>();

        if (interactTextComponent == null)
        {
            Debug.LogError("interactTextにTextMeshProUGUIコンポーネントが見つかりません！");
        }

        
        tutorialManager = flashlightTutorial.GetComponent<TutorialManager>();
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
        }
    }

    private void PickupItem(GameObject item)
    {
        // アイテムのデータを検索
        PocketItem itemData = itemDataBase.itemList.Find(i => i.item.name == item.name);

        if (itemData != null)
        {
            // インベントリにアイテムを追加
            inventory.AddItem(itemData);

            // Flashlightの場合にFlashLightSystemをアクティブ化
            if (item.name == "Flashlight")
            {
                if (flashLightSystem != null)
                {
                    //フラッシュライトを使えるようにする
                    flashLightSystem.SetActive(true);
                    
                    if (tutorialManager != null)
                    {
                        StartCoroutine(tutorialManager.ShowTutorial()); // コルーチンを直接呼び出す
                    }

                    //フラッシュライトのチュートリアルを表示する
                    flashlightTutorial.SetActive(true);
                    flashlightTutorial.transform.GetChild(0).gameObject.SetActive(true);

                }
               
            }
        }
        else
        {
            Debug.LogWarning("データベースにこのアイテムが存在しません！");
        }

        // アイテムをシーンから削除
        Destroy(item);
    }

    
}
