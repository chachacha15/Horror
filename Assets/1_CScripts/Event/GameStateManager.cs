using System.Collections;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class GameStateManager : MonoBehaviour
{
    


    #region Variables
    public static GameStateManager Instance;

    // タイムライン
    public TimelineAsset startTimeline; // ゲーム開始時のタイムライン
    [SerializeField] private TimelineAsset foundByEnemyFirstTime; // 初めて敵に見つかる演出のタイムライン
    [SerializeField] private TimelineAsset hideUnderDeskTimeline; // 机の下に隠れる演出のタイムライン

    // フラグ
    public bool HasMetEnemy = false;
    public bool IsElectricSystemON = false;

    // 他クラス
    private PlayerMove playerMove;
    private PlayerLook playerLook;
    private PlayerInteractor playerInteractor;

    #endregion



    #region Methods

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerMove = PlayerMove.Instance;
        playerLook = PlayerLook.Instance;
        playerInteractor = PlayerInteractor.Instance;
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
            case GameEvent.OnFoundFlashlight:
                // 懐中電灯を見つけたときの処理
                StartCoroutine(TutorialManager.Instance.ShowTutorial(TutorialDataType.Flashlight));
                ObjectiveTextManager.Instance.SetObjective(ObjectiveDataType.FindElevator);

                break;
            case GameEvent.FoundElevator:
                // エレベーターを見つけたときの処理
                ObjectiveTextManager.Instance.SetObjective(ObjectiveDataType.FindElectricSystem);
                break;
            case GameEvent.FoundByEnemyFirstTime:
                // 初めて敵に見つかったときの処理
                PlayerMove.Instance.gameObject.GetComponent<PlayableDirector>().playableAsset = foundByEnemyFirstTime;
                MonologueManager.Instance.playerPD.Play();
                break;
            case GameEvent.HidingUnderDesk:
                // 机の下に隠れ、出ることができるときの処理
                StartCoroutine(TutorialManager.Instance.ShowTutorial(TutorialDataType.LeaveDesk));
                ObjectiveTextManager.Instance.SetObjective(ObjectiveDataType.ExploreTheFloor);
                CameraSwitcher.Instance.CanControl = true;
                EnablePlayerControl();

                break;
            default:
                Debug.Log("No valid game event triggered.");
                break;
        }
    }

    #endregion


    




    /// <summary>
    /// タイムラインから参照する。プレイヤーの操作を無効にするメソッド
    /// </summary>
    public void DisablePlayerControl()
    {

        playerMove.enabled = false;
        playerLook.IsCameraLocked = true;
        playerInteractor.CanInteract = false;

        playerMove.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);


    }

    /// <summary>
    /// タイムラインから参照する。プレイヤーの操作を有効にするメソッド
    /// </summary>
    public void EnablePlayerControl()
    {

        playerMove.enabled = true;
        playerLook.IsCameraLocked = false;
        playerInteractor.CanInteract = true;

    }


    public IEnumerator WaitForSeconds(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
    }

}


public enum GameEvent
{
    None,
    OnFoundFlashlight,
    FoundByEnemyFirstTime,
    FoundElevator,
    HidingUnderDesk,

}
