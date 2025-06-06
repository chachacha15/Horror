using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine.Playables;

public class MonologueManager : MonoBehaviour
{


    public MonologueData[] monologue; // 出力する発言やログ


    public TextMeshProUGUI monologueText;  // UIテキスト
    public GameObject monologuePanel;  // ログを表示するパネル
    public float textSpeed = 0.05f;  // 文字を1つずつ表示する速度

    private Queue<string> monologueQueue = new Queue<string>(); // 会話キュー
    private Coroutine currentCoroutine;  // 現在のコルーチン

    private bool isTyping = false; // 文字を表示中かどうか
    private bool isDisplaying = false; // セリフを表示中かどうか


    public Animator playerAnimator;
    public PlayableDirector playerPD;

    // 他クラス
    public PlayerMove playerMove;
    public PlayerLook playerLook;

    private void Start()
    {
        // 他クラスを取得
        playerMove = PlayerMove.Instance;
        playerLook = PlayerLook.Instance;

        monologuePanel.SetActive(false); // 最初は非表示


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
                if (monologueQueue.Count > 0) monologueText.text = monologueQueue.Peek(); // 今のセリフを全部表示
                    isTyping = false;
            }
            else
            {
                // 次のセリフへ
                ShowNextMonologue();
            }
        }
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

            playerPD.Play();


            return;
        }

        isDisplaying = true;
        monologuePanel.SetActive(true);

        if (monologueQueue.Count > 0) // 連打時のエラー回避
        {
            string nextMonologue = monologueQueue.Dequeue();

            // 文字を1つずつ表示
            if (currentCoroutine != null) StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(TypeSentence(nextMonologue));
        }
    }


    /// <summary>
    /// 1文字ずつ表示する
    /// </summary>
    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        monologueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            monologueText.text += letter;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
        isTyping = false;
    }










    public void DisablePlayerControl()
    {
        playerMove.enabled = false;
        playerLook.enabled = false;
        EmissionLooper[] allEmissionLoopers = FindObjectsOfType<EmissionLooper>();
        foreach (EmissionLooper emissionLooper in allEmissionLoopers) emissionLooper.enabled = false;
    }

    public void EnablePlayerControl()
    {
        playerAnimator.enabled = false;

        playerMove.enabled = true;
        playerLook.enabled = true;

        EmissionLooper[] allEmissionLoopers = FindObjectsOfType<EmissionLooper>();
        foreach (EmissionLooper emissionLooper in allEmissionLoopers) emissionLooper.enabled = true;
    }






}
