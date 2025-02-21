using UnityEngine;
using UnityEngine.UI;

public class Option_Button : MonoBehaviour
{
    public Button optionButton; // ボタン参照
    public Text optionText;     // テキスト参照

    void Start()
    {
        // ボタンのクリックイベントを登録
        optionButton.onClick.AddListener(ToggleOptionText);

        // 初期状態ではテキストを非表示にする
        optionText.gameObject.SetActive(false);
    }

    void ToggleOptionText()
    {
        // テキストの表示/非表示を切り替え
        optionText.gameObject.SetActive(!optionText.gameObject.activeSelf);
    }
}
