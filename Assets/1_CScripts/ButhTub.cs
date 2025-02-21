using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // URP�p

public class BathTub : MonoBehaviour
{
    [SerializeField] private List<DecalProjector> bloodDecals; // ������ Decal Projector ���Ǘ�
    [SerializeField] private float fadeDuration = 2f; // �t�F�[�h�A�E�g����
    [SerializeField] private DecalProjector answerDecal; // �����̃f�J�[��
    private Inventory inventory;
    private bool isCleaning = false;

    private ItemChecker itemChecker; // ItemChecker �̎Q��

    private void Start()
    {
        // �V�[������ ItemChecker ��T���Ď擾
        itemChecker = FindObjectOfType<ItemChecker>();
        inventory = FindObjectOfType<Inventory>();
    }

    private void OnMouseDown()
    {
        // �X�|���W�������Ă���ꍇ�̂݃t�F�[�h�A�E�g
        if (itemChecker != null && inventory.selectedItem != null && inventory.selectedItem.item.name == "sponge" && !isCleaning)
        {
            inventory.RemoveHeldItem();
            StartCoroutine(FadeOutBlood());
        }
    }

    private IEnumerator FadeOutBlood()
    {
        isCleaning = true;
        float elapsedTime = 0f;

        // �����̕s�����x���擾
        List<float> startOpacities = new List<float>();
        foreach (var decal in bloodDecals)
        {
            startOpacities.Add(decal.fadeFactor);
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            for (int i = 0; i < bloodDecals.Count; i++)
            {
                if (bloodDecals[i] != null)
                {
                    bloodDecals[i].fadeFactor = Mathf.Lerp(startOpacities[i], 0f, t);
                }
            }
            answerDecal.fadeFactor = Mathf.Lerp(0f,1f, t);

            yield return null;
        }

        // �t�F�[�h�A�E�g��A���ׂẴf�J�[���𖳌���
        foreach (var decal in bloodDecals)
        {
            if (decal != null)
            {
                decal.gameObject.SetActive(false);
            }
        }

        isCleaning = false;
    }
}
