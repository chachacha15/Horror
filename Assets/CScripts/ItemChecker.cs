using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemChecker : MonoBehaviour
{
    public float interactDistance = 3f; // �C���^���N�g�\�ȋ���
    public LayerMask itemLayer; // �A�C�e���Ɏg�p���郌�C���[
    public GameObject interactText; // UI �e�L�X�g (�E�����b�Z�[�W��\��)
    public ItemDataBase itemDataBase; // �A�C�e���f�[�^�x�[�X���Q��
    public Inventory inventory; // �v���C���[�̃C���x���g�����Ǘ�����X�N���v�g
    public InventoryDisplay inventoryDisplay;

    public Transform itemDisplayArea; // 3D���f���\���G���A
    public Camera itemDisplayCamera; // �A�C�e���\���p�J����
    private GameObject currentDisplayedItem; // ���ݕ\�����̃A�C�e��

    private TextMeshProUGUI interactTextComponent; // TextMeshPro�̎Q��

    private void Start()
    {
        interactTextComponent = interactText.GetComponent<TextMeshProUGUI>();

        if (interactTextComponent == null)
        {
            Debug.LogError("interactText��TextMeshProUGUI�R���|�[�l���g��������܂���I");
        }

        if (itemDisplayCamera == null || itemDisplayArea == null)
        {
            Debug.LogError("ItemDisplayCamera�܂���ItemDisplayArea���ݒ肳��Ă��܂���I");
        }
    }

    private void Update()
    {
        // ���C�L���X�g�ŃA�C�e�������o
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        

        if (Physics.Raycast(ray, out hit, interactDistance, itemLayer))
        {
            GameObject hitItem = hit.collider.gameObject;

            if (hitItem.CompareTag("Item"))
            {
                interactTextComponent.text = $"���";
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
            //Display3DItem(itemData.item); // �A�C�e����3D�\��
        }
        else
        {
            Debug.LogWarning("�f�[�^�x�[�X�ɂ��̃A�C�e�������݂��܂���I");
        }

        Destroy(item);
    }

    //private void Display3DItem(GameObject itemPrefab)
    //{
    //    // �����̕\���A�C�e�����폜
    //    if (currentDisplayedItem != null)
    //    {
    //        Destroy(currentDisplayedItem);
    //    }

    //    // �A�C�e����\���G���A�ɃC���X�^���X��
    //    currentDisplayedItem = Instantiate(itemPrefab, itemDisplayArea);
    //    currentDisplayedItem.transform.localPosition = Vector3.zero;
    //    currentDisplayedItem.transform.localRotation = Quaternion.identity;
    //    currentDisplayedItem.transform.localScale = Vector3.one;
    //}
}
