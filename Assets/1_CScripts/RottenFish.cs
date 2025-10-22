using UnityEngine;

/// <summary>
/// 腐った魚アイテムの振る舞いを定義するスクリプト。
/// 皿の上に設置され、猫を呼び寄せる役割を持つ。
/// </summary>
public class RottenFish : MonoBehaviour
{
    // 配置されたかどうかを示すフラグ
    private bool isPlaced = false;

    // プレイヤーがアイテムを「使用」した際の処理を想定
    // 実際にアイテムを地面に置く/ワールドに出現させる処理は、プレイヤー側のスクリプトが行います

    /// <summary>
    /// 魚が**有効な設置場所（皿など）**に配置されたときに、外部から呼び出されるメソッド。
    /// </summary>
    /// <param name="placementPosition">魚が配置されたワールド座標。</param>
    public void OnFishPlaced(Vector3 placementPosition)
    {
        if (isPlaced) return;

        // 魚の見た目やコリジョンを「設置済み」の状態に切り替える処理
        isPlaced = true;

        // 物理演算の影響を受けないようにする（Rigidbodyがあれば）
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        // 配置座標へ移動（プレイヤー側から渡された座標）
        transform.position = placementPosition;

        // 回転をリセットして皿に平らに置かれたように見せる
        transform.rotation = Quaternion.identity;

        // --- 猫への通知 ---

        // シーン上のCatWanderコンポーネントを持つ猫を探す
        // 注意：猫が複数いる場合は、FindObjectOfTypeではなく、より特定の検索方法が必要です。
        CatWander cat = FindObjectOfType<CatWander>();

        if (cat != null)
        {
            // 猫に魚の餌付け場所を通知し、「Eating」状態に移行させる
            cat.SetTargetFish(placementPosition);
            UnityEngine.Debug.Log("腐った魚が設置されました。猫がおびき寄せられます。");
        }
        else
        {
            UnityEngine.Debug.LogWarning("腐った魚が設置されましたが、猫（CatWanderスクリプト）が見つかりません。");
        }
    }
}