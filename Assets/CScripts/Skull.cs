using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class Skull : MonoBehaviour
{
    #region Variables
    //本物かどうか
    private bool isReal = false;

    // 顎アニメーション用
    public Transform jaw; // 顎のオブジェクト
    private bool isJawOpen = false;
    public float jawOpenSpeed = 1.5f; // 顎が開くスピード（秒）

    // 生成アイテム用
    public Transform itemSpawnPoint; // アイテムを生成する位置
    public List<GameObject> itemPrefabs; // 口から出るアイテムのリスト

    // その他・他クラス
    private bool isLookingSkull = false;

    private SkullManager manager;
    private CameraSwitcher cameraSwitcher;

    #endregion

    private void Start()
    {
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        manager = FindObjectOfType<SkullManager>();
    }

    private void Update()
    {
        if (isLookingSkull)
        {
            cameraSwitcher.ClosshairAnimation(10f, 500f, 0.5f, cameraSwitcher.crosshairRectTransform, isLookingSkull);
        }
        else if(!isLookingSkull)
        {
            cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingSkull);
        }
    }

    public void SetReal(bool real)
    {
        isReal = real;
    }

    private void OnMouseEnter()
    {
        // カーソルを合わせたとき
        isLookingSkull = true;
        Debug.Log($"{gameObject.name} にカーソルを合わせた");
    }

    private void OnMouseExit()
    {
        // カーソルが外れたときの処理
        isLookingSkull=false;

    }

    private void OnMouseDown()
    {
        // 骸骨をクリックしたら管理スクリプトに通知
        manager.ShowConfirmationUI(this);
    }

    public void RevealReal()
    {
        Debug.Log("本物の骸骨を見破った！ 顎が外れる");

        // 顎を外す（例: Y軸に回転）
        if (jaw != null && !isJawOpen)
        {
            isJawOpen = true;
            StartCoroutine(OpenJawSmoothly()); // ゆっくり開くコルーチンを開始
        }
    }

    private IEnumerator OpenJawSmoothly()
    {
        float elapsedTime = 0f;
        Quaternion startRotation = jaw.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0, -70, 50); // X軸方向に開く



        while (elapsedTime < jawOpenSpeed)
        {
            elapsedTime += Time.deltaTime;
            jaw.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / jawOpenSpeed);
            yield return null;
        }

        jaw.localRotation = targetRotation; // 最終位置をセット
        SpawnItem();

    }

    private void SpawnItem()
    {
        if (itemPrefabs.Count == 0)
        {
            Debug.LogWarning("アイテムリストが空です！");
            return;
        }

        // ランダムなアイテムを選択
        GameObject selectedItem = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

        // アイテムを生成
        GameObject spawnedItem = Instantiate(selectedItem, itemSpawnPoint);

    }

    public void DisableColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(); // 自分と子オブジェクトのコライダーをすべて取得
        foreach (Collider col in colliders)
        {
            col.enabled = false; // コライダーを無効化
        }
    }

}
