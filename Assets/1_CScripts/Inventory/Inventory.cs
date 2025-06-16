using System;
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
    public Texture[] displayTextures = new Texture[5]; // インベントリ表示用のレンダーテクスチャ
    public Transform[] displayCameraTransform = new Transform[5]; // インベントリ表示用のカメラの位置を取得するのに使う
    private GameObject[] displayObjects = new GameObject[5]; // ディスプレイ用のオブジェクトを入れる配列
    public int maxItems = 5; // 最大所持アイテム数


    // アイテムを手に持つときに使う
    public Transform handItemPosition; // 手に持つアイテムの位置オブジェクトのtransform
    public Transform handItemParent; // 手に持つアイテムの親オブジェクトのtransform

    public GameObject selectedItemObject; // 現在手に持っているアイテム
    public PocketItem selectedItem; // (これを外部から参照してギミックを作ってください)

    public Sprite normalSlotImage; // 通常のアイテムスロットの見た目
    public Sprite selectedSlotImage; // 選択中のアイテムスロットの見た目

    public Color normalSlotColor = Color.white; // 通常時のスロットカラー
    public Color selectedSlotColor = Color.yellow; // 選択中のスロットカラー

    // 手に持っているアイテムが手にゆっくりと追従させるときに使う
    private Vector3 velocity = Vector3.zero; // moothDamp() 内部で使用する速度の値（変化する）
    public float followSpeed = 5f; // 手に持っているアイテムの手の位置についてくるスピード
    public float threshold = 0.3f; // // 手から離れすぎないように閾値を設定する

    // 手に持っているアイテムに揺れを追加するときに使う
    public float shakeIntensity = 0.01f; // 揺れの強さ
    public float shakeSpeed = 3f; // 揺れの速さ
    private float noiseOffsetX; // x方向の位置ノイズ
    private float noiseOffsetY; // y方向の位置ノイズ
    private float noiseOffsetZ; // z方向の位置ノイズ

    private int currentSlotIndex = -1; // 現在選択されているスロット(-1で持たないようにする)
    public Transform droppedItemParent; // 落としたアイテム用の親オブジェクト

    public ItemDataBase itemDataBase; // アイテムデータベースを参照

    // 他クラス
    private CameraSwitcher cameraSwitcher;
    


    #endregion

    
    private void Start()
    {
        selectedItem = null; // 手に持っているアイテムを初期化する
        selectedItemObject = null; // 手に持っているアイテムのオブジェクトを初期化する

        // 他クラスを取得
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();

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

        // マウススクロールでアイテム変更
        if (Input.mouseScrollDelta.y > 0) // 上スクロール（次のアイテム）
        {
            ScrollItem(1);
        }
        else if (Input.mouseScrollDelta.y < 0) // 下スクロール（前のアイテム）
        {
            ScrollItem(-1);
        }


        // `Q` キーでアイテムを落とす
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }

        // 手に持っているアイテムの位置や回転に遅延をかける・揺れを追加する
        if (selectedItemObject != null)
        {
            // 現在の位置と回転を手の位置と回転に（スムーズに遅れる）

            // 手との距離を計算
            float distance = Vector3.Distance(selectedItemObject.transform.position, handItemPosition.position);

            // しきい値 (`threshold`) を超えたら、補正
            if (distance < threshold)
            {
                // ゆっくりと追従
                selectedItemObject.transform.position = Vector3.SmoothDamp(selectedItemObject.transform.position, handItemPosition.position, ref velocity, 1f / followSpeed);        
            }
            else
            {
                // 猛スピードで追従
                selectedItemObject.transform.position = Vector3.SmoothDamp(selectedItemObject.transform.position, handItemPosition.position, ref velocity, 1f / (followSpeed * 10));
            }

            selectedItemObject.transform.rotation = Quaternion.Slerp(selectedItemObject.transform.rotation, handItemPosition.rotation, Time.deltaTime * followSpeed);


            // 揺れ（Perlin Noiseを使用）
            float shakeX = (Mathf.PerlinNoise(Time.time * shakeSpeed + noiseOffsetX, 0f) - 0.5f) * shakeIntensity;
            float shakeY = (Mathf.PerlinNoise(Time.time * shakeSpeed + noiseOffsetY, 1f) - 0.5f) * shakeIntensity;
            float shakeZ = (Mathf.PerlinNoise(Time.time * shakeSpeed + noiseOffsetZ, 2f) - 0.5f) * shakeIntensity;

            // 現在の位置に揺れを追加
            selectedItemObject.transform.position += new Vector3(shakeX, shakeY, shakeZ);
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
        selectedItemObject = Instantiate(selectedItem.item, handItemPosition.position, selectedItem.item.transform.rotation);
        selectedItemObject.tag = "Untagged";

        selectedItemObject.transform.SetParent(handItemParent); // 手に持たせる
        selectedItemObject.transform.localPosition = Vector3.zero; // 手の位置に合わせる

        // 回転を設定
        if (selectedItem.item.name == "Flashlight") // フラッシュライトは向きが違うため例外
        {
            selectedItemObject.transform.localRotation = Quaternion.Euler(-90f, 0, 180f);
        }
        else
        {
            selectedItemObject.transform.localRotation = Quaternion.Euler(-90f, 0, 0);
        }

        // 手に持っていいるアイテムのスロットの色を更新
        UpdateSlotColors(index);


    }


    /// マウススクロールでアイテムを変更
    private void ScrollItem(int direction)
    {
        if (items.Count == 0) return; // アイテムがない場合は何もしない
        else if (items.Count == 1)
        {
            
            EquipItem(1); // １個の場合は、選択状態にして動かさない
            return;
        }
        // 現在のインデックスを変更
        int newIndex = currentSlotIndex + direction;

        // インデックスをループさせる
        if (newIndex >= items.Count) newIndex = 0; // 最後 → 最初
        if (newIndex < 0) newIndex = items.Count - 1; // 最初 → 最後

        EquipItem(newIndex);
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
            selectedItem = null;
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

    // 手に持っているアイテムをその場の落とす処理
    private void DropItem()
    {
        if (selectedItemObject == null)
        {
            Debug.Log("手に持っているアイテムがありません！");
            return;
        }

        // 落とす位置（プレイヤーの前方 & 少し下）
        Vector3 dropPosition = handItemPosition.position + handItemPosition.forward * 0.5f + Vector3.down * 0.5f;

        // アイテムを生成
        GameObject droppedItem = Instantiate(selectedItemObject, dropPosition, selectedItemObject.transform.rotation);
        droppedItem.gameObject.name = items[currentSlotIndex].item.name;
        droppedItem.tag = "Item";

        droppedItem.transform.SetParent(droppedItemParent); // 親オブジェクトを移動

        // Rigidbody を追加して自然に落ちるようにする
        if (!droppedItem.GetComponent<Rigidbody>())
        {
            Rigidbody rb = droppedItem.AddComponent<Rigidbody>();
            rb.mass = 1.0f;
            rb.angularDrag = 0.05f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // すり抜け防止
        }

        // インベントリ更新
        RemoveHeldItem();

        Debug.Log("アイテムを落としました: " + droppedItem.name);
    }

    #endregion

    #region インベントリ更新処理

    // アイテムを追加
    public void AddItem(PocketItem item)
    {
        // インベントリがいっぱいのときは拾えない
        if (items.Count >= maxItems)
        {
            Debug.Log("インベントリが満杯です！");
            return;
        }
        // インベントリが無かったときは、そのアイテムを持つ
        else if (items.Count == 0) 
        {
            items.Add(item);
            EquipItem(0);
        }
        // その他は、そのまま拾う
        else 
        { 
            items.Add(item); 
        }
        Debug.Log($"アイテムを追加: {item.item.name}");

        // 初めて取得したアイテムならリストに追加
        if (!haveGotItems.Contains(item.item.name))
        {
            haveGotItems.Add(item.item.name);
            Debug.Log($"初めて入手したアイテム: {item.item.name}");
        }




        UpdateInventoryUI(); // アイテム追加時にUIを更新
    }


    /// <summary>
    /// 手に持っているアイテムを削除する処理
    /// </summary>
    public void RemoveHeldItem()
    {
        if (selectedItemObject == null)
        {
            Debug.Log("手に持っているアイテムがありません！");
            return;
        }
        

        // 現在手に持っているアイテムを削除
        Destroy(selectedItemObject);
        selectedItemObject = null;
        selectedItem= null;





        // インベントリから削除
        if (currentSlotIndex != -1 && currentSlotIndex < items.Count)
        {
            Debug.Log($"アイテムを削除: {items[currentSlotIndex].item.name}");
            for (int i = currentSlotIndex; i < items.Count; i++)
            {
                foreach (Transform child in itemListParent[i])
                {
                    Destroy(child.gameObject);
                }
            }
            items.RemoveAt(currentSlotIndex); // インベントリリストから削除

            // スロットの画像を通常のものに戻す
            itemListParent[currentSlotIndex].GetComponent<Image>().sprite = normalSlotImage;
            itemListParent[currentSlotIndex].GetComponent<Image>().color = normalSlotColor;
            currentSlotIndex = -1;
        }

        // スロットのアイコンを更新
        UpdateInventoryUI();

        Debug.Log("手に持っているアイテムを削除しました。");
    }

    /// <summary>
    /// インベントリのUIを更新
    /// </summary>
    public void UpdateInventoryUI()
    {

        for (int i = 0; i < items.Count; i++)
        {
            foreach (Transform child in itemListParent[i])
            {
                Destroy(child.gameObject);
            }

            Destroy(displayObjects[i]);
        }

        // インベントリ内のアイテムを順番に表示
        for (int i = 0; i < items.Count; i++)
        {

            // アイテムスロットを生成
            GameObject slot = Instantiate(itemSlotPrefab, itemListParent[i].transform);
            slot.name = items[i].item.name;


            


            //　アイテム名を設定
            Text itemNameText = slot.transform.Find("ItemName").GetComponent<Text>();
            if (itemNameText != null)
            {
                itemNameText.text = items[i].item.name;
            }

            // アイコンを設定
            RawImage iconImage = slot.transform.Find("ItemIcon").GetComponent<RawImage>();
            if (iconImage != null && displayTextures[i] != null)
            {
                iconImage.texture = displayTextures[i];
            }
            else
            {
                Debug.LogWarning($"アイコンが見つからないか、スプライトが設定されていません: {items[i].item.name}");
            }
            // アイテムをスロット内に表示
            displayObjects[i] = Instantiate(items[i].item);
            displayObjects[i].transform.SetParent(displayCameraTransform[i]); // 親を設定して移動を簡単に
            displayObjects[i].transform.localPosition = items[i].iconPosition;
            displayObjects[i].transform.localRotation = items[i].iconRotation;
            displayObjects[i].transform.localScale = items[i].iconScale;
            //displayCameraTransform[i].transform.localScale = new Vector3(displayCameraTransform[i].transform.localScale.x * 3f, displayCameraTransform[i].transform.localScale.y * 3f, displayCameraTransform[i].transform.localScale.z * 3f);

            // 不要なコンポーネントを無効化（物理やインタラクションなど）
            Collider collider = displayObjects[i].GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            Rigidbody rigidbody = displayObjects[i].GetComponent<Rigidbody>();
            if (rigidbody != null) rigidbody.isKinematic = true;

        }

    }

    #endregion 
}
