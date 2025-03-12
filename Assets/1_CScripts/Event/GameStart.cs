using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;

public class GameStart : MonoBehaviour
{

    public TextMeshProUGUI startLogText; // UIテキスト
    public GameObject startLogCanvas; // ログを表示するパネル

    public float textSpeed = 0.05f;  // 文字を1つずつ表示する速度

    private Queue<string> monologueQueue = new Queue<string>(); // 会話キュー
    private Coroutine currentCoroutine;  // 現在のコルーチン

    private bool isTyping = false; // 文字を表示中かどうか
    private bool isDisplaying = false; // セリフを表示中かどうか

    public MonologueData startMonologue; // ゲーム開始時の状況説明
    public bool hasFinishedStartTex = false; // ゲーム開始前の文章が表示し終えたか
    public bool hasStartedGame = false; // 状況説明をしゃべり終え、操作可能になったか



    private MonologueManager monologueManager;

    void Start()
    {
        monologueManager = FindObjectOfType<MonologueManager>();
        StartGameMonologue(startMonologue);
        Time.timeScale = 0f;
    }


    /// <summary>
    /// 最初の状況説明をしゃべる（ゲーム開始時に1回だけ）
    /// </summary>
    public void StartGameMonologue(MonologueData startMonologue)
    {
        if (hasStartedGame) return;

        SetStartSentence(startMonologue);
        ShowNextStartSentence();


    }


    /// <summary>
    /// クリックで次のセリフへ
    /// </summary>
    private void Update()
    {
        if (isDisplaying && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // 文字表示中にクリックしたら、全表示
                StopCoroutine(currentCoroutine);
                if (monologueQueue.Count > 0) startLogText.text = monologueQueue.Peek(); // 今のセリフを全部表示
                isTyping = false;
            }
            else
            {
                // 次のセリフへ
                ShowNextStartSentence();
            }
        }
    }



    /// <summary>
    /// 会話リストを設定
    /// </summary>
    public void SetStartSentence(MonologueData monologueData)
    {
        monologueQueue.Clear(); // 以前のデータをクリア
        foreach (var line in monologueData.monologueLines)
        {
            monologueQueue.Enqueue(line); // 追加
        }
    }

    /// <summary>
    /// 次のセリフを表示
    /// </summary>
    public void ShowNextStartSentence()
    {
        if (monologueQueue.Count == 0)
        {
            startLogCanvas.SetActive(false);
            isDisplaying = false;
            hasFinishedStartTex = true;
            Time.timeScale = 0f;

           

            // ここで開始地点で少ししゃべるようにする（まだゲーム始めない）
            monologueManager.SetMonologues(monologueManager.monologue[0]);
            monologueManager.ShowNextMonologue();


            return;
        }

        isDisplaying = true;
        startLogCanvas.SetActive(true);

        if (monologueQueue.Count > 0) // 連打時のエラー回避
        {
            string nextMonologue = monologueQueue.Dequeue();

            // 文字を1つずつ表示
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(TypeStartSentence(nextMonologue));
        }
    }


    /// <summary>
    /// 1文字ずつ表示する
    /// </summary>
    private IEnumerator TypeStartSentence(string sentence)
    {
        isTyping = true;
        startLogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            startLogText.text += letter;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
        isTyping = false;
    }


}
