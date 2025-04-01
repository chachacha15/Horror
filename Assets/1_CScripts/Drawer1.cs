using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer1 : MonoBehaviour
{

    #region Variables

    [SerializeField] private float interactDistance = 2.0f; // 触れる距離
    [SerializeField] private Vector3 openOffset = new Vector3(0, 0.22f, 0); // 開く方向と距離
    [SerializeField] private float openDuration = 0.5f; // 開閉時間

    [SerializeField] private bool isLocked = false; // 鍵付きかどうか


    // サウンド
    [SerializeField] private AudioClip drawerSound;
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] private AudioClip unlockSound;
    private AudioSource drawerAS;


    private bool isOpen = false;
    private bool isMoving = false; // アニメーション中かどうか
    private Vector3 closedPosition;
    private Vector3 openedPosition;

    // 他クラス
    CameraSwitcher cameraSwitcher;
    ItemChecker itemChecker;
    Inventory inventory;

    #endregion


    #region Methods

    // Start is called before the first frame update
    void Start()
    {
        //他クラスを取得
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        itemChecker = FindObjectOfType<ItemChecker>();
        inventory = FindObjectOfType<Inventory>();

        drawerAS = GetComponent<AudioSource>();

        isOpen = false;

        closedPosition = transform.localPosition;
        openedPosition = closedPosition + openOffset;

    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !cameraSwitcher.isPlayerHiding && !itemChecker.isLookingItem)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    ToggleDrawer();
                }
            }
        }
    }


    private void ToggleDrawer()
    {
        // 鍵がかかっていないとき
        if (!isLocked)
        {
            if (!isMoving)
            {
                StartCoroutine(MoveDrawer(isOpen ? openedPosition : closedPosition,
                                          isOpen ? closedPosition : openedPosition));
                isOpen = !isOpen;

                // サウンド再生
                drawerAS.pitch = 2.50f;
                drawerAS.PlayOneShot(drawerSound);
            }
        }

        // 鍵がかかっているとき
        else
        {
            // 引き出しのカギを開けたとき
            if (inventory.selectedItem != null && inventory.selectedItem.item.name == "Drawer1_Key")
            {
                inventory.RemoveHeldItem();
                isLocked = false;

                // サウンド再生
                drawerAS.pitch = 0.65f;
                drawerAS.PlayOneShot(unlockSound);            

            }

            // 開けられないとき
            else
            {
                drawerAS.PlayOneShot(lockedSound);
            }
        }
    }


    /// <summary>
    /// 引き出しアニメーション処理
    /// </summary>
    private IEnumerator MoveDrawer(Vector3 from, Vector3 to)
    {
        Debug.Log(isOpen);
        isMoving = true;
        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            transform.localPosition = Vector3.Lerp(from, to, elapsed / openDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = to;
        isMoving = false;
    }

    #endregion
}
