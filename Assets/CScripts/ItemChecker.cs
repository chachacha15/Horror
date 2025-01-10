using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemChecker : MonoBehaviour
{
    public float interactDistance = 3f; // インタラクト可能な距離
    public LayerMask itemLayer; // アイテムに使用するレイヤー
    public GameObject interactText; // UI テキスト (拾うメッセージを表示)
    public ItemDataBase itemDataBase; // アイテムデータベースを参照
    public Inventory inventory; // プレイヤーのインベントリを管理するスクリプト
    public InventoryDisplay inventoryDisplay;

    public Transform itemDisplayArea; // 3Dモデル表示エリア
    public Camera itemDisplayCamera; // アイテム表示用カメラ
    private GameObject currentDisplayedItem; // 現在表示中のアイテム

    private TextMeshProUGUI interactTextComponent; // TextMeshProの参照

    private void Start()
    {
        interactTextComponent = interactText.GetComponent<TextMeshProUGUI>();

        if (interactTextComponent == null)
        {
            Debug.LogError("interactTextにTextMeshProUGUIコンポーネントが見つかりません！");
        }

        if (itemDisplayCamera == null || itemDisplayArea == null)
        {
            Debug.LogError("ItemDisplayCameraまたはItemDisplayAreaが設定されていません！");
        }
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

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickupItem(hitItem);
                    inventoryDisplay.ToggleInventory();

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

        PocketItem itemData = itemDataBase.itemList.Find(i => i.item.name == item.name);
        if (itemData != null)
        {
            inventory.AddItem(itemData);
            //Display3DItem(itemData.item); // アイテムを3D表示
        }
        else
        {
            Debug.LogWarning("データベースにこのアイテムが存在しません！");
        }

        Destroy(item);
    }

    //private void Display3DItem(GameObject itemPrefab)
    //{
    //    // 既存の表示アイテムを削除
    //    if (currentDisplayedItem != null)
    //    {
    //        Destroy(currentDisplayedItem);
    //    }

    //    // アイテムを表示エリアにインスタンス化
    //    currentDisplayedItem = Instantiate(itemPrefab, itemDisplayArea);
    //    currentDisplayedItem.transform.localPosition = Vector3.zero;
    //    currentDisplayedItem.transform.localRotation = Quaternion.identity;
    //    currentDisplayedItem.transform.localScale = Vector3.one;
    //}
}
