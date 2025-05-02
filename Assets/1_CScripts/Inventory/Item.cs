using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    #region Variables

    private ItemChecker itemChecker;

    #endregion

    #region Interactable (IInteractable)

    public string GetInteractText()
    {
        if (itemChecker.isTakeTextChanged) return "アイテムがいっぱいです";
        return "取る";
    }

    public bool ShowInteractText => true; // テキスト表示するかどうか
    public bool ActivateCrosshair => true;

    /// <summary>
    /// クリック時、開閉
    /// </summary>
    public void Interact(GameObject targetObject)
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            itemChecker.PickupItem(targetObject);

        }

    }

    #endregion

    private void Start()
    {
        itemChecker = ItemChecker.Instance;
    }
}
