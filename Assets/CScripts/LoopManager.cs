using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor.EditorTools;

public class LoopManager : MonoBehaviour
{
    public float initialLoopDuration = 60f; // 初期ループ時間
    private float remainingTime; // 現在のループの残り時間
    public int loopCount = 0; // 現在のループ回数

    public Text timerText; // UIのTextオブジェクト

    private List<string> carriedItems = new List<string>(); // ループ間で引き継ぐアイテム

    private void Awake()
    {
        // ループマネージャをシーン間で保持
        if (FindObjectsOfType<LoopManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        remainingTime = initialLoopDuration; // 初期時間を設定
    }

    private void Update()
    {
        // 時間を進める
        remainingTime -= Time.deltaTime;

        // 0キーで残り時間を5秒短縮
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            remainingTime -= 5f;
            if (remainingTime < 0) remainingTime = 0; // マイナスにならないよう制限
        }

        // 時間が0以下になったらループを開始
        if (remainingTime <= 0)
        {
            StartNewLoop();
        }

        // 残り時間をTextに反映
        timerText.text = $"残り時間: {GetRemainingTime():F1} 秒　(0キーで５秒短縮)";

    }

    private void StartNewLoop()
    {
        // ループ回数を増加
        loopCount++;
        Debug.Log($"ループ開始: {loopCount}回目");

        // 現在のシーンを再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // 残り時間をリセット
        remainingTime = initialLoopDuration;
    }

    // アイテムを追加
    public void AddItem(string itemName)
    {
        if (!carriedItems.Contains(itemName))
            carriedItems.Add(itemName);
    }

    // アイテムを保持しているか確認
    public bool HasItem(string itemName)
    {
        return carriedItems.Contains(itemName);
    }

    // ループ回数を取得
    public int GetLoopCount()
    {
        return loopCount;
    }

    // 現在の残り時間を取得
    public float GetRemainingTime()
    {
        return remainingTime;
    }
}
