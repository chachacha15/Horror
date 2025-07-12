using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MonologueManager : MonoBehaviour
{
    #region Variables

    // シングルトン
    public static MonologueManager Instance;


    // インスペクターから設定する GameObject ・ Component ・ value 等
    public MonologueData[] monologue; // 出力する発言やログ
    public TextMeshProUGUI monologueText;  // セリフテキストUI
    public GameObject monologuePanel;      // ログを表示するパネル
    public GameObject nextIndicator;       // 続きを促すマーク
    public TextMeshProUGUI objectiveText;  // 目標を表示するテキストUI
    public float textSpeed = 0.05f;        // 文字を1つずつ表示する速度



    // 保持のための変数
    private Queue<string> monologueQueue = new Queue<string>(); // 会話キュー
    private string currentMonologueSentence;          // 現在表示中のセリフを保持
    private MonologueData currentActiveMonologueData; // 現在表示中のMonologueDataを追跡
    private Coroutine currentCoroutine;                   // 現在のセリフコルーチン
    private Coroutine currentIndicatorAnimationCoroutine; // アニメーションコルーチンを管理する変数


    // フラグ等
    private bool isTyping = false;           // 文字を表示中かどうか
    private bool isWaitingNextLogue = false; // 次のセリフを表示してもいいかどうか
    private bool isDisplaying = false; // セリフを表示中かどうか
    private bool isPlayedStartTimeLine = false; // 初登場時のアニメーションを起動したか
    public bool isWaitingGetFlashlight = false; // フラッシュライトを入手 待ちのときON
    public bool GotFlashLight = false;          // フラッシュライトを入手 済みでディスプレイがアクティブのときON
    public bool isWaitingReachElevator = false; // エレベーターに初到着 待ちのときON
    public bool isWaitingGetElevatorKey = false; // エレベーター起動アイテムを入手 待ちのときON



    public Animator playerAnimator;
    public PlayableDirector playerPD;

    

    // 他クラス
    public PlayerMove playerMove;
    public PlayerLook playerLook;
    private ItemDisplay itemDisplay;

    #endregion Variables




    #region Methods


    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        // 他クラスを取得
        playerMove = PlayerMove.Instance;
        playerLook = PlayerLook.Instance;
        itemDisplay = ItemDisplay.Instance;

        monologuePanel.SetActive(false); // 最初は非表示


    }

    /// <summary>
    /// クリックで次のセリフへ
    /// </summary>
    private void Update()
    {

        if (isDisplaying && Input.GetMouseButtonDown(0) && !itemDisplay.isItemDisplayON)
        {
            if (isTyping)
            {
                // 文字表示中にクリックしたら、全表示
                StopCoroutine(currentCoroutine);
                monologueText.text = currentMonologueSentence; // 今のセリフを全部表示
                isTyping = false;

                if (nextIndicator != null)
                {
                    nextIndicator.SetActive(true);
                    // 既存のアニメーションコルーチンが動いていれば停止し、新しく開始
                    if (currentIndicatorAnimationCoroutine != null)
                    {
                        StopCoroutine(currentIndicatorAnimationCoroutine);
                    }
                    currentIndicatorAnimationCoroutine = StartCoroutine(DoAnimateNextIndicator());
                }
            }
            else if (!isTyping)
            {
                StartCoroutine(WaitSecondsShowNextMonologue(0.1f));

            }

            
        }

        // アイテム表示中ではない、かつセリフが表示中の場合のみ処理を行う
        if (isDisplaying && !itemDisplay.isItemDisplayON)
        {
            
            // Lキーの処理（会話全体をスキップ）
            if (Input.GetKeyDown(KeyCode.L))
            {
                SkipAllMonologue();
            }
        }


    }
    /// <summary>
    /// Lキーによる会話全体スキップのロジック
    /// </summary>
    private void SkipAllMonologue()
    {
        // 現在のモノローグキューをクリア
        monologueQueue.Clear();

        // 実行中の文字表示コルーチンがあれば停止
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // 終了処理を呼び出す
        // ShowNextMonologue() はキューが空だと終了処理を実行するため、これを呼び出す
        ShowNextMonologue();
    }


    /// <summary>
    /// 会話リストを設定
    /// </summary>
    public void SetMonologues(MonologueData monologueData)
    {
        monologueQueue.Clear(); // 以前のデータをクリア
        foreach (var line in monologueData.monologueLines)
        {
            monologueQueue.Enqueue(line); // 追加
        }
        currentActiveMonologueData = monologueData;

    }

    /// <summary>
    /// 次のセリフを表示
    /// </summary>
    public void ShowNextMonologue()
    {
        if (monologueQueue.Count == 0)
        {
            monologuePanel.SetActive(false);
            isDisplaying = false;
            Time.timeScale = 1.0f;


            // currentActiveMonologueData が monologue[1] と同じオブジェクトの場合にのみ実行
            if (currentActiveMonologueData == monologue[1])
            {
                objectiveText.text = "〇 周囲を探索する";
            }

            if (!isPlayedStartTimeLine)
            {
                isPlayedStartTimeLine = true;
                playerPD.Play();
            }

            return;
        }

        isDisplaying = true;
        monologuePanel.SetActive(true);

        if (monologueQueue.Count > 0) // 連打時のエラー回避
        {
            // Dequeueしたセリフを currentMonologueSentence にも保持する
            currentMonologueSentence = monologueQueue.Dequeue();

            // 文字を1つずつ表示
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            if (!isTyping) currentCoroutine = StartCoroutine(TypeSentence(currentMonologueSentence));
        }
    }


    /// <summary>
    /// 1文字ずつ表示する
    /// </summary>
    private IEnumerator TypeSentence(string sentence)
    {
       
        isTyping = true;
        monologueText.text = "";
        // タイプ開始時にマークを非表示にする
        if (nextIndicator != null) nextIndicator.SetActive(false);

        foreach (char letter in sentence.ToCharArray())
        {
            monologueText.text += letter;
            yield return new WaitForSecondsRealtime(textSpeed);
        }

        isTyping = false;

        // 文字表示完了後、次のセリフがある場合にマークを表示
        if (nextIndicator != null)
        {
            nextIndicator.SetActive(true);
            // マークの移動アニメーションを開始
            StartCoroutine(AnimateNextIndicator());
        }
    }









    /// <summary>
    /// タイムラインから参照する。プレイヤーの操作を無効にするメソッド
    /// </summary>
    public void DisablePlayerControl()
    {
        playerMove.enabled = false;
        playerLook.enabled = false;
        EmissionLooper[] allEmissionLoopers = FindObjectsOfType<EmissionLooper>();
        foreach (EmissionLooper emissionLooper in allEmissionLoopers) emissionLooper.enabled = false;
    }

    /// <summary>
    /// タイムラインから参照する。プレイヤーの操作を有効にするメソッド
    /// </summary>
    public void EnablePlayerControl()
    {
        playerAnimator.enabled = false;

        playerMove.enabled = true;
        playerLook.enabled = true;

        EmissionLooper[] allEmissionLoopers = FindObjectsOfType<EmissionLooper>();
        foreach (EmissionLooper emissionLooper in allEmissionLoopers) emissionLooper.enabled = true;

        isWaitingGetFlashlight = true;
    }


    public void ActivateWaitingReachElevator()
    {
        isWaitingGetFlashlight = false;
        isWaitingReachElevator = true;
        
        SetMonologues(monologue[1]);
        ShowNextMonologue();
        
    }

    /// <summary>
    /// 次のセリフへ移る際に、少し猶予時間を作る
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator WaitSecondsShowNextMonologue(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        // 次のセリフへ
        ShowNextMonologue();
    }




    /// <summary>
    /// 次へ進むマークを反復移動させるコルーチン
    /// </summary>
    private IEnumerator AnimateNextIndicator()
    {
        // 既にアニメーションが実行中なら停止
        if (currentIndicatorAnimationCoroutine != null)
        {
            StopCoroutine(currentIndicatorAnimationCoroutine);
        }

        // このコルーチン自身をcurrentIndicatorAnimationCoroutineに代入
        currentIndicatorAnimationCoroutine = StartCoroutine(DoAnimateNextIndicator());
        yield return currentIndicatorAnimationCoroutine; // このコルーチンが終わるまで待つ（実質、無限ループなのでここは実行されない）
    }

    private IEnumerator DoAnimateNextIndicator()
    {
        Vector3 startPos = new Vector3(0, -485, 0);
        Vector3 endPos = new Vector3(0, -500, 0);
        float animationSpeed = 2.0f; // 移動速度を調整

        while (true) // 無限ループで繰り返す
        {
            // y: -485 -> -500 へ移動
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.unscaledDeltaTime * animationSpeed; // Time.unscaledDeltaTime を使用 (Time.timeScaleに影響されない)
                nextIndicator.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(startPos, endPos, timer);
                yield return null; // 1フレーム待つ
            }

            // y: -500 -> -485 へ移動
            timer = 0f;
            while (timer < 1f)
            {
                timer += Time.unscaledDeltaTime * animationSpeed;
                nextIndicator.GetComponent<RectTransform>().anchoredPosition3D = Vector3.Lerp(endPos, startPos, timer);
                yield return null; // 1フレーム待つ
            }
        }
    }


    #endregion Methods



}
