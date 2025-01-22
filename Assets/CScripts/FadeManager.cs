using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage; // �t�F�[�h�p��Image
    public float fadeDuration = 1.5f; // �t�F�[�h�ɂ����鎞��

    private void Start()
    {
        // �J�n���Ƀt�F�[�h�C�������s
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration); // ���l�����X�Ɍ���
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color; // ���S�ɓ�����
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); // ���l�����X�ɑ���
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color; // ���S�ɕs������
    }
}
