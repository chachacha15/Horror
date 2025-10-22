using UnityEngine;

public class BaitStand : MonoBehaviour
{
    // 餌が置かれたかどうか
    public bool HasBait { get; private set; } = false;

    // 餌のオブジェクト（子オブジェクトに設定し、RottenFish.csを持つ）
    [SerializeField] private GameObject fishObject;

    // 魚のスクリプトをキャッシュしておく
    private RottenFish fishScript;

    void Start()
    {
        if (fishObject != null)
        {


            // 魚のスクリプトを取得しておく
            fishScript = fishObject.GetComponent<RottenFish>();
            if (fishScript == null)
            {
                Debug.Log("BaitStand: fishObjectに RottenFish.cs がアタッチされていません！");
            }
        }
        
    }

    // プレイヤー側のスクリプトから呼び出される
    public void PlaceBait()
    {
        // 既に餌が置かれていたら何もしない
        if (HasBait || fishScript == null)
        {
            return;
        }

        Debug.Log("BaitStand: 餌が設置されました。");
        HasBait = true;

        // 1. 魚のモデルを表示する
        fishObject.SetActive(true);

        // 2. 魚のスクリプトに「設置された」ことを通知し、猫を呼んでもらう
        //    設置場所は、表示した魚自身の場所
        fishScript.OnFishPlaced(fishObject.transform.position);
    }
}