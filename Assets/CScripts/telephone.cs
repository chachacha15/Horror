using UnityEngine;
using UnityEngine.UI;

public class Telephone : MonoBehaviour
{
    public GameObject phoneUI; // 電話のUI
    public UnityEngine.UI.Text displayText; // メッセージ表示用テキスト
    public UnityEngine.UI.Text inputText; // 入力した番号表示用テキスト

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
        if (phoneNumber.Length < 11) // 11桁まで入力可能
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
                displayText.text = "発信中...";
            }
            Invoke(nameof(ReceiveCall), 2f); // 2秒後に応答
        }
    }

    private void ReceiveCall()
    {
        if (displayText != null)
        {
            displayText.text = phoneNumber switch
            {
                "123" => "助けて...ここは暗い...",
                "666" => "お前は見てはいけないものを見た",
                "999" => "これは現実ではない...",
                _ => "ツー...ツー...ツー..."
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
