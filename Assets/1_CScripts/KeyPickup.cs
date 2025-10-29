
using UnityEngine;

// プレイヤーが触れるようにColliderが必須
[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string keyName = "MyKey"; // インベントリ管理用のキー名
    private bool isTaken = false;

    void Start()
    {
        // プレイヤーがぶつからないよう、ColliderはTriggerにしておく
        GetComponent<Collider>().isTrigger = true;
    }

    public string GetInteractText()
    {
        return isTaken ? "" : "鍵を拾う";
    }

    public bool ShowInteractText => !isTaken;
    public bool ActivateCrosshair => !isTaken;

    public void Interact(GameObject targetObject)
    {
        if (isTaken) return;

        isTaken = true;

        // （ここに、プレイヤーのインベントリに鍵を追加する処理）
        // PlayerInventory.Instance.AddKey(keyName);
        Debug.Log($"プレイヤーが「{keyName}」を拾った！");

        // 拾ったら鍵を非表示にする
        gameObject.SetActive(false);
    }
}