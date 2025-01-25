using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    #region Variables
    public List<PocketItem> items = new List<PocketItem>(); // インベントリ内のアイテムリスト
    public Transform itemListParent; // アイテムを表示する親オブジェクト
    public GameObject itemLists; //アイテムを格納するスロット
    public GameObject itemSlotPrefab; // アイテムスロットのプレハブ
    #endregion
    private bool isInventoryOpen = false; // インベントリの開閉状態

    private void Start()
    {
       UpdateInventoryUI(); // 初期状態でUIを更新
    }
    private void Update()
    {
        // Iキーでインベントリの表示/非表示を切り替える
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    // インベントリの表示/非表示を切り替える
    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        itemLists.SetActive(isInventoryOpen); // インベントリ画面のオンオフを切り替え

        if (isInventoryOpen)
        {
            UpdateInventoryUI(); // 開いたときに内容を更新
            Cursor.lockState = CursorLockMode.None; // マウスを表示
            Cursor.visible = true; // カーソルを表示
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // マウスを非表示
            Cursor.visible = false; // カーソルを非表示
        }
    }

    // アイテムを追加
    public void AddItem(PocketItem item)
    {
        items.Add(item);
        Debug.Log($"アイテムを追加: {item.item.name}");
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
        // 現在のアイテム表示をクリア
        foreach (Transform child in itemLists.transform)
        {
            Destroy(child.gameObject);
        }

        // インベントリ内のアイテムを順番に表示
        foreach (var item in items)
        {
            // アイテムスロットを生成
            GameObject slot = Instantiate(itemSlotPrefab, itemLists.transform);
            slot.name = item.item.name;

            // アイコンを設定
            Image iconImage = slot.transform.Find("ItemIcon").GetComponent<Image>();
            if (iconImage != null && item.icon != null)
            {
                iconImage.sprite = item.icon;
            }
            else
            {
                Debug.LogWarning($"アイコンが見つからないか、スプライトが設定されていません: {item.item.name}");
            }

            // 必要なら追加の情報を設定する
            Text itemNameText = slot.transform.Find("ItemName").GetComponent<Text>();
            if (itemNameText != null)
            {
                itemNameText.text = item.item.name;
            }
        }
        // Layout Group を再計算
        //LayoutRebuilder.ForceRebuildLayoutImmediate(itemLists.GetComponent<RectTransform>());
    }
}
