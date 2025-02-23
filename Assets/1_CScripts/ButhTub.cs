/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // URP用

public class BathTub : MonoBehaviour
{
    [SerializeField] private List<DecalProjector> bloodDecals; // 複数の Decal Projector を管理
    [SerializeField] private float fadeDuration = 2f; // フェードアウト時間
    [SerializeField] private DecalProjector answerDecal; // 答えのデカール
    private Inventory inventory;
    private bool isCleaning = false;

    private ItemChecker itemChecker; // ItemChecker の参照

    private void Start()
    {
        // シーン内の ItemChecker を探して取得
        itemChecker = FindObjectOfType<ItemChecker>();
        inventory = FindObjectOfType<Inventory>();
    }

    private void OnMouseDown()
    {
        // スポンジを持っている場合のみフェードアウト
        if (itemChecker != null && inventory.selectedItem != null && inventory.selectedItem.item.name == "sponge" && !isCleaning)
        {
            inventory.RemoveHeldItem();
            StartCoroutine(FadeOutBlood());
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

            for (int i = 0; i < bloodDecals.Count; i++)
            {
                if (bloodDecals[i] != null)
                {
                    bloodDecals[i].fadeFactor = Mathf.Lerp(startOpacities[i], 0f, t);
                }
            }
            answerDecal.fadeFactor = Mathf.Lerp(0f,1f, t);

            yield return null;
        }

        // フェードアウト後、すべてのデカールを無効化
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
*/

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
    [SerializeField] private float numberFadeDuration = 2f; // 数字のフェードイン時間
    private Inventory inventory;
    private bool isCleaning = false;

    private ItemChecker itemChecker; // ItemChecker の参照

    private void Start()
    {
        // シーン内の ItemChecker を探して取得
        itemChecker = FindObjectOfType<ItemChecker>();
        inventory = FindObjectOfType<Inventory>();

        // 初期状態では数字を透明にする
        if (hiddenNumberText != null)
        {
            Color color = hiddenNumberText.color;
            color.a = 0;
            hiddenNumberText.color = color;
        }
    }

    private void OnMouseDown()
    {
        // スポンジを持っている場合のみフェードアウト
        if (itemChecker != null && inventory.selectedItem != null && inventory.selectedItem.item.name == "sponge" && !isCleaning)
        {
            inventory.RemoveHeldItem(); // スポンジを消費
            StartCoroutine(FadeOutBloodAndShowNumber());
        }
    }

    private IEnumerator FadeOutBloodAndShowNumber()
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

        // フェードアウト後、すべての血のデカールを非表示
        foreach (var decal in bloodDecals)
        {
            if (decal != null)
            {
                decal.gameObject.SetActive(false);
            }
        }

        // 数字をランダムに決定
        int randomNumber = Random.Range(1000, 9999); // 4桁のランダムな数字
        hiddenNumberText.text = randomNumber.ToString();

        // 数字のフェードイン
        StartCoroutine(FadeInNumber());

        isCleaning = false;
    }

    private IEnumerator FadeInNumber()
    {
        float elapsedTime = 0f;
        Color color = hiddenNumberText.color;

        while (elapsedTime < numberFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / numberFadeDuration);
            hiddenNumberText.color = color;
            yield return null;
        }
    }
}
