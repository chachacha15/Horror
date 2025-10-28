using UnityEngine;

public class FoundByEnemyFirstTime : MonoBehaviour
{
    [SerializeField] private GameObject enemyObjectInEvent; // イベントで使用する敵オブジェクト
    private GhostAI ghostAI;


    private void Start()
    {
        if (enemyObjectInEvent != null)
        {
            ghostAI = enemyObjectInEvent.GetComponent<GhostAI>();
            ghostAI.enabled = false; // 初期状態でGhostAIを無効にする
            enemyObjectInEvent.SetActive(false); // 初期状態で敵オブジェクトを非アクティブにする
        }
    }


    public void ActivateEnemy()
    {
        if (enemyObjectInEvent != null)
        {
            enemyObjectInEvent.SetActive(true); // 敵オブジェクトをアクティブにする
            Time.timeScale = 1f; // 時間を通常に戻す
        }
       
    }

    public void OnFoundPlayer()
    {
        ghostAI.enabled = true; // GhostAIを有効にする
        ghostAI.isPlayerVisible = true; // 敵の状態をChaseに設定
    }
}
