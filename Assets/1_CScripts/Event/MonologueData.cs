using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonologue", menuName = "Monologue System/Monologue Data")]
public class MonologueData : ScriptableObject
{
    [TextArea(3, 5)]

    public string[] monologueLines; // 会話リスト

    public MonologueType monologueType; // 会話の種類

    // このログが表示されるために「先に完了している必要があるログ」のリスト
    public List<MonologueData> Prerequisites;

    public GameEvent EventToActivation; // このログが完了したときに発火するイベント
}


public enum MonologueType
{
    None,
    WakeUp,
    GetFlashlight,
    FindElevator,
    FindElectricSystem,
}
