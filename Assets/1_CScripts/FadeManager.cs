using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage; // フェード用のImage
    public float fadeDuration = 1.5f; // フェードにかかる時間

    private void Start()
    {
        // 開始時にフェードインを実行
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration); // α値を徐々に減少
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color; // 完全に透明に
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); // α値を徐々に増加
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color; // 完全に不透明に
    }
}
