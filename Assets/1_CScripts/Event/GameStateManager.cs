using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GameStateManager : MonoBehaviour
{
    #region Variables
    public static GameStateManager Instance;

    // タイムライン
    public TimelineAsset startTimeline; // ゲーム開始時のタイムライン
    [SerializeField] private TimelineAsset foundByEnemyFirstTime; // 初めて敵に見つかる演出のタイムライン

    // フラグ
    public bool IsElectricSystemON = false;

    #endregion



    #region Methods

    private void Awake()
    {
        Instance = this;
    }


    /// <summary>
    /// ゲームイベントを発動するメソッド
    /// </summary>
    /// <param name="gameEvent"></param>
    public void TriggerGameEvent(GameEvent gameEvent)
    {

        switch (gameEvent)
        {
            case GameEvent.None:
                // 何もしない
                break;
            case GameEvent.FoundByEnemyFirstTime:
                Debug.Log("Player found by enemy!");
                PlayerMove.Instance.gameObject.GetComponent<PlayableDirector>().playableAsset = foundByEnemyFirstTime;
                MonologueManager.Instance.playerPD.Play();
                break;
            case GameEvent.FindElevator:
                // エレベーターを見つけたときの処理
                Debug.Log("Player found the elevator!");
                break;
            case GameEvent.ReachRoof:
                // 屋上に到達したときの処理
                Debug.Log("Player reached the roof!");
                break;
            default:
                Debug.Log("No valid game event triggered.");
                break;
        }
    }

    #endregion


}


public enum GameEvent
{
    None,
    FoundByEnemyFirstTime,
    FindElevator,
    ReachRoof,
}
