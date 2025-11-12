using UnityEngine;
using System.Collections;
using TMPro;

public class HidingUnderDesk : MonoBehaviour
{

    #region Constants

    private const float WAIT_TIME_AFTER_HIDING = 5.0f; // 机の下に隠れた後の待機時間

    #endregion

    #region Variables

    public static HidingUnderDesk Instance;

    [SerializeField] private EmissionLooper emissionLooper; // 机のEmissionLooperコンポーネントへの参照
    [SerializeField] private GameObject tutorialTextToLeaveDesk; // 机の下から出るためのチュートリアルテキスト

    // 他クラス
    private GameStateManager gameStateManager;

    #endregion

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        gameStateManager = GameStateManager.Instance;

    }


    public void OnTimelineStart()
    {

    }
    public void OnTimelineEnd()
    {

    }

    public IEnumerator ActivateHidingEvent()
    {

        CameraSwitcher.Instance.CanControl = false;

        emissionLooper.enabled = false;
        gameStateManager.DisablePlayerControl();
        yield return StartCoroutine(gameStateManager.WaitForSeconds(WAIT_TIME_AFTER_HIDING));
        MonologueManager.Instance.TrySettingLog(MonologueType.HidingUnderDesk);

    }

    
}
