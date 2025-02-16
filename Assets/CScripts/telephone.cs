using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class PhoneController : MonoBehaviour
{
    [SerializeField] private GameObject keypadPanel; // �L�[�p�b�hUI
    [SerializeField] private TMP_Text inputText; // ���͂����ԍ���\��
    [SerializeField] private TMP_Text messageText; // ���b�Z�[�W��\��
    [SerializeField] private string[] correctNumbers = { "1234", "5678" }; // �����̐���ԍ���ݒ�\
    [SerializeField] private AudioSource buttonSound; // �{�^�����i�I�v�V�����j
    [SerializeField] private AudioSource correctSound; // �������̉��i�I�v�V�����j
    [SerializeField] private AudioSource incorrectSound; // �ԈႢ���̉��i�I�v�V�����j

    private string currentInput = "";
    private bool isKeypadActive = false;

    void Start()
    {
        if (keypadPanel == null)
        {
            UnityEngine.Debug.LogError("KeypadPanel �� Inspector �Őݒ肳��Ă��܂���I");
            return;
        }

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

        if (!isKeypadActive)
        {
            ClearInput();
        }
    }

    public void PressKey(string key)
    {
        if (!isKeypadActive) return; // �L�[�p�b�h���J���Ă��Ȃ��ꍇ�͉������Ȃ�

        if (currentInput.Length < 4) // ���͂�4���܂łɐ���
        {
            currentInput += key;
            inputText.text = currentInput;

            if (buttonSound != null)
            {
                buttonSound.Play();
            }
        }
    }

    public void Call()
    {
        if (!isKeypadActive) return; // �L�[�p�b�h���J���Ă��Ȃ��ꍇ�͉������Ȃ�

        bool isCorrect = false;
        foreach (string correctNumber in correctNumbers)
        {
            if (currentInput == correctNumber)
            {
                isCorrect = true;
                break;
            }
        }

        if (isCorrect)
        {
            messageText.text = "�������ԍ��ł��B���b�Z�[�W���\������܂��B";
            if (correctSound != null)
            {
                correctSound.Play();
            }
        }
        else
        {
            messageText.text = "�ԍ����Ԉ���Ă��܂��B";
            if (incorrectSound != null)
            {
                incorrectSound.Play();
            }
        }

        currentInput = "";
        inputText.text = "";
    }

    public void ClearInput()
    {
        currentInput = "";
        inputText.text = "";
    }
}
