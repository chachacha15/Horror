/*
using System.Collections.Generic;
using UnityEngine;

public class ArrowHintSpawner : MonoBehaviour
{
    [Header("矢印プレハブ")]
    public GameObject leftArrowPrefab;
    public GameObject rightArrowPrefab;

    [Header("設置位置（8個）")]
    public Transform[] spawnPoints; // 8個の固定位置

    [HideInInspector]
    public List<string> generatedAnswerSequence = new List<string>();

    void Start()
    {
        GenerateRandomArrows();
    }

    void GenerateRandomArrows()
    {
        if (spawnPoints.Length != 8)
        {
            Debug.LogError("設置ポイントは8個必要です！");
            return;
        }

        // 左4枚・右4枚
        List<string> arrowPool = new List<string>()
        {
            "L", "L", "L", "L",
            "R", "R", "R", "R"
        };

        Shuffle(arrowPool);
        generatedAnswerSequence.Clear();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            string arrow = arrowPool[i];
            GameObject prefabToSpawn = (arrow == "L") ? leftArrowPrefab : rightArrowPrefab;

            Instantiate(prefabToSpawn, spawnPoints[i].position, spawnPoints[i].rotation, spawnPoints[i].parent);
            generatedAnswerSequence.Add(arrow);
        }

        Debug.Log("生成されたヒント配列: " + string.Join(",", generatedAnswerSequence));
    }

    // フィッシャー・イェーツシャッフル
    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
*/

using System.Collections.Generic;
using UnityEngine;
public class ArrowHintSpawner : MonoBehaviour
{
    public GameObject leftArrowPrefab;
    public GameObject rightArrowPrefab;
    public Transform[] spawnPoints;

    public List<string> generatedAnswerSequence = new List<string>();

    void Start()
    {
        GenerateHints();
    }

    void GenerateHints()
    {
        List<string> arrows = new List<string>() { "L", "L", "L", "L", "R", "R", "R", "R" };
        Shuffle(arrows);
        generatedAnswerSequence.Clear();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject prefab = (arrows[i] == "L") ? leftArrowPrefab : rightArrowPrefab;
            GameObject obj = Instantiate(prefab, spawnPoints[i].position, spawnPoints[i].rotation);
            GameObject obj = Instantiate(prefab, spawnPoints[i].position, spawnPoints[i].rotation);
            obj.transform.localScale = new Vector3(3f, 3f, 3f); // ← 好きなサイズに

            // 必要ならここで補正
            // obj.transform.Rotate(0f, 180f, 0f);

            generatedAnswerSequence.Add(arrows[i]);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
