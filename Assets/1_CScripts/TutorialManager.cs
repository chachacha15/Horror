using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public CanvasGroup tutorialCanvasGroup; // チュートリアル全体のフェード管理
    public float displayTime = 4f;         // 表示時間
    public float fadeDuration = 1f;         // フェード時間
    private bool isFading = false;          // フェード中かどうかのフラグ

    private void Start()
    {
        if(tutorialCanvasGroup.name == "StartTutorial")
        {
            // チュートリアルを表示
            StartCoroutine(ShowTutorial());
        }

    }
        

    //private void Update()
    //{
    //    // ユーザーがクリックしたら即座にフェードアウト
    //    if (Input.GetMouseButtonDown(0) && !isFading)
    //    {
    //        StartCoroutine(FadeOut());
    //    }
    //}

    public IEnumerator ShowTutorial()
    {
        // 表示中のアルファ値を最大に設定
        tutorialCanvasGroup.alpha = 1.0f;

        // 表示時間を待つ
        yield return new WaitForSeconds(displayTime);

        // フェードアウト開始
        if (!isFading)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        isFading = true;

        // フェードアウト処理
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

        // チュートリアルを完全に非表示に
        tutorialCanvasGroup.gameObject.SetActive(false);
    }
}
