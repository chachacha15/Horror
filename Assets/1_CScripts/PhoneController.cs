using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class PhoneController : MonoBehaviour
{
    [SerializeField] private GameObject keypadPanel; // �L�[�p�b�h�� UI
    [SerializeField] private TMP_Text inputText; // ���͕\��
    [SerializeField] private TMP_Text messageText; // ���b�Z�[�W�\��
    [SerializeField] private string[] correctNumbers = { "1234", "5678" }; // ����ԍ�
    [SerializeField] private AudioSource buttonSound; // �{�^�����i�I�v�V�����j
    [SerializeField] private AudioSource correctSound; // �������̉�
    [SerializeField] private AudioSource incorrectSound; // �ԈႢ���̉�
    [SerializeField] private bool isLookingTelephone = false; // �d�b�����Ă��邩�ǂ���


    private CameraSwitcher cameraSwitcher;

    private string currentInput = "";
    private bool isKeypadActive = false;

    void Start()
    {
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();

        keypadPanel.SetActive(false);
        messageText.text = "";
    }

    private void Update()
    {
        if (isLookingTelephone)
        {
            cameraSwitcher.ClosshairAnimation(10f, 500f, 0.5f, cameraSwitcher.crosshairRectTransform, isLookingTelephone);
        }
        else if (!isLookingTelephone)
        {
            cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingTelephone);
        }
    }
    private void OnMouseEnter()
    {
        // �J�[�\�������킹���Ƃ�
        isLookingTelephone = true;
    }
    private void OnMouseExit()
    {
        // �J�[�\�����O�ꂽ�Ƃ��̏���
        isLookingTelephone = false;
    }

    private void OnMouseDown()
    {
        // �N���b�N�����Ƃ��̏���
         if(!isKeypadActive) ToggleKeypad();
    }

    public void ToggleKeypad()
    {
        isKeypadActive = !isKeypadActive;
        keypadPanel.SetActive(isKeypadActive);

        Time.timeScale = 0f; // �����~�߂�
        Cursor.lockState = CursorLockMode.None; // �}�E�X��\��
        Cursor.visible = true;

        if (!isKeypadActive)
        {
            messageText.text = "";
        }
    }

    public void PressKey(string key)
    {
        if (!isKeypadActive) return;

        if (currentInput.Length < 4)
        {
            currentInput += key;
            inputText.text = currentInput;

            if (buttonSound != null)
            {
                buttonSound.Play();
            }
        }
    }

    public void Backspace()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            inputText.text = currentInput;
        }
    }

    public void Call()
    {
        if (!isKeypadActive) return;

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
            messageText.text = "�������ԍ��ł��B";
            if (correctSound != null) correctSound.Play();
 
        }
        else
        {
            messageText.text = "�ԍ����Ԉ���Ă��܂��B";
            if (incorrectSound != null) incorrectSound.Play();
        }

        StartCoroutine(ClearMessageAfterDelay(2.0f));
        currentInput = "";
        inputText.text = "";
    }

    IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = "";

    }

    public void ClearInput()
    {
        currentInput = "";
        inputText.text = "";
    }

    public void EscapeInput()
    {
        isKeypadActive = !isKeypadActive;
        keypadPanel.SetActive(isKeypadActive);

        Time.timeScale = 1f; // ���𓮂���
        Cursor.lockState = CursorLockMode.Locked; // �}�E�X���\��
        Cursor.visible = false;
    }
}
