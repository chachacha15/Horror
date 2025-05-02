using Unity.VisualScripting;
using UnityEngine;

public class NearbyItemHighlighter : MonoBehaviour
{
    public static NearbyItemHighlighter Instance;
    public float interactDistance = 3f;       // 強調する最大距離
    public GameObject currentHighlightedItem; // 現在強調されているアイテム
    private int defaultLayer;                 // 元のレイヤーを保存

    public string highlightLayer = "HighLight"; // 強調用レイヤー名


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

    private void Update()
    {
        //HighlightNearbyItems();
    }

    public void HighlightNearbyItems()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, interactDistance);

        foreach (Collider collider in nearbyColliders)
        {
            GameObject item = collider.gameObject;

            if (item.CompareTag("Interactable") || item.CompareTag("Battery")) // Itemタグを持つオブジェクトを検出
            {
                if (currentHighlightedItem != item)
                {
                    ClearHighlight(); // 既存のハイライトを解除
                    ApplyHighlight(item); // 新しいアイテムを強調
                }

                return; // 最初に見つけたItemタグのオブジェクトを強調
            }
        }

        ClearHighlight(); // 何も見つからなかった場合は強調を解除
    }

    public void ApplyHighlight(GameObject item)
    {
        // このオブジェクトがメッシュを持っている場合、そのままハイライト
        // 持っていなかった場合、子供のオブジェクトをハイライトするようにする
        if (item.GetComponent<MeshFilter>()) currentHighlightedItem = item;
        else currentHighlightedItem = item.transform.GetChild(0).gameObject;

        defaultLayer = currentHighlightedItem.layer; // 元のレイヤーを保存

        // 強調用レイヤーを取得
        int highlightLayerIndex = LayerMask.NameToLayer(highlightLayer);

        if (highlightLayerIndex == -1)
        {
            Debug.LogError($"レイヤー '{highlightLayer}' が存在しません。Unityのレイヤー設定を確認してください。");
            return;
        }

        // 強調用レイヤーに変更
         currentHighlightedItem.layer = highlightLayerIndex;
    }

    public void ClearHighlight()
    {
        if (currentHighlightedItem != null)
        {
            // 元のレイヤーに戻す
            currentHighlightedItem.layer = defaultLayer;
            currentHighlightedItem = null;
        }
    }
}
