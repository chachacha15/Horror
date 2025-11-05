using TMPro;
using UnityEngine;

public class FoundByEnemyFirstTime : MonoBehaviour
{
    [SerializeField] private GameObject enemyObjectInEvent; // イベントで使用する敵オブジェクト
    [SerializeField] private EmissionLooper emissionLooper; // 机のEmissionLooperコンポーネントへの参照
    [SerializeField] private TextMeshProUGUI objectiveText;  // 目標を表示するテキストUI
    private GhostAI ghostAI;


    private void Start()
    {
        if (enemyObjectInEvent != null)
        {
            ghostAI = enemyObjectInEvent.GetComponent<GhostAI>();
            ghostAI.enabled = false; // 初期状態でGhostAIを無効にする
            enemyObjectInEvent.SetActive(false); // 初期状態で敵オブジェクトを非アクティブにする
            emissionLooper.enabled = false; // 机のエミッションルーパーを無効にする
        }
    }

    /// <summary>
    /// タイムラインから参照する。敵に発見された時のタイムライン開始・終了時の処理
    /// </summary>
    public void OnFoundByEnemyTimelineStart()
    {

    }
    public void OnFoundByEnemyTimelineEnd()
    {
        emissionLooper.enabled = true; // 机のエミッションルーパーを有効にする
        ObjectiveTextManager.Instance.SetObjective(ObjectiveDataType.HideUnderDesk); // 目標テキストを更新
        GameStateManager.Instance.HasMetEnemy = true; // 敵に初めて見つかったフラグを立てる

        ghostAI.enabled = true; // GhostAIを有効にする
        ghostAI.isPlayerVisible = true; // 敵の状態をChaseに設定

    }




    public void ActivateEnemy()
    {
        if (enemyObjectInEvent != null)
        {
            enemyObjectInEvent.SetActive(true); // 敵オブジェクトをアクティブにする
            Time.timeScale = 1f; // 時間を通常に戻す
        }
       
    }

    
}
