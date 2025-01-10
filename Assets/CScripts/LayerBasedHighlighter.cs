using UnityEngine;

public class NearbyItemHighlighter : MonoBehaviour
{
    public float interactDistance = 3f;       // 強調する最大距離
    private GameObject currentHighlightedItem; // 現在強調されているアイテム
    private int defaultLayer;                 // 元のレイヤーを保存

    public string highlightLayer = "HighLight"; // 強調用レイヤー名

    private void Update()
    {
        HighlightNearbyItems();
    }

    private void HighlightNearbyItems()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, interactDistance);

        foreach (Collider collider in nearbyColliders)
        {
            GameObject item = collider.gameObject;

            if (item.CompareTag("Item")) // Itemタグを持つオブジェクトを検出
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

    private void ApplyHighlight(GameObject item)
    {
        currentHighlightedItem = item;
        defaultLayer = item.layer; // 元のレイヤーを保存

        // 強調用レイヤーを取得
        int highlightLayerIndex = LayerMask.NameToLayer(highlightLayer);

        if (highlightLayerIndex == -1)
        {
            Debug.LogError($"レイヤー '{highlightLayer}' が存在しません。Unityのレイヤー設定を確認してください。");
            return;
        }

        // 強調用レイヤーに変更
        item.layer = highlightLayerIndex;
    }

    private void ClearHighlight()
    {
        if (currentHighlightedItem != null)
        {
            // 元のレイヤーに戻す
            currentHighlightedItem.layer = defaultLayer;
            currentHighlightedItem = null;
        }
    }
}
