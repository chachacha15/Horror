using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;

/// <summary>
/// 敵に見つかったときなどにジャンプスケア演出をするクラス
/// </summary>
public class ShakeCamera : MonoBehaviour
{

    #region Variables
    public static ShakeCamera Instance { get; private set; }

    // カメラシェイク用
    private Vector3 originalPosition;
    public bool isShaking = false;
    private float shakeAmount = 0f;
    private float shakeDuration = 0f;

    // シェイク時の演出用（画像差し込みなど）
    public Image scaryImage;
    public float fadeDuration = 0.2f;

    [Header("FilmGrain用")]
    public Volume volume;            // PostProcessVolumeの参照
    private FilmGrain filmGrain;     // FilmGrainエフェクトの参照
    public Texture customTexture;           // カスタム用テクスチャ
    public float customTextureAlpha = 0.1f; // カスタム用テクスチャの表示するときの透明度
    public float large01TextureAlpha = 1.0f; // Large01テクスチャの表示するときの透明度


    #endregion


    #region Methods

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // 初期設定
        SetImageAlpha(0f);
        originalPosition = transform.localPosition;

        // PostProcessVolumeからFilmGrainエフェクトを取得
        if (volume.profile.TryGet<FilmGrain>(out filmGrain))
        {
            SetGrainTypeToLarge1();
        }
    }

    void Update()
    {
        if (isShaking)
        {
            if (shakeDuration > 0)
            {
                // 揺れの強さをランダムに決定
                transform.localPosition = originalPosition + (Vector3)Random.insideUnitSphere * shakeAmount;

                // 時間が経過するごとに減少
                shakeDuration -= Time.deltaTime;
            }
            else
            {
                // 揺れが終了したら元の位置に戻す
                transform.localPosition = originalPosition;
                isShaking = false;
            }
        }
    }

    /// <summary>
    /// 呼ばれると大きくカメラのシェイクを開始するメソッド
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="duration"></param>
    public void Shake(float amount, float duration)
    {
        shakeAmount = amount;
        shakeDuration = duration;
        isShaking = true;

        SetGrainTexture(customTexture); // FilmGrainでタイルのような画像を見せる
        StartCoroutine(FadeImage(0.05f)); // 普通にImageとして画像を見せる
    }

    /// <summary>
    /// 画像を一瞬だけ映すコルーチン
    /// </summary>
    /// <param name="targetAlpha"></param>
    /// <returns></returns>
    private IEnumerator FadeImage(float targetAlpha)
    {
        float elapsedTime = 0f;
        float startAlpha = scaryImage.color.a;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            SetImageAlpha(alpha);
            yield return null;
        }

        SetImageAlpha(targetAlpha); // 最終的にターゲットのアルファ値に設定

        elapsedTime = 0f;
        targetAlpha = scaryImage.color.a;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(targetAlpha, startAlpha, elapsedTime / fadeDuration);
            SetImageAlpha(alpha);
            yield return null;
        }


        SetImageAlpha(startAlpha); // 最終的に最初のアルファ値に設定
        SetGrainTypeToLarge1();    // 同じタイミングで、FilmGrainの方も元の設定に戻す
    }

    /// <summary>
    /// 透明度を設定するメソッド
    /// </summary>
    /// <param name="alpha"></param>
    private void SetImageAlpha(float alpha)
    {
        Color newColor = scaryImage.color;
        newColor.a = alpha;
        scaryImage.color = newColor;
    }


    /// <summary>
    /// FilmGrainのテクスチャをカスタムに変更するメソッド
    /// </summary>
    /// <param name="texture"></param>
    public void SetGrainTexture(Texture texture)
    {
        if (filmGrain != null)
        {
            // FilmGrainのテクスチャを設定
            filmGrain.intensity.value = customTextureAlpha;
            filmGrain.type.value = FilmGrainLookup.Custom;
            filmGrain.texture.overrideState = true;
            filmGrain.texture.value = texture;

        }
    }

    /// <summary>
    /// FilmGrainのタイプをLarge01に戻すメソッド
    /// </summary>
    public void SetGrainTypeToLarge1()
    {
        if (filmGrain != null)
        {
            // FilmGrainのタイプをLarge01に戻す
            filmGrain.intensity.value = large01TextureAlpha;
            filmGrain.type.value = FilmGrainLookup.Large01;

        }

    }
    #endregion

}
