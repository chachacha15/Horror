using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public GameObject gameOverUI; // GameOver UIを紐づけ

    private void Start()
    {
        // このオブジェクトの最初の子オブジェクトを取得し、Canvasかどうかを確認して設定
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Canvas>() != null) // 子オブジェクトがCanvasを持っているか確認
            {
                gameOverUI = child.gameObject; // 該当オブジェクトをgameOverUIに設定
                break;
            }
        }

        // 念のため、gameOverUIが設定されていない場合にエラーログを出す
        if (gameOverUI == null)
        {
            Debug.LogError("GameOver UI (Canvas) が見つかりませんでした。");
        }
    }

    private void Update()
    {
        // スペースキーが押されたらRestartGameを呼び出す
        if (gameOverUI != null && gameOverUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    public void TriggerGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // GameOver UIを表示
            //Time.timeScale = 0f; // ゲームを一時停止
        }
        else
        {
            Debug.LogError("GameOver UI が設定されていません。");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // 時間を再開
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 現在のシーンを再読み込み
    }
}
