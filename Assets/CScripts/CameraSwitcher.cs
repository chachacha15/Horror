using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityStandardAssets.Utility; // CurveControlledBob ���g�p

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    //public Camera closetCamera;

    public MonoBehaviour playerLookScript; // PlayerLook�X�N���v�g���Q��

    public LayerMask layerMask; // ���C�L���X�g�̑Ώۃ��C���[
    public float hideDistance = 5f;    // �B����鋗��

    private bool isClosetCameraActive = false; // ���݂̃J������Ԃ�ǐ�
    private Camera currentClosetCamera; // ���݂̃N���[�[�b�g�J������ǐ�

    public Image crosshair;   // �N���X�w�A��Image�R���|�[�l���g
    public Sprite normalCrosshair; // �ʏ펞�̃X�v���C�g
    public Sprite closetCrosshair; // �N���[�[�b�g���̃X�v���C�g

    public GameObject hideText;       // �B���Text�I�u�W�F�N�g
    public GameObject player;
    public bool isPlayerHiding = false;

    public RectTransform crosshairRectTransform; // �N���X�w�A��RectTransform

    private float currentSize; // ���݂̃T�C�Y
    private bool isLookingAtCloset = false; // �N���[�[�b�g�����Ă����Ԃ�
    public bool hasHiddenUnderDesk = false; //���̓N���[�[�b�g�ɉB�ꂽ���Ƃ����邩
    private Vector3 targetCameraBaseLocalPosition;

    // �J�����h��p
    [SerializeField] private CurveControlledBob bob = new CurveControlledBob();
    private Transform closetCameraTransform;

    private void Start()
    {
        // �B���Text�𓮓I�Ɏ擾�i�I�u�W�F�N�g���� "�B���Text" �̏ꍇ�j
        hideText = GameObject.Find("�B���Text");
        // ������ԂŔ�\����
        hideText.SetActive(false);

        mainCamera = Camera.main; // ���C���J�����𓮓I�Ɏ擾
        player = GameObject.FindWithTag("Player");

        playerLookScript = mainCamera.GetComponent<PlayerLook>(); // PlayerLook�X�N���v�g�𓮓I�Ɏ擾

        targetCameraBaseLocalPosition = this.transform.localPosition;

    }

    void Update()
    {
        // �N���[�[�b�g�ɃJ�[�\�������邩�𔻒�
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hideDistance, layerMask))
        {
            if (hit.collider.CompareTag("Closet"))
            {
                isLookingAtCloset = true;
                hideText.SetActive(true); // �B���Text��ON��

                // ���N���b�N�ŃJ������؂�ւ���
                if (Input.GetMouseButtonDown(0))
                {
                    if (isClosetCameraActive)
                    {
                        // �N���[�[�b�g�J�������A�N�e�B�u�Ȃ烁�C���J�����ɖ߂�
                        SwitchToMainCamera();
                    }
                    else
                    {
                        // �N���[�[�b�g�J�����ɐ؂�ւ�
                        Camera targetClosetCamera = FindClosetCamera(hit.collider.gameObject);
                        if (targetClosetCamera != null)
                        {
                            SwitchToClosetCamera(targetClosetCamera);
                            targetClosetCamera.transform.localPosition = 
                                new Vector3( targetClosetCamera.transform.localPosition.x,
                                             0,
                                             targetClosetCamera.transform.localPosition.z);

                            // �J�����h��̃Z�b�g�A�b�v
                            bob.Setup(targetClosetCamera, 1.0f); 

                        }
                        else
                        {
                            Debug.LogWarning("�Ώۂ̃N���[�[�b�g�ɃJ������������܂���I");
                        }

                        hasHiddenUnderDesk = true;
                    }
                }
            }
            else
            {
                // ���̃I�u�W�F�N�g�̏ꍇ
                isLookingAtCloset = false;
                hideText.SetActive(false); // �B���Text��OFF��
            }
        }
        else
        {
            // �����q�b�g���Ă��Ȃ��ꍇ
            isLookingAtCloset = false;
            hideText.SetActive(false); // �B���Text��OFF��
        }

        ClosshairAnimation(10f, 500f, 0.5f, crosshairRectTransform, isLookingAtCloset);

        // �B��Ă���Ԃ̃J�����h��
        if (isClosetCameraActive && closetCameraTransform != null)
        {
            Vector3 bobOffset = bob.DoHeadBob(0.15f); // �h��̌v�Z
            closetCameraTransform.localPosition = bobOffset; // �N���[�[�b�g�J������h�炷
        }
    }

    public void ClosshairAnimation(float normalSize, float targetSize, float animationSpeed,
        RectTransform chRectTransform, bool isLooking)
    {
        //crosshairRectTransform.sizeDelta = new Vector2(normalSize, normalSize);
        // �T�C�Y���A�j���[�V�����ŕύX
        float target = isLooking ? targetSize : normalSize;
        currentSize = Mathf.Lerp(currentSize, target, animationSpeed * Time.deltaTime);
        chRectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }

    void SwitchToClosetCamera(Camera targetCamera)
    {
        isPlayerHiding = true;
        mainCamera.gameObject.SetActive(false);
        targetCamera.gameObject.SetActive(true); // �w�肳�ꂽ�J�������A�N�e�B�u��
        playerLookScript.enabled = false; // PlayerLook�X�N���v�g�𖳌���
        isClosetCameraActive = true;

        currentClosetCamera = targetCamera; // ���݂̃N���[�[�b�g�J������ێ�
        closetCameraTransform = targetCamera.transform; // �N���[�[�b�g�J������Transform���擾

        // �N���X�w�A�ƉB���e�L�X�g���\���ɂ���
        crosshair.gameObject.SetActive(false);
        hideText.SetActive(false);

        // �v���C���[�̃I�u�W�F�N�g�𖳌���
        player.SetActive(false);
    }

    void SwitchToMainCamera()
    {
        isPlayerHiding = false;
        mainCamera.gameObject.SetActive(true);

        if (currentClosetCamera != null)
        {
            currentClosetCamera.gameObject.SetActive(false); // ���݂̃N���[�[�b�g�J�����𖳌���
            currentClosetCamera = null; // �ێ�����J���������Z�b�g
        }

        playerLookScript.enabled = true; // PlayerLook�X�N���v�g��L����
        isClosetCameraActive = false;

        // �N���X�w�A���ĕ\��
        crosshair.gameObject.SetActive(true);

        // �v���C���[�I�u�W�F�N�g��L����
        player.SetActive(true);
    }

    Camera FindClosetCamera(GameObject closetObject)
    {
        Transform[] children = closetObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.CompareTag("ClosetCamera")) // �N���[�[�b�g�J������T��
            {
                return child.GetComponent<Camera>();
            }
        }
        return null; // �J������������Ȃ��ꍇ
    }
}
