using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bookreader : MonoBehaviour, IInteractable
{
    [Header("ギミック設定")]
    [SerializeField] private float moveDistanceX = 1.5f;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float uiDisplayDelay = 0.5f;
    [SerializeField] private bool hasOpened = false; // "hasMoved" -> "hasOpened" に名前を変更
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    [Header("UI設定")]
    [SerializeField] private GameObject bookContentPanel;
    [SerializeField] private Image bookContentImage;

    // プレイヤーからの入力を検知するスクリプトへの参照を追加
    private ClickDetector clickDetector;

    private void Start()
    {
        originalPosition = transform.position;
        if (bookContentPanel != null)
        {
            bookContentPanel.SetActive(false);
        }

        // ClickDetectorのインスタンスをシーンから取得
        clickDetector = FindObjectOfType<ClickDetector>();
        if (clickDetector == null)
        {
            Debug.LogError("ClickDetectorが見つかりません。");
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * moveSpeed);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    // --- IInteractableの実装 ---

    public string GetInteractText()
    {
        if (hasOpened) return "本を閉じる";
        return "本を読む";
    }

    public bool ShowInteractText => true;
    public bool ActivateCrosshair => true;

    public void Interact(GameObject targetObject)
    {
        if (!isMoving && !hasOpened)
        {
            OpenBook();
        }
    }

    // 本を開く処理を独立したメソッドに
    private void OpenBook()
    {
        targetPosition = originalPosition + new Vector3(moveDistanceX, 0, 0);
        Time.timeScale = 0f;
        hasOpened = true;
        isMoving = true;
        StartCoroutine(ShowUIDelayed());

        // クリック検知スクリプトに「本が開いた」ことを通知
        if (clickDetector != null)
        {
            clickDetector.isBookOpen = true;
        }
    }

    // 本を閉じる処理を独立したメソッドに
    public void CloseBook()
    {
        if (hasOpened && !isMoving)
        {
            targetPosition = originalPosition;
            if (bookContentPanel != null)
            {
                bookContentPanel.SetActive(false);
            }
            Time.timeScale = 1f;
            hasOpened = false;
            isMoving = true;

            // クリック検知スクリプトに「本が閉じた」ことを通知
            if (clickDetector != null)
            {
                clickDetector.isBookOpen = false;
            }
        }
    }

    private IEnumerator ShowUIDelayed()
    {
        yield return new WaitForSecondsRealtime(uiDisplayDelay);

        if (bookContentPanel != null)
        {
            bookContentPanel.SetActive(true);
        }
    }
}