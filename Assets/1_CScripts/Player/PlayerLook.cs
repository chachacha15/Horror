using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    #region Variables

    public static PlayerLook Instance;

    [SerializeField] private string mouseXInputName = "Mouse X";
    [SerializeField] private string mouseYInputName = "Mouse Y";
    [SerializeField] private float mouseSensitivity = 150f;

    [SerializeField] private Transform playerBody;

    public bool IsCameraLocked = false;

    // メインカメラ用の回転値
    private float xAxisClamp;

    // クローゼットカメラ用の回転値
    private float closetXAxisRotation = 0f;
    private float closetYAxisRotation = 0f;
    private bool m_cursorIsLocked = true;

    [SerializeField] private Camera mainCamera; // メイン一人称カメラ

    // 他クラス
    private CameraSwitcher cameraSwitcher;

    #endregion


    #region Methods

    private void Awake()
    {
        Instance = this;
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
        if (!IsCameraLocked) CameraRotation();
        else
        {
            mainCamera.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            xAxisClamp = 0f;
        }
    }


    /// <summary>
    /// カメラ視点操作メソッド
    /// </summary>
    private void CameraRotation()
    {
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        
        if (cameraSwitcher && cameraSwitcher.isPlayerHiding && cameraSwitcher.currentClosetCamera)
        {
            // クローゼット時の視点操作（新しく作った回転値を使用）
            closetXAxisRotation += mouseY;
            closetYAxisRotation += mouseX;

            // 回転制限
            closetXAxisRotation = Mathf.Clamp(closetXAxisRotation, -90f, 90f);

            cameraSwitcher.currentClosetCamera.transform.rotation =
                Quaternion.Euler(-closetXAxisRotation, closetYAxisRotation, 0f);
        }
        else if(mainCamera)
        {
            // 通常時の視点操作
            xAxisClamp += mouseY;
            xAxisClamp = Mathf.Clamp(xAxisClamp, -90f, 90f);

            mainCamera.transform.localEulerAngles = new Vector3(-xAxisClamp, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
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
