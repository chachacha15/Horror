using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<PocketItem> items = new List<PocketItem>(); // �C���x���g�����̃A�C�e�����X�g

    // �A�C�e����ǉ�
    public void AddItem(PocketItem item)
    {
        items.Add(item);
        Debug.Log($"�A�C�e����ǉ�: {item.item.name}");
    }

    // �C���x���g���̃A�C�e�����m�F
    public void ShowInventory()
    {
        foreach (var item in items)
        {
            Debug.Log($"�A�C�e��: {item.item.name} - {item.explainText}");
        }
    }
}
