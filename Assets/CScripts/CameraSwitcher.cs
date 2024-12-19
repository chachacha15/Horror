using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera closetCamera;
   
    public MonoBehaviour playerLookScript; // PlayerLook�X�N���v�g���Q��
    public LayerMask layerMask; // ���C�L���X�g�̑Ώۃ��C���[
    public float hideDistance = 5f;    // �B����鋗��

    private bool isClosetCameraActive = false; // ���݂̃J������Ԃ�ǐ�


    public Image crosshair;   // �N���X�w�A��Image�R���|�[�l���g
    public Sprite normalCrosshair; // �ʏ펞�̃X�v���C�g
    public Sprite closetCrosshair; // �N���[�[�b�g���̃X�v���C�g

    public GameObject hideText;       // �B���Text�I�u�W�F�N�g


    public GameObject player;
    public bool isPlayerHiding = false;

    public RectTransform crosshairRectTransform; // �N���X�w�A��RectTransform
   

    private float currentSize; // ���݂̃T�C�Y
    private bool isLookingAtCloset = false; // �N���[�[�b�g�����Ă����Ԃ�

    private void Start()
    {   

        // �B���Text�𓮓I�Ɏ擾�i�I�u�W�F�N�g���� "�B���Text" �̏ꍇ�j
        hideText = GameObject.Find("�B���Text");
        // ������ԂŔ�\����
        hideText.SetActive(false);

        mainCamera = Camera.main; // ���C���J�����𓮓I�Ɏ擾
        closetCamera = FindClosetCamera(); // �N���[�[�b�g�J�����𓮓I�Ɏ擾
        player = GameObject.FindWithTag("Player");
        
        playerLookScript = mainCamera.GetComponent<PlayerLook>(); // PlayerLook�X�N���v�g�𓮓I�Ɏ擾
    }
    void Update()
    {
        // �N���[�[�b�g�ɃJ�[�\�������邩�𔻒�
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hideDistance, layerMask))
        {
            if (hit.collider.CompareTag("Closet") && !isClosetCameraActive)
            {
                // �N���[�[�b�g�����Ă���ꍇ
                isLookingAtCloset = true;
                hideText.SetActive(true); // �B���Text��ON��

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

        ClosshairAnimation(10f, 35f, 5f,crosshairRectTransform, isLookingAtCloset);
        
        

        if (Input.GetMouseButtonDown(0)) // ���N���b�N�����o
        {
            if (isClosetCameraActive)
            {
                SwitchToMainCamera();
            }
            else
            {
                

                if (Physics.Raycast(ray, out hit, hideDistance, layerMask))
                {
                    if (hit.collider.CompareTag("Closet"))
                    {
                        SwitchToClosetCamera();
                    }
                }
            }
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

    void SwitchToClosetCamera()
    {
        isPlayerHiding = true;
        mainCamera.gameObject.SetActive(false);
        closetCamera.gameObject.SetActive(true);
        playerLookScript.enabled = false; // PlayerLook�X�N���v�g�𖳌���
        isClosetCameraActive = true;
        
    }

    void SwitchToMainCamera()
    {
        isPlayerHiding = false;
        mainCamera.gameObject.SetActive(true);
        closetCamera.gameObject.SetActive(false);
        playerLookScript.enabled = true; // PlayerLook�X�N���v�g��L����
        isClosetCameraActive = false;
        
    }

    Camera FindClosetCamera()
    {
        GameObject closet = GameObject.FindGameObjectWithTag("Closet");
        if (closet != null)
        {
            Transform[] children = closet.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.CompareTag("ClosetCamera"))
                {
                    return child.GetComponent<Camera>();
                }
            }
        }
        return null;
    }
}

