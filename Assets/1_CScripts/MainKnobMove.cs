using UnityEngine;
using System.Collections;

public class MainKnobMove : MonoBehaviour, IInteractable
{
    [Header("ギミック設定")]
    [Tooltip("再生するアニメーターコンポーネント")]
    [SerializeField] private Animator targetAnimator;
    [Tooltip("アニメーションのトリガー名")]
    [SerializeField] private string animationTriggerName = "Activate";
    [Tooltip("ノブが起動した後に有効化するオブジェクト (例: エレベーターのインジケーター)")]
    [SerializeField] private GameObject objectToActivate;

    private bool isActivated = false;

    // --- IInteractableの実装 ---

    public string GetInteractText()
    {
        if (isActivated) return "起動済み";
        return "起動する";
    }

    public bool ShowInteractText => true;
    public bool ActivateCrosshair => true;

    /// <summary>
    /// クリック時、アニメーション再生とオブジェクト起動
    /// </summary>
    public void Interact(GameObject targetObject)
    {
        if (isActivated) return;

        // アニメーション再生
        if (targetAnimator != null)
        {
            targetAnimator.SetTrigger(animationTriggerName);
        }
        else
        {
            Debug.LogError("KnobActivator: Target Animatorが設定されていません。");
        }

        // 起動処理
        isActivated = true;
        Debug.Log("ノブが起動しました。");

        // 別のオブジェクト（エレベーターシステムなど）に通知するイベントをここで発火させることも可能

        // 起動後に表示/有効化するオブジェクト
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}
