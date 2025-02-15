/*using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PhoneController : MonoBehaviour
{
    public GameObject keypadPanel; // キーパッドUI
    public UnityEngine.UI.Text inputText; // 入力した番号を表示
    public UnityEngine.UI.Text messageText; // メッセージを表示
    public string correctNumber = "1234"; // 正解の番号

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
            messageText.text = "正しい番号です。メッセージが表示されます。";
        }
        else
        {
            messageText.text = "番号が間違っています。";
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
