using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false; // 一時停止中かどうかを追跡

    void Update()
    {
        // Pキーを押したときに処理を切り替える
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame(); // ゲームを再開
            }
            else
            {
                PauseGame(); // ゲームを一時停止
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // 時間を停止
        isPaused = true;
        Debug.Log("ゲームが一時停止しました");
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // 時間を再開
        isPaused = false;
        Debug.Log("ゲームが再開しました");
    }
}
