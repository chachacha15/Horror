using UnityEngine;


[CreateAssetMenu(fileName = "NewTutorialData", menuName = "New Data/New TutorialData")]

public class TutorialTextData : ScriptableObject
{
    [TextArea(3, 5)]
    public string Text;
    public bool IsTimeBasedDisappear; // 時間制御で消えるチュートリアルかどうか

    public TutorialDataType Type;
}

public enum TutorialDataType
{
    None,

    Flashlight,
    LeaveDesk,
}
