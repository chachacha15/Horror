using System;
using UnityEngine;
using UnityEngine.UI;

public struct SoundEvent
{
    public Vector3 Position;
    public float Radius;
    public string Tag;       // 例: "Dash", "Door"

    public SoundEvent(Vector3 pos, float radius, string tag = null)
    {
        Position = pos; Radius = radius; Tag = tag;
    }
}

public class SoundEventManager : MonoBehaviour
{
    public static SoundEventManager Instance { get; private set; }
    public event Action<SoundEvent> OnSoundEmitted;

    public Image noiseImage; // ノイズ
    public float normalPixelAmount = 150f; // ノイズ画像のスタートピクセル
    public float targetPixelsAmount = 50f; // ノイズ画像の目標ピクセル
    public float lowerPixelsSpeed = 150f;  // ノイズアニメーションスピード
    public bool isPlayingNoise = false; // ノイズ演出中かどうか  

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>音イベントを発火する。tag は敵の反応を絞り込むのに使える。</summary>
    public static void Emit(Vector3 position, float radius, string tag = null)
    {
        if (Instance == null) return;
        Instance.OnSoundEmitted?.Invoke(new SoundEvent(position, radius, tag));
    }
}
