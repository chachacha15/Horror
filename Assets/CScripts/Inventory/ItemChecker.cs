using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemChecker : MonoBehaviour
{
    #region Variables
    public float interactDistance = 3f; // �C���^���N�g�\�ȋ���
    public LayerMask itemLayer; // �A�C�e���Ɏg�p���郌�C���[
    public GameObject interactText; // UI �e�L�X�g (�E�����b�Z�[�W��\��)
    public ItemDataBase itemDataBase; // �A�C�e���f�[�^�x�[�X���Q��
    public Inventory inventory; // �v���C���[�̃C���x���g�����Ǘ�����X�N���v�g
    public ItemDisplay itemDisplay;


    private TextMeshProUGUI interactTextComponent; // TextMeshPro�̎Q��

    [SerializeField] GameObject flashLightSystem;
    [SerializeField] GameObject flashlightTutorial;
    private TutorialManager tutorialManager;


    #endregion


    private void Start()
    {
        interactTextComponent = interactText.GetComponent<TextMeshProUGUI>();

        if (interactTextComponent == null)
        {
            Debug.LogError("interactText��TextMeshProUGUI�R���|�[�l���g��������܂���I");
        }

        
        tutorialManager = flashlightTutorial.GetComponent<TutorialManager>();
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

                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                {
                    PickupItem(hitItem);
                    itemDisplay.ToggleItemDisplay();
                    inventory.UpdateInventoryUI();

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
        // �A�C�e���̃f�[�^������
        PocketItem itemData = itemDataBase.itemList.Find(i => i.item.name == item.name);

        if (itemData != null)
        {
            // �C���x���g���ɃA�C�e����ǉ�
            inventory.AddItem(itemData);

            // Flashlight�̏ꍇ��FlashLightSystem���A�N�e�B�u��
            if (item.name == "Flashlight")
            {
                if (flashLightSystem != null)
                {
                    //�t���b�V�����C�g���g����悤�ɂ���
                    flashLightSystem.SetActive(true);
                    
                    if (tutorialManager != null)
                    {
                        StartCoroutine(tutorialManager.ShowTutorial()); // �R���[�`���𒼐ڌĂяo��
                    }

                    //�t���b�V�����C�g�̃`���[�g���A����\������
                    flashlightTutorial.SetActive(true);
                    flashlightTutorial.transform.GetChild(0).gameObject.SetActive(true);

                }
               
            }
        }
        else
        {
            Debug.LogWarning("�f�[�^�x�[�X�ɂ��̃A�C�e�������݂��܂���I");
        }

        // �A�C�e�����V�[������폜
        Destroy(item);
    }

    
}
