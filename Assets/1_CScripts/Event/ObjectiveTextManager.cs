using TMPro;
using UnityEngine;

public class ObjectiveTextManager : MonoBehaviour
{
    public static ObjectiveTextManager Instance;

    [SerializeField] private ObjectiveTextData[] objectiveTextDataList;  // 目標テキストデータのリスト
    [SerializeField] private TextMeshProUGUI objectiveText;  // 目標を表示するテキストUI


    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 現在の目標テキストを更新する
    /// </summary>
    /// <param name="newText"></param>
    private void UpdateObjectiveText(string newText)
    {
        objectiveText.text = newText;
    }


    /// <summary>
    /// 指定されたタイプの目標テキストに更新する
    /// </summary>
    /// <param name="type"></param>
    public void SetObjective(ObjectiveDataType type)
    {
        foreach (var data in objectiveTextDataList)
        {
            if (data.Type == type)
            {
                UpdateObjectiveText(data.Text);
                break;
            }
        }
    }

}
