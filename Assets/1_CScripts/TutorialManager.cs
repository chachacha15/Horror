using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    #region Constants

    private const float DISPLAY_DURATION = 5.0f; // 時間制御時の表示時間
    private const float FADE_DURATION = 1.0f;    // フェードアウト時のフェード時間

    #endregion

    #region Variables

    public static TutorialManager Instance;

    [SerializeField] private TutorialTextData[] tutorialTextDataList; // 全チュートリアルテキストデータ

    [SerializeField] private TextMeshProUGUI tutorialText; // チュートリアルテキスト
    private CanvasGroup tutorialCanvasGroup; // チュートリアル全体のフェード管理

    private bool isFading = false;          // フェード中かどうかのフラグ


    // 他クラス


    #endregion


    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // 他クラスを取得


        tutorialCanvasGroup = tutorialText.GetComponent<CanvasGroup>();
        tutorialCanvasGroup.alpha = 0f;
        if(tutorialCanvasGroup.name == "StartTutorial")
        {
            // チュートリアルを表示
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

    private void UpdateTutorialText(string text)
    {
        tutorialText.text = text;
    }
   

    public void SetTutorialText(TutorialDataType type)
    {
        foreach(var data in tutorialTextDataList)
        {
            if (data.Type == type)
            {
                UpdateTutorialText(data.Text);
                break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public TutorialTextData GetTutorialTextData(TutorialDataType type)
    {
        foreach (var data in tutorialTextDataList)
        {
            if (data.Type == type)
            {
                return data;
            }
        }
        return null;
    }

    public IEnumerator ShowTutorial(TutorialDataType type)
    {
        // タイプからデータを特定
        TutorialTextData data = GetTutorialTextData(type);
        Debug.Log(data);
        SetTutorialText(data.Type); // テキストを変更

        // 表示中のアルファ値を最大に設定
        tutorialCanvasGroup.alpha = 1.0f;

        // 時間ベースで制御するチュートリアルは一定の時間でフェードアウト
        if (data.IsTimeBasedDisappear)
        {
            // 表示時間を待つ
            yield return new WaitForSeconds(DISPLAY_DURATION);

            // フェードアウト開始
            if (!isFading)
            {
                StartCoroutine(FadeOut());
            }
        }
    }

    public void DisappearTutorialText()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        isFading = true;

        // フェードアウト処理
        float startAlpha = tutorialCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < FADE_DURATION)
        {
            elapsedTime += Time.deltaTime;
            tutorialCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsedTime / FADE_DURATION);
            yield return null;
        }

        tutorialCanvasGroup.alpha = 0;
        isFading = false;

    }
}
