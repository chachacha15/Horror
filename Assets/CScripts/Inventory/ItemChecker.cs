/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;



public class ItemChecker : MonoBehaviour
{
    #region Variables
    public float interactDistance = 3f; // インタラクト可能な距離
    public LayerMask itemLayer; // アイテムに使用するレイヤー
    public GameObject interactText; // UI テキスト (拾うメッセージを表示)
    public ItemDataBase itemDataBase; // アイテムデータベースを参照
    public Inventory inventory; // プレイヤーのインベントリを管理するスクリプト
    public ItemDisplay itemDisplay;


    private TextMeshProUGUI interactTextComponent; // TextMeshProの参照

    [SerializeField] GameObject flashLightSystem;
    [SerializeField] GameObject flashlightTutorial;
    private TutorialManager tutorialManager;


    #endregion

    public Material bloodMaterial; // 血のマテリアル
    public float fadeDuration = 1.5f;    // フェードアウトの時間


    private void Start()
    {
        interactTextComponent = interactText.GetComponent<TextMeshProUGUI>();

        if (interactTextComponent == null)
        {
            Debug.LogError("interactTextにTextMeshProUGUIコンポーネントが見つかりません！");
        }

        
        tutorialManager = flashlightTutorial.GetComponent<TutorialManager>();
    }

    private void Update()
    {
        // レイキャストでアイテムを検出
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        

        if (Physics.Raycast(ray, out hit, interactDistance, itemLayer))
        {
            GameObject hitItem = hit.collider.gameObject;

            if (hitItem.CompareTag("Item"))
            {
                interactTextComponent.text = $"取る";
                interactText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                {
                    PickupItem(hitItem);
                    itemDisplay.ToggleItemDisplay();
                    inventory.UpdateInventoryUI();

                }
            }
        }
        else
        {
            interactText.SetActive(false);
        }
    }

    private void PickupItem(GameObject item)
    {
        // アイテムのデータを検索
        PocketItem itemData = itemDataBase.itemList.Find(i => i.item.name == item.name);

        if (itemData != null)
        {
            // インベントリにアイテムを追加
            inventory.AddItem(itemData);

            // Flashlightの場合にFlashLightSystemをアクティブ化
            if (item.name == "Flashlight")
            {
                if (flashLightSystem != null)
                {
                    //フラッシュライトを使えるようにする
                    flashLightSystem.SetActive(true);
                    
                    if (tutorialManager != null)
                    {
                        StartCoroutine(tutorialManager.ShowTutorial()); // コルーチンを直接呼び出す
                    }

                    //フラッシュライトのチュートリアルを表示する
                    flashlightTutorial.SetActive(true);
                    flashlightTutorial.transform.GetChild(0).gameObject.SetActive(true);

                }
               
            }

            if (item.name == "sponge")
            {
                private void OnMouseDown()
                {
                    // 浴槽がクリックされた時の処理
                    if (bloodMaterial != null)
                    {
                        StartCoroutine(FadeOutBlood());
                    }
                }

                private IEnumerator FadeOutBlood()
                {
                    float elapsedTime = 0f;
                    Color color = bloodMaterial.color;
                    float startAlpha = color.a;

                    // 徐々に透明度を0にしていく
                    while (elapsedTime < fadeDuration)
                    {
                        elapsedTime += Time.deltaTime;
                        color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
                        bloodMaterial.color = color;
                        yield return null;
                    }

                    // 完全に透明にする
                    color.a = 0f;
                    bloodMaterial.color = color;
                }
                /*
                void OnMouseDown()
                {
                    if (bloodMaterial != null)
                    {
                        // マテリアルの透明度を0にして非表示にする
                        Color color = bloodMaterial.color;
                        color.a = 0f;  // 透明度を0にする
                        bloodMaterial.color = color;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("データベースにこのアイテムが存在しません！");
        }

        // アイテムをシーンから削除
        Destroy(item);
    }

    
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemChecker : MonoBehaviour
{
    #region Variables
    public float interactDistance = 3f; // インタラクト可能な距離
    public LayerMask itemLayer; // アイテムに使用するレイヤー
    public GameObject interactText; // UI テキスト (拾うメッセージを表示)
    public ItemDataBase itemDataBase; // アイテムデータベースを参照
    public Inventory inventory; // プレイヤーのインベントリを管理するスクリプト
    public ItemDisplay itemDisplay;

    private TextMeshProUGUI interactTextComponent; // TextMeshProの参照

    [SerializeField] GameObject flashLightSystem;
    [SerializeField] GameObject flashlightTutorial;
    private TutorialManager tutorialManager;

    #endregion

    public Material bloodMaterial; // 血のマテリアル
    public float fadeDuration = 1.5f; // フェードアウトの時間
    private bool hasSponge = false;   // スポンジ取得フラグ

    private void Start()
    {
        interactTextComponent = interactText.GetComponent<TextMeshProUGUI>();

        if (interactTextComponent == null)
        {
            Debug.LogError("interactTextにTextMeshProUGUIコンポーネントが見つかりません！");
        }

        tutorialManager = flashlightTutorial.GetComponent<TutorialManager>();

        // 🩸 初期化：血の透明度を100%に強制設定
        if (bloodMaterial != null)
        {
            Color color = bloodMaterial.color;
            color.a = 1f;  // 不透明に初期化
            bloodMaterial.color = color;
        }
    }

    private void Update()
    {
        // レイキャストでアイテムを検出
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, itemLayer))
        {
            GameObject hitItem = hit.collider.gameObject;

            if (hitItem.CompareTag("Item"))
            {
                interactTextComponent.text = $"取る";
                interactText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                {
                    PickupItem(hitItem);
                    itemDisplay.ToggleItemDisplay();
                    inventory.UpdateInventoryUI();
                }
            }
        }
        else
        {
            interactText.SetActive(false);
        }

        // スポンジ取得後に血をクリックした場合
        if (hasSponge && Input.GetMouseButtonDown(0))
        {
            Ray clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(clickRay, out hit, interactDistance))
            {
                if (hit.collider.CompareTag("Blood"))
                {
                    StartCoroutine(FadeOutBlood());
                }
            }
        }
    }

    private void PickupItem(GameObject item)
    {
        PocketItem itemData = itemDataBase.itemList.Find(i => i.item.name == item.name);

        if (itemData != null)
        {
            inventory.AddItem(itemData);

            if (item.name == "Flashlight")
            {
                if (flashLightSystem != null)
                {
                    flashLightSystem.SetActive(true);

                    if (tutorialManager != null)
                    {
                        StartCoroutine(tutorialManager.ShowTutorial());
                    }

                    flashlightTutorial.SetActive(true);
                    flashlightTutorial.transform.GetChild(0).gameObject.SetActive(true);
                }
            }

            if (item.name == "sponge")
            {
                hasSponge = true; // 🧽 スポンジ取得フラグを立てる
            }
        }
        else
        {
            Debug.LogWarning("データベースにこのアイテムが存在しません！");
        }

        Destroy(item);
    }

    private IEnumerator FadeOutBlood()
    {
        float elapsedTime = 0f;
        Color color = bloodMaterial.color;
        float startAlpha = color.a;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            bloodMaterial.color = color;
            yield return null;
        }

        color.a = 0f;
        bloodMaterial.color = color;
    }
}
