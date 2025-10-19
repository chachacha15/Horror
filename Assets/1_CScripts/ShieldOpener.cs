using UnityEngine;
using TMPro;
using System.Collections;

public class ShieldOpener : MonoBehaviour, IInteractable
{
    [Header("ギミック設定")]
    [SerializeField] private string requiredKeyName;
    [SerializeField] private GameObject objectToOpen;
    [SerializeField] private float openDelay = 0.5f;

    private bool isOpened = false;

    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI interactTextDisplay;

    // 他クラス
    private Inventory inventory;
    private MonologueManager monologueManager;
    private Animator shieldAnimator;

    private void Start()
    {

        monologueManager = MonologueManager.Instance;
        inventory = Inventory.Instance;


        shieldAnimator = GetComponent<Animator>();
        if (shieldAnimator == null)
        {
            Debug.LogError("Animatorコンポーネントがアタッチされていません。");
        }

        if (objectToOpen != null)
        {
            objectToOpen.SetActive(false);
        }
    }

    // --- IInteractableの実装 ---

    public string GetInteractText()
    {
        if (isOpened) return "開いている";

        if (HasRequiredKey())
        {
            return "開ける";
        }
        else
        {
            return "ロックされている";
        }
    }

    public bool ShowInteractText => true;
    public bool ActivateCrosshair => true;

    public void Interact(GameObject targetObject)
    {
        if (isOpened) return;

        if (HasRequiredKey())
        {
            StartCoroutine(OpenShield());
        }
        else
        {
            StartCoroutine(ShowLockedText());
            monologueManager.TrySettingLog(MonologueType.FindElectricSystem);
        }
    }

    // --- ギミックのコアロジック ---

    /// <summary>
    /// インベントリに指定された鍵があるかチェックする
    /// </summary>
    private bool HasRequiredKey()
    {
        // ★修正点: DoorControllerのロジックを参考に、この部分を修正します
        if (inventory.selectedItem != null && inventory.selectedItem.item.name == requiredKeyName)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// シールドを開くコルーチン
    /// </summary>
    private IEnumerator OpenShield()
    {
        isOpened = true;

        if (shieldAnimator != null)
        {
            shieldAnimator.SetTrigger("Open");
        }

        yield return new WaitForSeconds(openDelay);

        if (objectToOpen != null)
        {
            objectToOpen.SetActive(true);
        }

        Debug.Log("シールドが開きました！");
    }

    /// <summary>
    /// 鍵がないときにテキストを表示するコルーチン
    /// </summary>
    private IEnumerator ShowLockedText()
    {
        if (interactTextDisplay != null)
        {
            string originalText = interactTextDisplay.text;
            interactTextDisplay.text = "鍵がかかっている";

            yield return new WaitForSeconds(1.0f);
            interactTextDisplay.text = originalText;
        }
    }
}