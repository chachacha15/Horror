using UnityEngine;
using TMPro; // TextMeshPro用
using UnityEngine.EventSystems; // EventSystem用

public class ChangeTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText; // ボタンのTextMeshProUGUI
    public Color normalColor = Color.black; // 通常時の色
    public Color hoverColor = Color.red; // マウスオーバー時の色

    void Start()
    {
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // 初期色を設定
        buttonText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // マウスがボタンに入ったときの色変更
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // マウスがボタンから離れたときの色変更
        buttonText.color = normalColor;
    }
}
