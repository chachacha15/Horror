using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // URP用
using TMPro; // TextMeshPro 用



public class BathTub : MonoBehaviour
{
    [SerializeField] private List<DecalProjector> bloodDecals; // フェードアウトする血の Decal 一覧
    [SerializeField] private float fadeDuration = 2f; // フェード時間
    [SerializeField] private TextMeshProUGUI hiddenNumberText; // 浴槽に表示するランダムな数字

    private Inventory inventory;
    private bool isCleaning = false;

    private ItemChecker itemChecker; // ItemChecker の参照

   

    // 変更点: Safe_Manager への参照を追加
    [SerializeField] private Safe_Manager safeManager;

    // Safe_Manager.cs 内の変数定義の下あたりに追加
    

    private void Start()
    {
        // シーン内の ItemChecker を探して取得
        itemChecker = FindObjectOfType<ItemChecker>();
        inventory = FindObjectOfType<Inventory>();

        // 初期状態では数字を非表示
        if (hiddenNumberText != null)
        {
            hiddenNumberText.gameObject.SetActive(false);
        }

        // 変更点: safeManager が未設定の場合は自動取得（必要に応じて）
        if (safeManager == null)
        {
            safeManager = FindObjectOfType<Safe_Manager>();
        }
    }

    private void OnMouseDown()
    {
        // スポンジを持っている場合のみフェードアウト＆数字表示
        if (itemChecker != null && inventory.selectedItem != null && inventory.selectedItem.item.name == "sponge" && !isCleaning)
        {
            inventory.RemoveHeldItem(); // スポンジを消費
            ShowNumber(); // クリック時に即座に数字を表示
            StartCoroutine(FadeOutBlood());
        }
    }

    private void ShowNumber()
    {
        if (hiddenNumberText != null)
        {
            // 数字をランダムに決定
            int randomNumber = Random.Range(1000, 9999); // 4桁のランダムな数字
            hiddenNumberText.text = randomNumber.ToString();

            // 変更点: Safe_Manager の正解番号を更新する
            if (safeManager != null)
            {
                safeManager.correctCode = randomNumber.ToString();
            }

            // 数字を即座に表示
            hiddenNumberText.gameObject.SetActive(true);
        }
    }

    private IEnumerator FadeOutBlood()
    {
        isCleaning = true;
        float elapsedTime = 0f;

        // 初期の不透明度を取得
        List<float> startOpacities = new List<float>();
        foreach (var decal in bloodDecals)
        {
            startOpacities.Add(decal.fadeFactor);
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            // 血のデカールをフェードアウト
            for (int i = 0; i < bloodDecals.Count; i++)
            {
                if (bloodDecals[i] != null)
                {
                    bloodDecals[i].fadeFactor = Mathf.Lerp(startOpacities[i], 0f, t);
                }
            }

            yield return null;
        }

        // フェードアウト後、すべてのデカールを非表示
        foreach (var decal in bloodDecals)
        {
            if (decal != null)
            {
                decal.gameObject.SetActive(false);
            }
        }

        isCleaning = false;
    }
}
