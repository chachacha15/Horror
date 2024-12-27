using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask layerMask; // ���C�L���X�g�̑Ώۃ��C���[

    public Animator animator; // �h�A��Animator
    public Transform player; // �v���C���[��Transform
    public float interactionDistance = 6f; // �h�A�Ƃ̃C���^���N�V��������

    private bool isOpen = false;
    private bool isLookingAtDoor = false; // �N���[�[�b�g�����Ă����Ԃ�

    public GameObject doortext;
    TextMeshProUGUI doorGUI;

    public Image crosshair;   // �N���X�w�A��Image�R���|�[�l���g
    private float currentSize; // ���݂̃T�C�Y
    CameraSwitcher cameraSwitcher;
    void Start()
    {
        


        // �����̐e�I�u�W�F�N�g���擾
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // �e�̎q���̒�����Canvas��T��
            Transform canvasTransform = parentTransform.Find("Canvas");

            if (canvasTransform != null)
            {
                // Canvas�̎q���̒�����J��Text��T��
                Transform textTransform = canvasTransform.Find("�J��Text");

                if (textTransform != null)
                {
                    // Text�R���|�[�l���g���擾
                    doortext = textTransform.gameObject;
                }
            }
        }

        // TextMeshProUGUI�ւ̎Q��
        doorGUI = doortext.GetComponent<TextMeshProUGUI>();
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        animator = GetComponent<Animator>();

        // Animator��isOpen�p�����[�^��������Ԃɓ���
        if (animator != null)  animator.SetBool("isOpen", isOpen);

        //MainCamera���^�O�œ��I�Ɏ擾
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

    }
    void Update()
    {

        // �N���[�[�b�g�ɃJ�[�\�������邩�𔻒�
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // �v���C���[���߂Â�����Click�ŊJ��
        if (Physics.Raycast(ray, out hit, interactionDistance, layerMask))
        {
            if (hit.collider.gameObject == gameObject) // ���݂̃h�A�Ɉ�v����ꍇ
            {
                isLookingAtDoor = true;
                if (doortext != null) doortext.SetActive(true); // �J��Text���\��

            }
            else
            {
                isLookingAtDoor = false;
                if (doortext != null) doortext.SetActive(false); // �J��Text���\��

            }
        }
        else
        {
            isLookingAtDoor = false;
            if (doortext != null) doortext.SetActive(false); // �J��Text���\��

            cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingAtDoor);
        }

        cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingAtDoor);

        // ���N���b�N���Ƀh�A���J��
        if (Input.GetMouseButtonDown(0) && isLookingAtDoor)
        {
            ToggleDoor();
        }



    }

    public void ToggleDoor()
    {

        isOpen = !isOpen;

        animator.SetBool("isOpen", isOpen);


        if (doorGUI != null) doorGUI.text = isOpen ? "�߂�" : "�J����";

    }
}
