using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class PhoneController : MonoBehaviour
{
    [SerializeField] private GameObject keypadPanel; // キーパッドの UI
    [SerializeField] private TMP_Text inputText; // 入力表示
    [SerializeField] private TMP_Text messageText; // メッセージ表示
    [SerializeField] private string[] correctNumbers = { "1234", "5678" }; // 正解番号
    [SerializeField] private AudioSource buttonSound; // ボタン音（オプション）
    [SerializeField] private AudioSource correctSound; // 正解時の音
    [SerializeField] private AudioSource incorrectSound; // 間違い時の音
    [SerializeField] private bool isLookingTelephone = false; // 電話を見ているかどうか


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
        // カーソルを合わせたとき
        isLookingTelephone = true;
    }
    private void OnMouseExit()
    {
        // カーソルが外れたときの処理
        isLookingTelephone = false;
    }

    private void OnMouseDown()
    {
        // クリックしたときの処理
         if(!isKeypadActive) ToggleKeypad();
    }

    public void ToggleKeypad()
    {
        isKeypadActive = !isKeypadActive;
        keypadPanel.SetActive(isKeypadActive);

        Time.timeScale = 0f; // 時を止める
        Cursor.lockState = CursorLockMode.None; // マウスを表示
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
            messageText.text = "正しい番号です。";
            if (correctSound != null) correctSound.Play();
 
        }
        else
        {
            messageText.text = "番号が間違っています。";
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

        Time.timeScale = 1f; // 時を動かす
        Cursor.lockState = CursorLockMode.Locked; // マウスを非表示
        Cursor.visible = false;
    }
}
