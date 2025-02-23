/*using System.Collections;
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
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // URP�p
using TMPro; // TextMeshPro �p

public class BathTub : MonoBehaviour
{
    [SerializeField] private List<DecalProjector> bloodDecals; // �t�F�[�h�A�E�g���錌�� Decal �ꗗ
    [SerializeField] private float fadeDuration = 2f; // �t�F�[�h����
    [SerializeField] private TextMeshProUGUI hiddenNumberText; // �����ɕ\�����郉���_���Ȑ���
    [SerializeField] private float numberFadeDuration = 2f; // �����̃t�F�[�h�C������
    private Inventory inventory;
    private bool isCleaning = false;

    private ItemChecker itemChecker; // ItemChecker �̎Q��

    private void Start()
    {
        // �V�[������ ItemChecker ��T���Ď擾
        itemChecker = FindObjectOfType<ItemChecker>();
        inventory = FindObjectOfType<Inventory>();

        // ������Ԃł͐����𓧖��ɂ���
        if (hiddenNumberText != null)
        {
            Color color = hiddenNumberText.color;
            color.a = 0;
            hiddenNumberText.color = color;
        }
    }

    private void OnMouseDown()
    {
        // �X�|���W�������Ă���ꍇ�̂݃t�F�[�h�A�E�g
        if (itemChecker != null && inventory.selectedItem != null && inventory.selectedItem.item.name == "sponge" && !isCleaning)
        {
            inventory.RemoveHeldItem(); // �X�|���W������
            StartCoroutine(FadeOutBloodAndShowNumber());
        }
    }

    private IEnumerator FadeOutBloodAndShowNumber()
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

            // ���̃f�J�[�����t�F�[�h�A�E�g
            for (int i = 0; i < bloodDecals.Count; i++)
            {
                if (bloodDecals[i] != null)
                {
                    bloodDecals[i].fadeFactor = Mathf.Lerp(startOpacities[i], 0f, t);
                }
            }

            yield return null;
        }

        // �t�F�[�h�A�E�g��A���ׂĂ̌��̃f�J�[�����\��
        foreach (var decal in bloodDecals)
        {
            if (decal != null)
            {
                decal.gameObject.SetActive(false);
            }
        }

        // �����������_���Ɍ���
        int randomNumber = Random.Range(1000, 9999); // 4���̃����_���Ȑ���
        hiddenNumberText.text = randomNumber.ToString();

        // �����̃t�F�[�h�C��
        StartCoroutine(FadeInNumber());

        isCleaning = false;
    }

    private IEnumerator FadeInNumber()
    {
        float elapsedTime = 0f;
        Color color = hiddenNumberText.color;

        while (elapsedTime < numberFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / numberFadeDuration);
            hiddenNumberText.color = color;
            yield return null;
        }
    }
}
