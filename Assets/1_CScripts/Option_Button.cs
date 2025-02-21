using UnityEngine;
using UnityEngine.UI;

public class Option_Button : MonoBehaviour
{
    public Button optionButton; // �{�^���Q��
    public Text optionText;     // �e�L�X�g�Q��

    void Start()
    {
        // �{�^���̃N���b�N�C�x���g��o�^
        optionButton.onClick.AddListener(ToggleOptionText);

        // ������Ԃł̓e�L�X�g���\���ɂ���
        optionText.gameObject.SetActive(false);
    }

    void ToggleOptionText()
    {
        // �e�L�X�g�̕\��/��\����؂�ւ�
        optionText.gameObject.SetActive(!optionText.gameObject.activeSelf);
    }
}
