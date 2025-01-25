using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    #region Variables
    public GameObject ItemDisplayUI; // インベントリ画面の親オブジェクト
    public Transform itemListParent; // アイテムリストを表示する親オブジェクト
    public GameObject itemSlotPrefab; // アイテムスロットのプレハブ
    public Inventory inventory; // インベントリデータを参照
    public Camera itemDisplayCamera; // アイテム表示用カメラ
    public Transform itemDisplayPosition; // アイテムを配置する位置

    private GameObject currentDisplayedItem; // 表示中のアイテムを追跡
    private bool isItemDisplayON  = false; // インベントリの開閉状態

    private bool isPausedByDisplayItem = false; // 一時停止状態を追跡
    private GameObject slot; //現在表示しているアイテムを格納
    #endregion

    private void Start()
    {
        if (ItemDisplayUI != null)
        {
            ItemDisplayUI.SetActive(false); // 初期状態で非表示
        }
    }

    void Update()
    {
        if (isItemDisplayON)
        {
            // 時間に基づいて上下に移動
            float floatOffset = Mathf.Sin(Time.unscaledTime * 2f) * 0.05f; // 振幅と速度を調整
            currentDisplayedItem.transform.position = itemDisplayPosition.position + new Vector3(0f, floatOffset, 0f);
        }

        //　ディスプレイ画面を閉じる時
        if (isPausedByDisplayItem && Input.GetMouseButtonDown(0)) // 0は左クリック
        {
            ResumeGame();
            isItemDisplayON = !isItemDisplayON;
            ItemDisplayUI.SetActive(isItemDisplayON);

            Cursor.lockState = CursorLockMode.Locked; // マウスを非表示
            Cursor.visible = false;                  // マウスカーソルを非表示
            ClearDisplayedItem();                    // 表示中のアイテムをクリア
        }
    }


    public void ToggleItemDisplay()
    {
        isItemDisplayON = !isItemDisplayON;

        if (ItemDisplayUI != null)
        {
            ItemDisplayUI.SetActive(isItemDisplayON);

            if (isItemDisplayON)
            {
                UpdateItemDisplayUI(); // 開いたときに内容を更新
                Cursor.lockState = CursorLockMode.None; // マウスを表示
                Cursor.visible = true;                 // マウスカーソルを表示
            }
            
        }
    }

    private void UpdateItemDisplayUI()
    {
        

        // 最新のアイテムを取得
        var item = inventory.items[inventory.items.Count - 1];

        // スロットを生成
        slot = Instantiate(itemSlotPrefab, itemListParent);
        slot.name = "Displayed : " + item.item.name;

        TextMeshProUGUI itemNameText = slot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
        //Debug.Log(itemNameText);

        if (itemNameText != null)
        {
            itemNameText.text = item.item.name;
        }
        else
        {
            Debug.LogError("ItemName に TextMeshProUGUI コンポーネントが見つかりません！");
        }


            

        TextMeshProUGUI itemDescriptionText = slot.transform.Find("ItemDiscription").GetComponent<TextMeshProUGUI>();

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = item.explainText;
        }
        else
        {
            Debug.LogError("ItemDiscription に Text コンポーネントが見つかりません！");
        }
            

        Image itemIcon = slot.transform.Find("ItemIcon").GetComponent<Image>();
        if (itemIcon != null && item.item.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
        {
            itemIcon.sprite = spriteRenderer.sprite;
        }
        else if (itemIcon == null)
        {
            Debug.LogError("ItemIcon に Image コンポーネントが見つかりません！");
        }

        PocketItem currentItem = item; // クロージャ問題を防ぐため
        DisplayItemIn3D(currentItem.item);
            
            
    }

    private void DisplayItemIn3D(GameObject item)
    {
        // 既に表示中のアイテムがあれば削除
        ClearDisplayedItem();

        // アイテムを表示位置に配置 (X軸を-90度回転)
        currentDisplayedItem = Instantiate(item, itemDisplayPosition.position, Quaternion.Euler(-80f, 34f, 0f));
        currentDisplayedItem.transform.SetParent(itemDisplayPosition); // 親を設定して移動を簡単に

        // 不要なコンポーネントを無効化（物理やインタラクションなど）
        Collider collider = currentDisplayedItem.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        Rigidbody rigidbody = currentDisplayedItem.GetComponent<Rigidbody>();
        if (rigidbody != null) rigidbody.isKinematic = true;

        // ゲームを一時停止
        Time.timeScale = 0f;
        isPausedByDisplayItem = true; // 一時停止フラグを有効化

    }

    private void ClearDisplayedItem()
    {
        if (currentDisplayedItem != null)
        {
            Destroy(currentDisplayedItem);
            currentDisplayedItem = null;
            Destroy(slot);
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // ゲームを再開
        isPausedByDisplayItem = false; // フラグをリセット
    }
}
