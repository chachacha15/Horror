using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;


    public Image fadeImage; // フェード用のImage
    public float fadeDuration = 1.5f; // フェードにかかる時間


    // 他クラス
    private GameStart gameStart;

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        gameStart = FindObjectOfType<GameStart>();

        // 開始時にフェードインを実行
        FadeIn();
    }



    public void FadeIn()
    {
        StartCoroutine(FadeInImage(fadeImage));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutImage(fadeImage));
    }

    public IEnumerator FadeOutAndSceneChange(string sceneName)
    {
        yield return StartCoroutine(FadeOutImage(fadeImage));
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeInImage(Image image)
    {
        float elapsedTime = 0f;
        Color color = image.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration); // α値を徐々に減少
            image.color = color;
            yield return null;
        }
        color.a = 0f;
        image.color = color; // 完全に透明に
    }

    private IEnumerator FadeOutImage(Image image)
    {
        float elapsedTime = 0f;
        Color color = image.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / (fadeDuration)); // α値を徐々に増加
            image.color = color;
            yield return null;
        }
        color.a = 1f;
        image.color = color; // 完全に不透明に
    }
}
