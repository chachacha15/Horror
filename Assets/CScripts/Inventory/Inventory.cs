using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    #region Variables
    // インベントリ内容に使う
    public List<string> haveGotItems = new List<string>(); // すでに入手経験のあるアイテムリスト 
    public List<PocketItem> items = new List<PocketItem>(); // インベントリ内のアイテムリスト
    public Transform[] itemListParent; // アイテムを表示する親オブジェクト
    public GameObject itemLists; //アイテムを格納するスロット
    public GameObject itemSlotPrefab; // アイテムスロットのプレハブ
    public int maxItems = 5; // 最大所持アイテム数


    // アイテムを手に持つときに使う
    public Transform handSlot; // 手に持つアイテムの位置
    public GameObject selectedItemObject; // 現在手に持っているアイテム
    public PocketItem selectedItem; // (これを外部から参照してギミックを作ってください)
    public Sprite normalSlotImage;
    public Sprite selectedSlotImage;
    public Color normalSlotColor = Color.white; // 通常時のスロットカラー
    public Color selectedSlotColor = Color.yellow; // 選択中のスロットカラー
    private int currentSlotIndex = -1; // 現在選択されているスロット(-1で持たないようにする)

    public ItemDataBase itemDataBase; // アイテムデータベースを参照


    #endregion

    private void Start()
    {
        selectedItem = null;
        UpdateInventoryUI(); // 初期状態でUIを更新
    }
    private void Update()
    {
        // 数字キー (1〜5) を押して手に持つ
        for (int i = 1; i <= maxItems; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i)) // Alpha1～Alpha5 をチェック
            {
                EquipItem(i - 1); // インデックスは 0 から始まるので -1 する
            }
        }
    }

    #region アイテムを手に持つ処理関連
    // アイテムを手に持つ
    private void EquipItem(int index)
    {

        // すでにそのスロットのアイテムを持っている場合、解除する
        if (currentSlotIndex == index)
        {
            Debug.Log($"アイテムを解除: {items[index].item.name}");
            RemoveItemFromHand();
            return;
        }

        if (index < 0 || index >= items.Count)
        {
            Debug.Log("このスロットにはアイテムがありません！");
            return;
        }

        selectedItem = items[index];
        selectedItem.item.name = items[index].item.name;
        // すでに持っているアイテムがあれば削除
        if (selectedItemObject != null)
        {
            Destroy(selectedItemObject);
        }

        // アイテムを生成して手に持つ
        Debug.Log(selectedItem.item.name);
        selectedItemObject = Instantiate(selectedItem.item, handSlot.position, selectedItem.item.transform.rotation);

        selectedItemObject.transform.SetParent(handSlot); // 手に持たせる
        selectedItemObject.transform.localPosition = Vector3.zero; // 手の位置に合わせる
        selectedItemObject.transform.localRotation = selectedItem.item.transform.localRotation; // 回転をプレハブの元の回転に設定

        // 手に持っていいるアイテムのスロットの色を更新
        UpdateSlotColors(index);
    }

    //スロットの色を更新（選択中のスロットをハイライト）
    private void UpdateSlotColors(int selectedIndex)
    {
        // 以前のスロットを通常色に戻す
        if (currentSlotIndex != -1)
        {
            itemListParent[currentSlotIndex].GetComponent<Image>().sprite = normalSlotImage;
            itemListParent[currentSlotIndex].GetComponent<Image>().color = normalSlotColor;
        }


        // 新しいスロットをハイライト
        itemListParent[selectedIndex].GetComponent<Image>().sprite = selectedSlotImage;
        itemListParent[selectedIndex].GetComponent<Image>().color = selectedSlotColor;

        // 現在のスロットを更新
        currentSlotIndex = selectedIndex;
    }

    // 手に持っているアイテムを解除
    private void RemoveItemFromHand()
    {
        if (selectedItemObject != null)
        {
            Destroy(selectedItemObject);
            selectedItemObject = null;
        }

        // スロットの画像を通常のものに戻す
        if (currentSlotIndex != -1)
        {
            itemListParent[currentSlotIndex].GetComponent<Image>().sprite = normalSlotImage;
            itemListParent[currentSlotIndex].GetComponent<Image>().color = normalSlotColor;

        }

        // 持っているアイテムをなしにする
        currentSlotIndex = -1;
    }

    #endregion



    // アイテムを追加
    public void AddItem(PocketItem item)
    {
        if (items.Count >= maxItems)
        {
            Debug.Log("インベントリが満杯です！");
            return;
        }

        items.Add(item);
        Debug.Log($"アイテムを追加: {item.item.name}");

        // 初めて取得したアイテムならリストに追加
        //if (!haveGotItems.Contains(item.item.name))
        {
            haveGotItems.Add(item.item.name);
            Debug.Log($"初めて入手したアイテム: {item.item.name}");
        }

        UpdateInventoryUI(); // アイテム追加時にUIを更新
    }

    // インベントリのUIを更新
    public void UpdateInventoryUI()
    {
       
        // インベントリ内のアイテムを順番に表示
        for (int i = 0; i < items.Count; i++)
        {
            // アイテムスロットを生成
            GameObject slot = Instantiate(itemSlotPrefab, itemListParent[i].transform);
            slot.name = items[i].item.name;

            // アイコンを設定
            Image iconImage = slot.transform.Find("ItemIcon").GetComponent<Image>();
            if (iconImage != null && items[i].icon != null)
            {
                iconImage.sprite = items[i].icon;
            }
            else
            {
                Debug.LogWarning($"アイコンが見つからないか、スプライトが設定されていません: {items[i].item.name}");
            }

            //　アイテム名を設定
            Text itemNameText = slot.transform.Find("ItemName").GetComponent<Text>();
            if (itemNameText != null)
            {
                itemNameText.text = items[i].item.name;
            }
        }

       
    }
}
