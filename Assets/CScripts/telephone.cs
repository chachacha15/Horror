/*using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PhoneController : MonoBehaviour
{
    public GameObject keypadPanel; // �L�[�p�b�hUI
    public UnityEngine.UI.Text inputText; // ���͂����ԍ���\��
    public UnityEngine.UI.Text messageText; // ���b�Z�[�W��\��
    public string correctNumber = "1234"; // �����̔ԍ�

    private string currentInput = "";
    private bool isKeypadActive = false;

    void Start()
    {
        keypadPanel.SetActive(false);
        messageText.text = "";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                ToggleKeypad();
            }
        }
    }

    public void ToggleKeypad()
    {
        isKeypadActive = !isKeypadActive;
        keypadPanel.SetActive(isKeypadActive);
    }

    public void PressKey(string key)
    {
        if (currentInput.Length < 10)
        {
            currentInput += key;
            inputText.text = currentInput;
        }
    }

    public void Call()
    {
        if (currentInput == correctNumber)
        {
            messageText.text = "�������ԍ��ł��B���b�Z�[�W���\������܂��B";
        }
        else
        {
            messageText.text = "�ԍ����Ԉ���Ă��܂��B";
        }
        currentInput = "";
        inputText.text = "";
    }

    public void ClearInput()
    {
        currentInput = "";
        inputText.text = "";
    }
}*/
