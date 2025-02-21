using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public CanvasGroup tutorialCanvasGroup; // �`���[�g���A���S�̂̃t�F�[�h�Ǘ�
    public float displayTime = 4f;         // �\������
    public float fadeDuration = 1f;         // �t�F�[�h����
    private bool isFading = false;          // �t�F�[�h�����ǂ����̃t���O

    private void Start()
    {
        if(tutorialCanvasGroup.name == "StartTutorial")
        {
            // �`���[�g���A����\��
            StartCoroutine(ShowTutorial());
        }

    }
        

    //private void Update()
    //{
    //    // ���[�U�[���N���b�N�����瑦���Ƀt�F�[�h�A�E�g
    //    if (Input.GetMouseButtonDown(0) && !isFading)
    //    {
    //        StartCoroutine(FadeOut());
    //    }
    //}

    public IEnumerator ShowTutorial()
    {
        // �\�����̃A���t�@�l���ő�ɐݒ�
        tutorialCanvasGroup.alpha = 1.0f;

        // �\�����Ԃ�҂�
        yield return new WaitForSeconds(displayTime);

        // �t�F�[�h�A�E�g�J�n
        if (!isFading)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        isFading = true;

        // �t�F�[�h�A�E�g����
        float startAlpha = tutorialCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tutorialCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        tutorialCanvasGroup.alpha = 0;
        isFading = false;

        // �`���[�g���A�������S�ɔ�\����
        tutorialCanvasGroup.gameObject.SetActive(false);
    }
}
