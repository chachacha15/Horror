using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    #region Variables
    public List<PocketItem> items = new List<PocketItem>(); // �C���x���g�����̃A�C�e�����X�g
    public Transform itemListParent; // �A�C�e����\������e�I�u�W�F�N�g
    public GameObject itemLists; //�A�C�e�����i�[����X���b�g
    public GameObject itemSlotPrefab; // �A�C�e���X���b�g�̃v���n�u
    #endregion
    private bool isInventoryOpen = false; // �C���x���g���̊J���

    private void Start()
    {
       UpdateInventoryUI(); // ������Ԃ�UI���X�V
    }
    private void Update()
    {
        // I�L�[�ŃC���x���g���̕\��/��\����؂�ւ���
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    // �C���x���g���̕\��/��\����؂�ւ���
    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        itemLists.SetActive(isInventoryOpen); // �C���x���g����ʂ̃I���I�t��؂�ւ�

        if (isInventoryOpen)
        {
            UpdateInventoryUI(); // �J�����Ƃ��ɓ��e���X�V
            Cursor.lockState = CursorLockMode.None; // �}�E�X��\��
            Cursor.visible = true; // �J�[�\����\��
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // �}�E�X���\��
            Cursor.visible = false; // �J�[�\�����\��
        }
    }

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

    // �C���x���g����UI���X�V
    public void UpdateInventoryUI()
    {
        // ���݂̃A�C�e���\�����N���A
        foreach (Transform child in itemLists.transform)
        {
            Destroy(child.gameObject);
        }

        // �C���x���g�����̃A�C�e�������Ԃɕ\��
        foreach (var item in items)
        {
            // �A�C�e���X���b�g�𐶐�
            GameObject slot = Instantiate(itemSlotPrefab, itemLists.transform);
            slot.name = item.item.name;

            // �A�C�R����ݒ�
            Image iconImage = slot.transform.Find("ItemIcon").GetComponent<Image>();
            if (iconImage != null && item.icon != null)
            {
                iconImage.sprite = item.icon;
            }
            else
            {
                Debug.LogWarning($"�A�C�R����������Ȃ����A�X�v���C�g���ݒ肳��Ă��܂���: {item.item.name}");
            }

            // �K�v�Ȃ�ǉ��̏���ݒ肷��
            Text itemNameText = slot.transform.Find("ItemName").GetComponent<Text>();
            if (itemNameText != null)
            {
                itemNameText.text = item.item.name;
            }
        }
        // Layout Group ���Čv�Z
        //LayoutRebuilder.ForceRebuildLayoutImmediate(itemLists.GetComponent<RectTransform>());
    }
}
