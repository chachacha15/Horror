using UnityEngine;
using UnityEngine.UI;

public class Telephone : MonoBehaviour
{
    public GameObject phoneUI; // �d�b��UI
    public UnityEngine.UI.Text displayText; // ���b�Z�[�W�\���p�e�L�X�g
    public UnityEngine.UI.Text inputText; // ���͂����ԍ��\���p�e�L�X�g

    private string phoneNumber = "";
    private bool isCalling = false;

    void Start()
    {
        if (phoneUI != null)
        {
            phoneUI.SetActive(false);
        }
    }

    public void OpenPhone()
    {
        if (phoneUI != null)
        {
            phoneUI.SetActive(true);
            ResetPhoneState();
        }
    }

    public void ClosePhone()
    {
        if (phoneUI != null)
        {
            phoneUI.SetActive(false);
        }
    }

    public void InputNumber(string number)
    {
        if (phoneNumber.Length < 11) // 11���܂œ��͉\
        {
            phoneNumber += number;
            if (inputText != null)
            {
                inputText.text = phoneNumber;
            }
        }
    }

    public void Call()
    {
        if (phoneNumber.Length > 0 && !isCalling)
        {
            isCalling = true;
            if (displayText != null)
            {
                displayText.text = "���M��...";
            }
            Invoke(nameof(ReceiveCall), 2f); // 2�b��ɉ���
        }
    }

    private void ReceiveCall()
    {
        if (displayText != null)
        {
            displayText.text = phoneNumber switch
            {
                "123" => "������...�����͈Â�...",
                "666" => "���O�͌��Ă͂����Ȃ����̂�����",
                "999" => "����͌����ł͂Ȃ�...",
                _ => "�c�[...�c�[...�c�[..."
            };
        }
        isCalling = false;
    }

    public void HangUp()
    {
        ResetPhoneState();
    }

    private void ResetPhoneState()
    {
        if (displayText != null)
        {
            displayText.text = "";
        }
        if (inputText != null)
        {
            inputText.text = "";
        }
        phoneNumber = "";
        isCalling = false;
    }
}
