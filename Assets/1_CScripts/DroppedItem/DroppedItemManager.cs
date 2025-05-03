using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DroppedItemManager : MonoBehaviour
{
    public static DroppedItemManager Instance; // シングルトンインスタンス

    [SerializeField] private ItemDataBase itemDataBase; // アイテムデータ

     List<PocketItem> itemList = new List<PocketItem>(); // 生成するアイテムリスト

    [Header("生成する位置をここに")]
    public List<Transform> spawnPoints;  // スポーン位置リスト
    private List<Transform> spawnedPositions = new List<Transform>(); // 生成された位置を追跡


    private bool importantItemSpawned = false;  // 重要なアイテムがすでに生成されたかどうかを追跡

    #region Methods

    /// <summary>
    /// シングルトン用
    /// </summary>
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


    void Start()
    {
        foreach (var item in itemDataBase.itemList)
            itemList.Add(item);

        StartCoroutine(WaitSeconds());
    }

    private IEnumerator WaitSeconds()
    {
   
        yield return new WaitForSeconds(1);
        SpawnRandomItem();

    }



    /// <summary>
    /// ランダムでアイテムを生成する
    /// </summary>
    void SpawnRandomItem()
    {
        // 重要なアイテムを生成するフラグが立っていなければ、重要なアイテムを生成
        //if (!importantItemSpawned)
        {
            SpawnImportantItem();
        }
        
        {
            // 重要なアイテムが生成された後は、重みを考慮してアイテムをランダムに選ぶ
            SpawnWithWeight();
        }
    }

    // 重要なアイテムを一度だけ生成する
    void SpawnImportantItem()
    {

        // 重要なアイテムを選ぶ
        foreach (var item in itemList)
        {

            if (item.isImportant)
            {
                int randomSpawnPointIndex = GetRandomSpawnPoint();

                // 重要なアイテムを生成
                GameObject itemInstance = Instantiate(item.item, spawnPoints[randomSpawnPointIndex].position, Quaternion.identity, spawnPoints[randomSpawnPointIndex]);
                itemInstance.name = item.item.name;

                // 生成した位置を記録
                spawnedPositions.Add(spawnPoints[randomSpawnPointIndex]);

            }
        }

        // 条件に一致するアイテムを一度に削除
        itemList.RemoveAll(item => item.isImportant);
    }

    // 重みを使ってアイテムをランダムに生成する
    void SpawnWithWeight()
    {
        int totalWeight = 0;

        // アイテムの重みを合計
        foreach (var item in itemList)
        {
            if (item.weight > 0)  // 重みが0より大きければ、生成対象
            {
                totalWeight += item.weight;
            }
        }

        if (totalWeight <= 0)
        {
            Debug.LogWarning("No items to spawn! Check your weights.");
            return;
        }

        // 重みを基にランダムにアイテムを選択
        int randomWeight = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;
        int selectedItemIndex = 0;

        // アイテムを重みで選択
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemDataBase.itemList[i].weight <= 0) continue;  // 重みが0の場合はスキップ

            cumulativeWeight += itemList[i].weight;
            if (randomWeight < cumulativeWeight)
            {
                selectedItemIndex = i;
                break;
            }

        }

        // アイテムをスポーン
        int randomSpawnPointIndex = GetRandomSpawnPoint();
        GameObject itemInstance = Instantiate(itemList[selectedItemIndex].item, spawnPoints[randomSpawnPointIndex].position, Quaternion.identity, spawnPoints[randomSpawnPointIndex]);
        itemInstance.name = itemList[selectedItemIndex].item.name;

        // 生成した位置を記録
        spawnedPositions.Add(spawnPoints[randomSpawnPointIndex]);

    }

    // スポーンする位置をランダムに選択し、すでに生成した位置は選ばない
    int GetRandomSpawnPoint()
    {
        List<int> availableIndexes = new List<int>();

        // 利用可能な位置をリストアップ
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (!spawnedPositions.Contains(spawnPoints[i]))
            {
                availableIndexes.Add(i);
            }
        }

        if (availableIndexes.Count == 0)
        {
            Debug.LogWarning("スポーンする位置がありません！");
            return 0;
        }

        // 利用可能な位置からランダムに選ぶ
        int randomIndex = Random.Range(0, availableIndexes.Count);
        return availableIndexes[randomIndex];
    }

    #endregion
}
