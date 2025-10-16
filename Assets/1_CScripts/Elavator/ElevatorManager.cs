using System.Collections;
using UnityEditor;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{


    #region Variables

    public static ElevatorManager Instance { get; private set; }


    public bool IsMovingDoor = false; // エレベータードア開閉動作中か
    public bool IsOpen = false; // エレベータードア開いた状態か

    // アニメーション
    [SerializeField] private Animator rightDoorAnimator;
    [SerializeField] private Animator leftDoorAnimator;
    private string openTrigger = "Open";
    private string closeTrigger = "Close";



    #endregion



    private void Awake()
    {
        Instance = this;
    }



    /// <summary>
    /// 
    /// </summary>
    public void OnOpen()
    {

        rightDoorAnimator.SetTrigger(openTrigger);
        leftDoorAnimator.SetTrigger(openTrigger);
    }


    /// <summary>
    /// 
    /// </summary>
    public void OnClose() 
    {
        rightDoorAnimator.SetTrigger(closeTrigger);
        leftDoorAnimator.SetTrigger(closeTrigger);
    }
    


    public IEnumerator WaitStoppingAnimation(bool isOpen)
    {
        string stateName = isOpen ? "ElevatorRightDoorOpen" : "ElevatorRightDoorClose";
        yield return new WaitUntil(() => {
            AnimatorStateInfo stateInfo = rightDoorAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(stateName) && stateInfo.normalizedTime >= 1.0f;
        }); IsMovingDoor = false;
    }

}
