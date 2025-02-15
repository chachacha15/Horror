using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    #region Variables
    public List<PocketItem> items = new List<PocketItem>(); // �C���x���g�����̃A�C�e�����X�g
    public Transform[] itemListParent; // �A�C�e����\������e�I�u�W�F�N�g
    public GameObject itemLists; //�A�C�e�����i�[����X���b�g
    public GameObject itemSlotPrefab; // �A�C�e���X���b�g�̃v���n�u
    public int maxItems = 5; // �ő及���A�C�e����

    #endregion
    private bool isInventoryOpen = false; // �C���x���g���̊J���

    private void Start()
    {
        UpdateInventoryUI(); // ������Ԃ�UI���X�V
    }
    private void Update()
    {
        // I�L�[�ŃC���x���g���̕\��/��\����؂�ւ���
        //if (Input.GetKeyDown(KeyCode.I))
        {

        }
    }


    

    // �A�C�e����ǉ�
    public void AddItem(PocketItem item)
    {
        if (items.Count >= maxItems)
        {
            Debug.Log("�C���x���g�������t�ł��I");
            return;
        }

        items.Add(item);
        Debug.Log($"�A�C�e����ǉ�: {item.item.name}");

        UpdateInventoryUI(); // �A�C�e���ǉ�����UI���X�V
    }

    // �C���x���g���̃A�C�e�����m�F
    public void ShowInventory()
    {
        foreach (var item in items)
        {
            Debug.Log($"�A�C�e��: {item.item.name} - {item.explainText}");
        }
    }

    // �C���x���g����UI���X�V
    public void UpdateInventoryUI()
    {

        // �C���x���g�����̃A�C�e�������Ԃɕ\��
        for (int i = 0; i < items.Count; i++)
        {
            // �A�C�e���X���b�g�𐶐�
            GameObject slot = Instantiate(itemSlotPrefab, itemListParent[i].transform);
            slot.name = items[i].item.name;

            // �A�C�R����ݒ�
            Image iconImage = slot.transform.Find("ItemIcon").GetComponent<Image>();
            if (iconImage != null && items[i].icon != null)
            {
                iconImage.sprite = items[i].icon;
            }
            else
            {
                Debug.LogWarning($"�A�C�R����������Ȃ����A�X�v���C�g���ݒ肳��Ă��܂���: {items[i].item.name}");
            }

            // �K�v�Ȃ�ǉ��̏���ݒ肷��
            Text itemNameText = slot.transform.Find("ItemName").GetComponent<Text>();
            if (itemNameText != null)
            {
                itemNameText.text = items[i].item.name;
            }
        }
    }
}
