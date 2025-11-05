using UnityEngine;


[CreateAssetMenu(fileName = "NewObjectiveData", menuName = "New Data/NewObjectiveData")]
public class ObjectiveTextData : ScriptableObject
{
    [TextArea(3, 5)]
    public string Text; // 目標テキスト
    public ObjectiveDataType Type; // 目標テキストデータの種類
}

public enum ObjectiveDataType
{
    None,
    FindFlashlight,
    FindElevator,
    FindElectricSystem,
    HideUnderDesk,
    ExploreTheFloor,
}