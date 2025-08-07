using UnityEngine;

public class BaitStand : MonoBehaviour
{
    // 餌が置かれたかどうか
    public bool HasBait { get; private set; } = false;

    // 餌のオブジェクトを非表示にするため
    [SerializeField] private GameObject fishObject;

    // 餌を置くパブリックメソッド
    public void PlaceBait()
    {
        HasBait = true;
        // 餌を置いた後、プレイヤーから見えないようにする
        if (fishObject != null)
        {
            fishObject.SetActive(true);
        }
    }
}