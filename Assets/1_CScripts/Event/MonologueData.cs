using UnityEngine;

[CreateAssetMenu(fileName = "NewMonologue", menuName = "Monologue System/Monologue Data")]
public class MonologueData : ScriptableObject
{
    [TextArea(3, 5)]

    public string[] monologueLines; // ‰ï˜bƒŠƒXƒg
}
