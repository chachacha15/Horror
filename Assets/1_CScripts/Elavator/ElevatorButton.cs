using UnityEngine;

public class ElevatorButton : MonoBehaviour, IInteractable
{
    #region variables



    // 他クラス
    private ElevatorManager elevatorManager;

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
        if (elevatorManager.IsMovingDoor) return;
        elevatorManager.IsMovingDoor = true;
        if (elevatorManager.IsOpen) elevatorManager.OnClose();
        else elevatorManager.OnOpen();
        elevatorManager.IsOpen = !elevatorManager.IsOpen;
        StartCoroutine(elevatorManager.WaitStoppingAnimation(elevatorManager.IsOpen));

    }

    #endregion


    #region Methods


    private void Start()
    {
        // 他クラスを取得
        elevatorManager = ElevatorManager.Instance;
    }

    



    #endregion
}
