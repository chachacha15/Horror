using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Text用
using TMPro; //TextMeshPro用


public class GameOverScript : MonoBehaviour
{
    public GameObject gameOverUI; // GameOver UIを紐づけ
    public TextMeshProUGUI gameOverText; // GameOver Textオブジェクト
    private Color textColor;

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

        // GameOverTextを探す
        if (gameOverUI != null)
        {
            gameOverText = gameOverUI.GetComponentInChildren<TextMeshProUGUI>();
            textColor = gameOverText.color;

        }

        // エラーログの確認
        if (gameOverUI == null)
        {
            Debug.LogError("GameOver UI (Canvas) が見つかりませんでした。");
        }
        if (gameOverText == null)
        {
            Debug.LogError("GameOver Text が見つかりませんでした。");
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
            StartCoroutine(FadeTextAlpha()); // ガンマ値（アルファ値）をフェード
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

    private IEnumerator FadeTextAlpha()
    {
        if (gameOverText == null)
            yield break;

        float alpha = 0; // 初期アルファ値
        bool increasing = true; // アルファ値を増加させるか減少させるか

        while (true)
        {
            // アルファ値の増減
            alpha += (increasing ? 1f : -1f) * Time.deltaTime * 2f; // 調整可能な速度

            // アルファ値の範囲を制限
            if (alpha > 1f)
            {
                alpha = 1f;
                increasing = false; // 減少に切り替え
            }
            else if (alpha < 0f)
            {
                alpha = 0f;
                increasing = true; // 増加に切り替え
            }

            // テキストのカラーを更新
            textColor.a = alpha; // アルファ値を更新
            gameOverText.color = textColor;

            yield return null; // 次のフレームまで待機
        }
    }
}
