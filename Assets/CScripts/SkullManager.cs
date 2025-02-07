using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;


public class SkullManager : MonoBehaviour
{
    #region Variables
    public GameObject fakeSkullPrefab; // �U���̊[���i�S�ē������́j
    public List<GameObject> realSkullPrefabs; // �{���̊[���̌��i���X�g����1�̂�I�ԁj
    public Transform[] spawnPoints; // �[����z�u����ʒu�i3�ӏ��j

    public GameObject confirmationUI; // �m�F�pUI
    public TextMeshProUGUI confirmationText; // �u���̊[�����{���ł����H�v�̃��b�Z�[�W
    private Skull selectedSkull; // �N���b�N���ꂽ�[�����L�^


    private Skull realSkullInstance; // ���ۂɔz�u���ꂽ�{���̊[��

    private bool isConfirmed = false; // UI����x�J������true�ɂȂ�

    #endregion

    private void Start()
    {

        SpawnSkulls();
    }

    private void SpawnSkulls()
    {
        if (realSkullPrefabs.Count == 0)
        {
            Debug.LogError("�{���̊[��Prefab���X�g����ł��I");
            return;
        }

        // �{���̊[�������X�g���烉���_����1�I��
        GameObject selectedRealSkullPrefab = realSkullPrefabs[Random.Range(0, realSkullPrefabs.Count)];

        if (selectedRealSkullPrefab == null)
        {
            Debug.LogError("�I�����ꂽ�{���̊[��Prefab�� null �ł��I");
            return;
        }

        // 3�̂̊[����z�u�i1�͖{���j
        int realSkullIndex = Random.Range(0, spawnPoints.Length);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject spawnedSkull;

            if (i == realSkullIndex)
            {
                // �{���̊[����z�u
                spawnedSkull = Instantiate(selectedRealSkullPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                realSkullInstance = spawnedSkull.GetComponent<Skull>();
                realSkullInstance.SetReal(true);
            }
            else
            {
                // �U���̊[����z�u
                spawnedSkull = Instantiate(fakeSkullPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                spawnedSkull.GetComponent<Skull>().SetReal(false);
            }
        }
    }

    // �v���C���[���[�����N���b�N�������̏���
    public void OnSkullClicked(Skull clickedSkull)
    {
        if (clickedSkull == realSkullInstance)
        {
            clickedSkull.RevealReal(); // �{���Ȃ�{���O��
        }
        else
        {
            Debug.Log("����͋U�����I");
        }
    }

    public void ShowConfirmationUI(Skull skull)
    {
        // ���łɊm�F�ς݂Ȃ�UI��\�����Ȃ�
        if (isConfirmed) return;

        selectedSkull = skull;
        confirmationText.text = "���̊[�����{���ł����H";
        confirmationUI.SetActive(true); // UI��\��
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None; // �}�E�X��\��
        Cursor.visible = true;
    }

    public void OnConfirm() // OK�{�^���������ꂽ�Ƃ�
    {
        if (selectedSkull != null)
        {
            DisableAllSkulls(); // ���ׂĂ̊[���̃R���C�_�[�𖳌���
            OnSkullClicked(selectedSkull); // �{�����J��
        }
        confirmationUI.SetActive(false); // UI�����
        Cursor.lockState = CursorLockMode.Locked; // �}�E�X���\��
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        isConfirmed = true; // �ȍ~�AUI���J�����Ȃ�



    }

    public void OnCancel() // �L�����Z���{�^���������ꂽ�Ƃ�
    {
        confirmationUI.SetActive(false); // UI�����
        Cursor.lockState = CursorLockMode.Locked; // �}�E�X���\��
        Cursor.visible = false;
        Time.timeScale = 1.0f;

    }

    private void DisableAllSkulls()
    {
        // �V�[�����̂��ׂĂ�Skull���擾���āA�R���C�_�[�𖳌���
        Skull[] allSkulls = FindObjectsOfType<Skull>();
        foreach (Skull skull in allSkulls)
        {
            skull.DisableColliders();
        }
    }

}
