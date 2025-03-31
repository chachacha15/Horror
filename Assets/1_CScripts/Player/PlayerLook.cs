using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    #region Variables

    [SerializeField] private string mouseXInputName = "Mouse X";
    [SerializeField] private string mouseYInputName = "Mouse Y";
    [SerializeField] private float mouseSensitivity = 150f;

    [SerializeField] private Transform playerBody;
    private float xAxisClamp;
    private bool m_cursorIsLocked = true;

    [SerializeField] private Camera mainCamera; // メイン一人称カメラ

    // 他クラス
    private CameraSwitcher cameraSwitcher;

    #endregion


    #region Methods

    private void Awake()
    {
        LockCursor();
        xAxisClamp = 0.0f;
    }

    private void Start()
    {
        // 他クラスを取得
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
    }

    private void LockCursor()
    {
       
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }

    private void Update()
    {
         CameraRotation();
    }


    /// <summary>
    /// カメラ視点操作メソッド
    /// </summary>
    private void CameraRotation()
    {
        // マウス入力を取得
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        // X軸の回転量にマウスY入力を加算
        xAxisClamp += mouseY;

        // プレイヤーの視点が上を向きすぎた場合（90度以上）、制限をかける
        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(270.0f);
        }
        // プレイヤーの視点が下を向きすぎた場合（-90度以下）、制限をかける
        else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(90.0f);
        }

        // プレイヤーが隠れているかどうかで分岐
        if (!cameraSwitcher.isPlayerHiding && mainCamera)
        {
            // 通常操作（カメラとプレイヤー両方回転）
            mainCamera.transform.Rotate(Vector3.left * mouseY);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else if(cameraSwitcher.isPlayerHiding)
        {
            // クローゼットカメラを操作（プレイヤーは回転させない）
            cameraSwitcher.currentClosetCamera.transform.Rotate(Vector3.left * mouseY);
            cameraSwitcher.currentClosetCamera.transform.Rotate(Vector3.up * mouseX, Space.World); // ワールド空間で左右回転
        }
    }


    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;

        // 現在アクティブなカメラを操作
        if (!cameraSwitcher.isPlayerHiding) mainCamera.transform.eulerAngles = eulerRotation;
        else cameraSwitcher.currentClosetCamera.gameObject.transform.eulerAngles = eulerRotation;
    }

    
    #endregion
}
