using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class Skull : MonoBehaviour
{
    #region Variables
    //�{�����ǂ���
    private bool isReal = false;

    // �{�A�j���[�V�����p
    public Transform jaw; // �{�̃I�u�W�F�N�g
    private bool isJawOpen = false;
    public float jawOpenSpeed = 1.5f; // �{���J���X�s�[�h�i�b�j

    // �����A�C�e���p
    public Transform itemSpawnPoint; // �A�C�e���𐶐�����ʒu
    public List<GameObject> itemPrefabs; // ������o��A�C�e���̃��X�g

    // ���̑��E���N���X
    private bool isLookingSkull = false;

    private SkullManager manager;
    private CameraSwitcher cameraSwitcher;

    #endregion

    private void Start()
    {
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        manager = FindObjectOfType<SkullManager>();
    }

    private void Update()
    {
        if (isLookingSkull)
        {
            cameraSwitcher.ClosshairAnimation(10f, 500f, 0.5f, cameraSwitcher.crosshairRectTransform, isLookingSkull);
        }
        else if(!isLookingSkull)
        {
            cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingSkull);
        }
    }

    public void SetReal(bool real)
    {
        isReal = real;
    }

    private void OnMouseEnter()
    {
        // �J�[�\�������킹���Ƃ�
        isLookingSkull = true;
        Debug.Log($"{gameObject.name} �ɃJ�[�\�������킹��");
    }

    private void OnMouseExit()
    {
        // �J�[�\�����O�ꂽ�Ƃ��̏���
        isLookingSkull=false;

    }

    private void OnMouseDown()
    {
        // �[�����N���b�N������Ǘ��X�N���v�g�ɒʒm
        manager.ShowConfirmationUI(this);
    }

    public void RevealReal()
    {
        Debug.Log("�{���̊[�������j�����I �{���O���");

        // �{���O���i��: Y���ɉ�]�j
        if (jaw != null && !isJawOpen)
        {
            isJawOpen = true;
            StartCoroutine(OpenJawSmoothly()); // �������J���R���[�`�����J�n
        }
    }

    private IEnumerator OpenJawSmoothly()
    {
        float elapsedTime = 0f;
        Quaternion startRotation = jaw.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, -70, 50); // X�������ɊJ��



        while (elapsedTime < jawOpenSpeed)
        {
            elapsedTime += Time.deltaTime;
            jaw.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / jawOpenSpeed);
            yield return null;
        }

        jaw.localRotation = targetRotation; // �ŏI�ʒu���Z�b�g
        SpawnItem();

    }

    private void SpawnItem()
    {
        if (itemPrefabs.Count == 0)
        {
            Debug.LogWarning("�A�C�e�����X�g����ł��I");
            return;
        }

        // �����_���ȃA�C�e����I��
        GameObject selectedItem = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

        // �A�C�e���𐶐�
        GameObject spawnedItem = Instantiate(selectedItem, itemSpawnPoint);

    }

    public void DisableColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(); // �����Ǝq�I�u�W�F�N�g�̃R���C�_�[�����ׂĎ擾
        foreach (Collider col in colliders)
        {
            col.enabled = false; // �R���C�_�[�𖳌���
        }
    }

}
