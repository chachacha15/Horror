using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class PhoneController : MonoBehaviour
{
    [SerializeField] private GameObject keypadPanel; // キーパッドUI
    [SerializeField] private TMP_Text inputText; // 入力した番号を表示
    [SerializeField] private TMP_Text messageText; // メッセージを表示
    [SerializeField] private string[] correctNumbers = { "1234", "5678" }; // 複数の正解番号を設定可能
    [SerializeField] private AudioSource buttonSound; // ボタン音（オプション）
    [SerializeField] private AudioSource correctSound; // 正解時の音（オプション）
    [SerializeField] private AudioSource incorrectSound; // 間違い時の音（オプション）

    private string currentInput = "";
    private bool isKeypadActive = false;

    void Start()
    {
        if (keypadPanel == null)
        {
            UnityEngine.Debug.LogError("KeypadPanel が Inspector で設定されていません！");
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
        if (!isKeypadActive) return; // キーパッドが開いていない場合は何もしない

        if (currentInput.Length < 4) // 入力は4桁までに制限
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
        if (!isKeypadActive) return; // キーパッドが開いていない場合は何もしない

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
            messageText.text = "正しい番号です。メッセージが表示されます。";
            if (correctSound != null)
            {
                correctSound.Play();
            }
        }
        else
        {
            messageText.text = "番号が間違っています。";
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
