using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    #region Variables
    public GameObject ItemDisplayUI; // �C���x���g����ʂ̐e�I�u�W�F�N�g
    public Transform itemListParent; // �A�C�e�����X�g��\������e�I�u�W�F�N�g
    public GameObject itemSlotPrefab; // �A�C�e���X���b�g�̃v���n�u
    public Inventory inventory; // �C���x���g���f�[�^���Q��
    public Camera itemDisplayCamera; // �A�C�e���\���p�J����
    public Transform itemDisplayPosition; // �A�C�e����z�u����ʒu

    private GameObject currentDisplayedItem; // �\�����̃A�C�e����ǐ�
    private bool isItemDisplayON  = false; // �C���x���g���̊J���

    private bool isPausedByDisplayItem = false; // �ꎞ��~��Ԃ�ǐ�
    private GameObject slot; //���ݕ\�����Ă���A�C�e�����i�[
    #endregion

    private void Start()
    {
        if (ItemDisplayUI != null)
        {
            ItemDisplayUI.SetActive(false); // ������ԂŔ�\��
        }
    }

    void Update()
    {
        if (isItemDisplayON)
        {
            // ���ԂɊ�Â��ď㉺�Ɉړ�
            float floatOffset = Mathf.Sin(Time.unscaledTime * 2f) * 0.05f; // �U���Ƒ��x�𒲐�
            currentDisplayedItem.transform.position = itemDisplayPosition.position + new Vector3(0f, floatOffset, 0f);
        }

        //�@�f�B�X�v���C��ʂ���鎞
        if (isPausedByDisplayItem && Input.GetMouseButtonDown(0)) // 0�͍��N���b�N
        {
            ResumeGame();
            isItemDisplayON = !isItemDisplayON;
            ItemDisplayUI.SetActive(isItemDisplayON);

            Cursor.lockState = CursorLockMode.Locked; // �}�E�X���\��
            Cursor.visible = false;                  // �}�E�X�J�[�\�����\��
            ClearDisplayedItem();                    // �\�����̃A�C�e�����N���A
        }
    }


    public void ToggleItemDisplay()
    {
        isItemDisplayON = !isItemDisplayON;

        if (ItemDisplayUI != null)
        {
            ItemDisplayUI.SetActive(isItemDisplayON);

            if (isItemDisplayON)
            {
                UpdateItemDisplayUI(); // �J�����Ƃ��ɓ��e���X�V
                Cursor.lockState = CursorLockMode.None; // �}�E�X��\��
                Cursor.visible = true;                 // �}�E�X�J�[�\����\��
            }
            
        }
    }

    private void UpdateItemDisplayUI()
    {
        

        // �ŐV�̃A�C�e�����擾
        var item = inventory.items[inventory.items.Count - 1];

        // �X���b�g�𐶐�
        slot = Instantiate(itemSlotPrefab, itemListParent);
        slot.name = "Displayed : " + item.item.name;

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
            Destroy(slot);
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // �Q�[�����ĊJ
        isPausedByDisplayItem = false; // �t���O�����Z�b�g
    }
}
