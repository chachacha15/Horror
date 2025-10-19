using NUnit.Framework.Interfaces;
using UnityEngine;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    #region variables



    // 他クラス
    private ElevatorManager elevatorManager;
    private GameStateManager gameStateManager;
    private MonologueManager monologueManager;

    #endregion


    #region Interactable (IInteractable)

    public string GetInteractText()
    {
        return "";
    }

    public bool ShowInteractText => false; // テキスト表示するかどうか
    public bool ActivateCrosshair => !elevatorManager.IsMovingDoor; // クロスヘアをアニメーションするかどうか

    /// <summary>
    /// クリックしたら選択画面UIを表示
    /// </summary>
    public void Interact(GameObject targetObject)
    {
        if (gameStateManager.IsElectricSystemON)
        {


            if (elevatorManager.IsMovingDoor) return;
            elevatorManager.IsMovingDoor = true;
            if (elevatorManager.IsOpen) elevatorManager.OnClose();
            else elevatorManager.OnOpen();
            elevatorManager.IsOpen = !elevatorManager.IsOpen;
            StartCoroutine(elevatorManager.WaitStoppingAnimation(elevatorManager.IsOpen));
        }
        else
        {
            monologueManager.TrySettingLog(MonologueType.FindElevator);
        }
    }

    #endregion


    #region Methods


    private void Start()
    {
        // 他クラスを取得
        elevatorManager = ElevatorManager.Instance;
        gameStateManager = GameStateManager.Instance;
        monologueManager = MonologueManager.Instance;
    }

    



    #endregion
}
