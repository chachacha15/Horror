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

        // ★修正点: objectToOpenを非表示にする処理を削除 (Cupが開きたい扉なので、常に表示されているべき)
        //           （ただし、objectToOpenが内部のブレーカーなどの場合は、そのオブジェクト自体をHierarchyで非表示にする必要があります。）

        // Safety Check: objectToOpenが内部の別のオブジェクト（ブレーカーなど）である場合にのみ非表示にする
        // シールドの扉（Cup）がobjectToOpenに設定されている場合は、このブロックが実行されないように注意してください。
        // もし、このオブジェクトが内部のブレーカーなどであれば、Hierarchy上で非表示にしてください。
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

            if (monologueManager != null)
            {
                // monologueManager.TrySettingLog(MonologueType.FindElectricSystem); 
                Debug.Log("モノローグを試行しました。");
            }
        }
    }

    // --- ギミックのコアロジック ---

    /// <summary>
    /// インベントリに指定された鍵があるかチェックする
    /// </summary>
    private bool HasRequiredKey()
    {
        if (inventory != null && inventory.selectedItem != null && inventory.selectedItem.item != null && inventory.selectedItem.item.name == requiredKeyName)
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
            // アニメーションを再生
            shieldAnimator.SetTrigger("Open");
        }

        // アニメーション再生時間(openDelay)を待つ
        yield return new WaitForSeconds(openDelay);

        // 扉が開いた後、内部のオブジェクトを表示 (objectToOpenが内部のブレーカーなどの場合)
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