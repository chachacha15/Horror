using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // URP用

public class BathTub : MonoBehaviour
{
    [SerializeField] private List<DecalProjector> bloodDecals; // 複数の Decal Projector を管理
    [SerializeField] private float fadeDuration = 2f; // フェードアウト時間
    private bool isCleaning = false;

    private ItemChecker itemChecker; // ItemChecker の参照

    private void Start()
    {
        // シーン内の ItemChecker を探して取得
        itemChecker = FindObjectOfType<ItemChecker>();
    }

    private void OnMouseDown()
    {
        // スポンジを持っている場合のみフェードアウト
        if (itemChecker != null && itemChecker.hasSponge && !isCleaning)
        {
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
