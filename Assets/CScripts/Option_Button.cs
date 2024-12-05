using UnityEngine;

public class Option_Button : MonoBehaviour
{
    public GameObject optionPrefab; // Option Prefabを指定する
    private GameObject instantiatedOption; // インスタンス化したOptionの参照

    public void OnOption_ButtonClick()
    {
        if (instantiatedOption == null)
        {
            // Option Prefabをインスタンス化
            instantiatedOption = Instantiate(optionPrefab, transform);

            // 初期位置調整（必要なら）
            RectTransform rectTransform = instantiatedOption.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero; // Canvas中央に配置
            }
        }
        else
        {
            // 表示/非表示を切り替え
            instantiatedOption.SetActive(!instantiatedOption.activeSelf);
        }
    }
}