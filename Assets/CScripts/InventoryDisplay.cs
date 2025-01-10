using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    public GameObject inventoryUI; // �C���x���g����ʂ̐e�I�u�W�F�N�g
    public Transform itemListParent; // �A�C�e�����X�g��\������e�I�u�W�F�N�g
    public GameObject itemSlotPrefab; // �A�C�e���X���b�g�̃v���n�u
    public Inventory inventory; // �C���x���g���f�[�^���Q��
    public Camera itemDisplayCamera; // �A�C�e���\���p�J����
    public Transform itemDisplayPosition; // �A�C�e����z�u����ʒu

    private GameObject currentDisplayedItem; // �\�����̃A�C�e����ǐ�
    private bool isInventoryOpen = false; // �C���x���g���̊J���

    private bool isPausedByDisplayItem = false; // �ꎞ��~��Ԃ�ǐ�
    private GameObject NowSlot; //���\�����Ă���X���b�g


    private void Start()
    {
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false); // ������ԂŔ�\��
        }
        else
        {
            Debug.LogError("Inventory UI ���ݒ肳��Ă��܂���I");
        }

        if (itemDisplayCamera == null)
        {
            Debug.LogError("ItemDisplayCamera ���ݒ肳��Ă��܂���I");
        }

        if (itemDisplayPosition == null)
        {
            Debug.LogError("ItemDisplayPosition ���ݒ肳��Ă��܂���I");
        }
    }

    void Update()
    {
        if (isInventoryOpen)
        {
            // ���ԂɊ�Â��ď㉺�Ɉړ�
            float floatOffset = Mathf.Sin(Time.unscaledTime * 2f) * 0.05f; // �U���Ƒ��x�𒲐�
            currentDisplayedItem.transform.position = itemDisplayPosition.position + new Vector3(0f, floatOffset, 0f);
        }

        //�@�C���x���g�����鎞
        if (isPausedByDisplayItem && Input.GetMouseButtonDown(0)) // 0�͍��N���b�N
        {
            ResumeGame();
            isInventoryOpen = !isInventoryOpen;
            inventoryUI.SetActive(isInventoryOpen);
            NowSlot.SetActive(false);  //���ݕ\�����Ă���X���b�g���A�N�e�B�u

            Cursor.lockState = CursorLockMode.Locked; // �}�E�X���\��
            Cursor.visible = false;                  // �}�E�X�J�[�\�����\��
            ClearDisplayedItem();                    // �\�����̃A�C�e�����N���A
        }
    }


    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (inventoryUI != null)
        {
            inventoryUI.SetActive(isInventoryOpen);

            if (isInventoryOpen)
            {
                UpdateInventoryUI(); // �J�����Ƃ��ɓ��e���X�V
                Cursor.lockState = CursorLockMode.None; // �}�E�X��\��
                Cursor.visible = true;                 // �}�E�X�J�[�\����\��
            }
            
        }
    }

    private void UpdateInventoryUI()
    {
        //// ���݂̃A�C�e�����X�g���N���A
        //foreach (Transform child in itemListParent)
        //{
        //    //Debug.Log(itemListParent);
        //    //Debug.Log(child);
        //    //Debug.Log(child.gameObject);
        //    Destroy(child.gameObject);
        //}

        // �ŐV�̃A�C�e�����擾
        var item = inventory.items[inventory.items.Count - 1];

        // �X���b�g�𐶐�
        GameObject slot = Instantiate(itemSlotPrefab, itemListParent);
        NowSlot = slot;
            

        TextMeshProUGUI itemNameText = slot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        //Debug.Log(itemNameText);

        if (itemNameText != null)
        {
            itemNameText.text = item.item.name;
        }
        else
        {
            Debug.LogError("ItemName �� TextMeshProUGUI �R���|�[�l���g��������܂���I");
        }


            

        TextMeshProUGUI itemDescriptionText = slot.transform.Find("ItemDiscription").GetComponent<TextMeshProUGUI>();

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = item.explainText;
        }
        else
        {
            Debug.LogError("ItemDiscription �� Text �R���|�[�l���g��������܂���I");
        }
            

        Image itemIcon = slot.transform.Find("ItemIcon").GetComponent<Image>();
        if (itemIcon != null && item.item.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            itemIcon.sprite = spriteRenderer.sprite;
        }
        else if (itemIcon == null)
        {
            Debug.LogError("ItemIcon �� Image �R���|�[�l���g��������܂���I");
        }

        PocketItem currentItem = item; // �N���[�W������h������
        DisplayItemIn3D(currentItem.item);
            
            
    }

    private void DisplayItemIn3D(GameObject item)
    {
        // ���ɕ\�����̃A�C�e��������΍폜
        ClearDisplayedItem();

        // �A�C�e����\���ʒu�ɔz�u (X����-90�x��])
        currentDisplayedItem = Instantiate(item, itemDisplayPosition.position, Quaternion.Euler(-80f, 34f, 0f));
        currentDisplayedItem.transform.SetParent(itemDisplayPosition); // �e��ݒ肵�Ĉړ����ȒP��

        // �s�v�ȃR���|�[�l���g�𖳌����i������C���^���N�V�����Ȃǁj
        Collider collider = currentDisplayedItem.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        Rigidbody rigidbody = currentDisplayedItem.GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.isKinematic = true;

        // �Q�[�����ꎞ��~
        Time.timeScale = 0f;
        isPausedByDisplayItem = true; // �ꎞ��~�t���O��L����

    }

    private void ClearDisplayedItem()
    {
        if (currentDisplayedItem != null)
        {
            Destroy(currentDisplayedItem);
            currentDisplayedItem = null;

        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // �Q�[�����ĊJ
        isPausedByDisplayItem = false; // �t���O�����Z�b�g
    }
}
