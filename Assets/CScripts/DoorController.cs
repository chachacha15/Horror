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
    private float interactionDistance = 10f; // �h�A�Ƃ̃C���^���N�V��������

    private bool isOpen = false;
    private bool isLookingAtDoor = false; // �N���[�[�b�g�����Ă����Ԃ�

    public GameObject doortext;
    TextMeshProUGUI doorGUI;

    public Image crosshair;   // �N���X�w�A��Image�R���|�[�l���g
    private float currentSize; // ���݂̃T�C�Y
    CameraSwitcher cameraSwitcher;

    private bool isLockedDoor = true; // �h�A�����܂��Ă��邩
    private string requiredKeyName; // �K�v�ȃJ�M�̖��O

    private AudioSource audioSource; // �����Đ�����AudioSource
    public AudioClip UnLockSound; // �J����
    public AudioClip CardKeySound; // �s�b�Ƃ����J�[�h�L�[�F�؉�
    public AudioClip LockedSound; // �K�`���K�`���Ƃ����J�����Ȃ���

    public Inventory inventory; // �v���C���[�̃C���x���g��

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        inventory = FindObjectOfType<Inventory>();


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


        // �I�u�W�F�N�g�����琔���𒊏o���ĕK�v�ȃJ�M��ݒ�
        requiredKeyName = GetRequiredKeyNameFromObjectName(gameObject.name);
        if(requiredKeyName == null) isLockedDoor= false;
    }
    void Update()
    {

        // �N���[�[�b�g�ɃJ�[�\�������邩�𔻒�
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // �f�o�b�O�p�F���C�L���X�g�̉���
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.magenta);

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

        cameraSwitcher.ClosshairAnimation(10f, 500f, 0.5f, cameraSwitcher.crosshairRectTransform, isLookingAtDoor);

        // ���N���b�N���Ƀh�A���J��
        if (Input.GetMouseButtonDown(0) && isLookingAtDoor)
        {
            if (isLockedDoor)
            {
                if (HasRequiredKey())
                {
                    isLockedDoor = false;
                    audioSource.PlayOneShot(CardKeySound);
                    StartCoroutine(PlaySoundWithDelay(UnLockSound, 0.35f));
                }
                else
                {
                    audioSource.PlayOneShot(LockedSound);
                    StartCoroutine(DelayText());
                }
            }
            else
            {
                ToggleDoor();
            }

        }



    }

    public void ToggleDoor()
    {

        isOpen = !isOpen;

        animator.SetBool("isOpen", isOpen);


        if (doorGUI != null) doorGUI.text = isOpen ? "�߂�" : "�J����";

    }


    // �w�肵�������w�肵���x�����Ԍ�ɍĐ�
    private IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay); // �w�肵���b�������҂�
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // �����Đ�
        }
    }

    // �I�u�W�F�N�g������K�v�ȃJ�M�̖��O���擾
    private string GetRequiredKeyNameFromObjectName(string objectName)
    {
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(objectName, @"\d+");
        Debug.Log(objectName);
        Debug.Log(match.Value);
        if (match.Success)
        {
            return $"�J�[�h�L�[({match.Value}����)"; // �K�v�ȃJ�M�̖��O�𐶐�
        }
        else
        {
            return null; // �������Ȃ��ꍇ�̓J�M�s�v
        }
    }

    // �K�v�ȃJ�M�������Ă��邩�m�F
    private bool HasRequiredKey()
    {
        foreach (var item in inventory.items)
        {
            if (item.item.name == requiredKeyName)
            {
                return true; // �J�M�������Ă���
            }
        }
        return false; // �J�M���Ȃ�
    }

    IEnumerator DelayText()
    {
        doorGUI.text = "�J���Ȃ�";
        yield return new WaitForSeconds(1.0f);
        doorGUI.text = "�J����";
    }
}
