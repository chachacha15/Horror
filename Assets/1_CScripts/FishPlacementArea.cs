using UnityEngine;

/// <summary>
/// 魚アイテムを設置できるエリア（皿など）を定義するスクリプト。
/// </summary>
public class FishPlacementArea : MonoBehaviour, IInteractable
{
    [Tooltip("魚が設置される正確なワールド座標。皿の表面を指定。")]
    [SerializeField] private Transform placementPoint;

    // 既に魚が置かれているかどうかのフラグ
    private bool isOccupied = false;

    // IInteractable の実装
    public string GetInteractText()
    {
        // プレイヤーのインベントリに魚があるかどうかの判定が必要です
        // 例: return PlayerInventory.Instance.HasItem("RottenFish") ? "魚を設置する" : "空の皿だ";

        // 今回は簡略化し、設置可能な状態のみを表示
        return isOccupied ? "魚が置かれている" : "魚を設置する";
    }

    public bool ShowInteractText => !isOccupied; // 魚が置かれていない時だけ表示
    public bool ActivateCrosshair => true;

    // プレイヤーがこの皿にインタラクト（例: Eキーを押す）したときの処理
    public void Interact(GameObject targetObject)
    {
        if (isOccupied) return;

        // --- プレイヤーのインベントリから魚を使う処理をシミュレート ---

        // 1. 魚を持っているか確認（ここでは成功と仮定）
        // bool hasFish = PlayerInventory.Instance.RemoveItem("RottenFish");
        // if (!hasFish) return; 

        // 2. 魚のゲームオブジェクトをシーンに生成する（またはプールから取り出す）
        // ここでは、既にシーンにある魚を操作するものと仮定し、魚オブジェクトを検索する
        // 実際のゲームではPrefabからInstantiateするのが一般的です


        RottenFish fish = FindObjectOfType<RottenFish>(); // 例として検索

        if (fish != null)
        {
            // 魚がワールドに存在し、インタラクトする魚が特定できた場合
            isOccupied = true;

            // 魚のロジックを呼び出し、魚をこの皿の上に配置させる
            fish.OnFishPlaced(placementPoint.position);

            // 魚のゲームオブジェクトを、皿の子にすると管理しやすくなります
            fish.transform.SetParent(this.transform);
        }
    }

    // 配置ポイントを設定するためのプロパティ（Inspectorから設定）
    private void OnValidate()
    {
        if (placementPoint == null)
        {
            // 皿の表面に設置ポイント用の空のGameObjectを作成することを推奨します
            UnityEngine.Debug.LogWarning("FishPlacementArea: placementPointが設定されていません。皿の表面に設置ポイントとなるTransformを設定してください。");
        }
    }
}