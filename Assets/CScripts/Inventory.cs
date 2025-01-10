using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<PocketItem> items = new List<PocketItem>(); // インベントリ内のアイテムリスト

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
}
