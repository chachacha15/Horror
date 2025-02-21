using UnityEngine;
using TMPro; // TextMeshPro�p
using UnityEngine.EventSystems; // EventSystem�p

public class ChangeTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText; // �{�^����TextMeshProUGUI
    public Color normalColor = Color.black; // �ʏ펞�̐F
    public Color hoverColor = Color.red; // �}�E�X�I�[�o�[���̐F

    void Start()
    {
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // �����F��ݒ�
        buttonText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // �}�E�X���{�^���ɓ������Ƃ��̐F�ύX
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // �}�E�X���{�^�����痣�ꂽ�Ƃ��̐F�ύX
        buttonText.color = normalColor;
    }
}
