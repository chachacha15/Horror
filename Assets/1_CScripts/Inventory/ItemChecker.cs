using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;

public class ItemChecker : MonoBehaviour
{
    #region Variables
    public static ItemChecker Instance { get; private set; }

    public float interactDistance = 3f; // インタラクト可能な距離

    // アイテム用
    public LayerMask itemLayer; // アイテムに使用するレイヤー
    public ItemDataBase itemDataBase; // アイテムデータベースを参照
    public Inventory inventory; // プレイヤーのインベントリを管理するスクリプト
    public ItemDisplay itemDisplay;

    // サウンド
    public AudioClip pickUpSound; //拾った時に鳴る音
    private AudioSource pickUpAS; 


    // 表示するUI用
    private TextMeshProUGUI interactTextComponent; // TextMeshProの参照
    public BoolWrapper isLookingItem = new BoolWrapper { Value = false };
    public bool isTakeTextChanged = false;

    //その他・他クラス


    Ray ray;
 

    // 血・スポンジギミック用
    public Material bloodMaterial; // 血のマテリアル
    public float fadeDuration = 1.5f; // フェードアウトの時間
    public bool hasSponge { get; private set; }  // 読み取り専用のプロパティにする


    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    private void Start()
    {
        pickUpAS = GetComponent<AudioSource>();
       
    }

    
    /// <summary>
    /// アイテムをインベントリに格納するメソッド
    /// </summary>
    /// <param name="item"></param>
    public void PickupItem(GameObject item)
    {
        PocketItem itemData = itemDataBase.itemList.Find(i => i.item.name == item.name);

        if (itemData != null)
        {

            // サウンド
            pickUpAS.PlayOneShot(pickUpSound);

            if (inventory.items.Count >= inventory.maxItems)
            {
                StartCoroutine(ChangeTakeText());
                return;
            }

            bool haveGotThisItem = inventory.haveGotItems.Contains(item.name);
            inventory.AddItem(itemData);
            Debug.Log(itemData.item.transform.name +" : "+itemData.item.transform.rotation.x);

            // 初ゲットならディスプレイに表示
            if (!haveGotThisItem)
            {
                itemDisplay.ToggleItemDisplay();
                inventory.UpdateInventoryUI();
            }            

            Destroy(item);

        }
        else
        {
            Debug.LogWarning("データベースにこのアイテムが存在しません！");
        }

    }

   /// <summary>
   /// アイテムが拾えないときのテキストを一時変更するコルーチン
   /// </summary>
   /// <returns></returns>
    private IEnumerator ChangeTakeText()
    {
        isTakeTextChanged = true;
        yield return new WaitForSeconds(2f);
        isTakeTextChanged = false;
    }
}

