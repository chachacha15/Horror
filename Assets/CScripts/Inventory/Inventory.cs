using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    #region Variables
    public List<PocketItem> items = new List<PocketItem>(); // インベントリ内のアイテムリスト
    public Transform[] itemListParent; // アイテムを表示する親オブジェクト
    public GameObject itemLists; //アイテムを格納するスロット
    public GameObject itemSlotPrefab; // アイテムスロットのプレハブ
    public int maxItems = 5; // 最大所持アイテム数

    #endregion
    private bool isInventoryOpen = false; // インベントリの開閉状態

    private void Start()
    {
        UpdateInventoryUI(); // 初期状態でUIを更新
    }
    private void Update()
    {
        // Iキーでインベントリの表示/非表示を切り替える
        //if (Input.GetKeyDown(KeyCode.I))
        {

        }
    }


    

    // アイテムを追加
    public void AddItem(PocketItem item)
    {
        if (items.Count >= maxItems)
        {
            Debug.Log("インベントリが満杯です！");
            return;
        }

        items.Add(item);
        Debug.Log($"アイテムを追加: {item.item.name}");

        UpdateInventoryUI(); // アイテム追加時にUIを更新
    }

    // インベントリのアイテムを確認
    public void ShowInventory()
    {
        foreach (var item in items)
        {
            Debug.Log($"アイテム: {item.item.name} - {item.explainText}");
        }
    }

    // インベントリのUIを更新
    public void UpdateInventoryUI()
    {

        // インベントリ内のアイテムを順番に表示
        for (int i = 0; i < items.Count; i++)
        {
            // アイテムスロットを生成
            GameObject slot = Instantiate(itemSlotPrefab, itemListParent[i].transform);
            slot.name = items[i].item.name;

            // アイコンを設定
            Image iconImage = slot.transform.Find("ItemIcon").GetComponent<Image>();
            if (iconImage != null && items[i].icon != null)
            {
                iconImage.sprite = items[i].icon;
            }
            else
            {
                Debug.LogWarning($"アイコンが見つからないか、スプライトが設定されていません: {items[i].item.name}");
            }

            // 必要なら追加の情報を設定する
            Text itemNameText = slot.transform.Find("ItemName").GetComponent<Text>();
            if (itemNameText != null)
            {
                itemNameText.text = items[i].item.name;
            }
        }
    }
}
