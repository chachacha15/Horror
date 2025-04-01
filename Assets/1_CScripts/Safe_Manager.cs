using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 変更点: EventSystem利用のため追加
using System.Collections;
using TMPro;

public class Safe_Manager : MonoBehaviour
{
    // 正しい4桁の番号を定義（例："1234"）
    public string correctCode = "1234";

    // 入力フィールドのGameObjectを設定（初期状態は非表示にする）
    public GameObject inputFieldObject;
    private TMP_InputField inputField;

    // 金庫の各パーツの参照をInspectorから設定
    public GameObject door;    // 金庫の扉部分
    public GameObject dial;    // 金庫のダイヤル部分（扉の子オブジェクト）
    public GameObject handle;  // 金庫のハンドル部分（扉の子オブジェクト）

    // 金庫の開閉状態を管理する変数
    private bool isOpen = false;

    // 扉を開くための最終的なY軸の角度（例: -90度に設定）
    public float openDoorAngle = -90f;
    // 扉が開くスピード（度/秒）
    public float doorOpenSpeed = 90f;

    // 他のキー入力処理を持つオブジェクトを一時的に無効化するための参照
    public GameObject otherInputManager;

    void Start()
    {
        if (inputFieldObject != null)
        {
            inputFieldObject.SetActive(false);
            inputField = inputFieldObject.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                // InputFieldを数字入力専用に設定
                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                inputField.interactable = true;
                inputField.ForceLabelUpdate();

                // 入力完了時のイベントにリスナーを追加
                inputField.onEndEdit.AddListener(ValidateCode);
            }
        }
    }

    // 金庫がクリックされた際、入力フィールドを表示し、他の入力処理を無効化
    void OnMouseDown()
    {
        if (!isOpen && inputFieldObject != null)
        {
            inputFieldObject.SetActive(true);
            // 他の入力処理を持つオブジェクトを無効化
            if (otherInputManager != null)
            {
                otherInputManager.SetActive(false);
            }

            // ゲーム全体を一時停止
            Time.timeScale = 0f;

            EventSystem.current.SetSelectedGameObject(inputFieldObject);

            //inputField.ActivateInputField();
        }
    }

    // 入力されたコードを検証するメソッド
    public void ValidateCode(string enteredCode)
    {
        // 入力が終わったらゲームを再開し、他の入力処理も有効にする
        Time.timeScale = 1f;
        if (otherInputManager != null)
        {
            otherInputManager.SetActive(true);
        }

        if (inputField.text == correctCode)
        {
            StartCoroutine(OpenSafe());
        }
        else
        {
            Debug.Log("Incorrect Code");
            inputFieldObject.SetActive(false);
        }
    }

    // 金庫を開く処理（扉、ダイヤル、ハンドルの回転を制御）
    IEnumerator OpenSafe()
    {
        isOpen = true;
        Debug.Log("Safe Opening...");
        inputFieldObject.SetActive(false);

        if (door != null)
        {
            float currentY = door.transform.eulerAngles.y;
            if (currentY > 180f)
                currentY -= 360f;
            while (currentY > openDoorAngle)
            {
                currentY -= doorOpenSpeed * Time.deltaTime;
                if (currentY < openDoorAngle)
                    currentY = openDoorAngle;
                door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, currentY, door.transform.eulerAngles.z);
                yield return null;
            }
        }

        if (dial != null)
        {
            dial.transform.eulerAngles = new Vector3(dial.transform.eulerAngles.x, dial.transform.eulerAngles.y, -30f);
        }

        if (handle != null)
        {
            handle.transform.eulerAngles = new Vector3(handle.transform.eulerAngles.x, handle.transform.eulerAngles.y, -15f);
        }

        Debug.Log("Safe Opened!");
    }
}
