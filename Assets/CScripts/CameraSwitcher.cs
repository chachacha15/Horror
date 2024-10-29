using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera closetCamera;
    public MonoBehaviour playerLookScript; // PlayerLook�X�N���v�g���Q��
    public LayerMask layerMask; // ���C�L���X�g�̑Ώۃ��C���[
    private bool isClosetCameraActive = false; // ���݂̃J������Ԃ�ǐ�

    private void Start()
    {
        mainCamera = Camera.main; // ���C���J�����𓮓I�Ɏ擾
        closetCamera = FindClosetCamera(); // �N���[�[�b�g�J�����𓮓I�Ɏ擾
        playerLookScript = mainCamera.GetComponent<PlayerLook>(); // PlayerLook�X�N���v�g�𓮓I�Ɏ擾
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���N���b�N�����o
        {
            if (isClosetCameraActive)
            {
                SwitchToMainCamera();
            }
            else
            {
                Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider.CompareTag("Closet"))
                    {
                        SwitchToClosetCamera();
                    }
                }
            }
        }
    }

    void SwitchToClosetCamera()
    {
        mainCamera.gameObject.SetActive(false);
        closetCamera.gameObject.SetActive(true);
        playerLookScript.enabled = false; // PlayerLook�X�N���v�g�𖳌���
        isClosetCameraActive = true;
    }

    void SwitchToMainCamera()
    {
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

