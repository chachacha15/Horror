using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditorInternal.VersionControl.ListControl;

public class foundmusic : MonoBehaviour
{
    #region Variables
    public AudioClip SE;

    private GhostAI ghostAI; // GhostAIスクリプトへの参照
    private AudioSource bgmAudioSource; // BGM再生用のAudioSource
    private AudioSource sfxAudioSource; // 効果音再生用のAudioSource
    private bool isPlayingBGM = false; // BGM再生中かどうかを管理
    private bool hasPlayedSFX = false; // 効果音を再生したかどうかを管理

    // 前回の状態を記録する変数（初期状態は Patrol としておく）
    private State lastState = State.Patrol;

    public float bgmStopDelay = 10f;    // BGM停止までの遅延時間（秒）
    private float bgmStopTimer = 0f;   // タイマー変数

    public float fadeOutDuration = 2f;  // フェードアウトにかける秒数
    private Coroutine fadeOutCoroutine; // フェードアウトのコルーチンを保持


    #endregion

    #region Methods

    // Start is called before the first frame update
    void Start()
    {
        //  Ghost1 から GhostAI スクリプトを取得
        //if (ghost1Transform != null)
        {

            ghostAI = GetComponent<GhostAI>();
            if (ghostAI != null)
            {
                Debug.Log("GhostAI スクリプトが正常に取得されました");
            }
            else
            {
                Debug.LogWarning("GhostAI スクリプトが見つかりません");
            }
        }


        // 子オブジェクト "EnemyBGM" を探して BGM 用 AudioSource を取得
        Transform timeLimitObject = transform.Find("EnemyBGM");
        if (timeLimitObject != null)
        {
            bgmAudioSource = timeLimitObject.GetComponent<AudioSource>();
            if (bgmAudioSource != null)
            {
                Debug.Log("BGM用のAudioSourceが正常に取得されました");
            }
            else
            {
                Debug.LogWarning("EnemyBGM オブジェクトに AudioSource が見つかりません");
            }
        }


        // 子オブジェクト "FoundSound" を探して効果音用 AudioSource を取得
        Transform foundObject = transform.Find("FoundSound");
        if (foundObject != null)
        {
            sfxAudioSource = foundObject.GetComponent<AudioSource>();
            if (sfxAudioSource != null)
            {
                Debug.Log("効果音用のAudioSourceが正常に取得されました");
            }
            else
            {
                Debug.LogWarning("found オブジェクトに AudioSource が見つかりません");
            }
        }


        // BGMのループ設定
        if (bgmAudioSource != null)
        {
            bgmAudioSource.loop = true; // BGMをループ再生
        }
    }


    void Update()
    {
        if (ghostAI != null && bgmAudioSource != null && sfxAudioSource != null)
        {
            // もし現在の状態が Chase で、前回の状態が Patrol なら
            if (ghostAI.currentState == State.Chase && lastState == State.Patrol)
            {
                // 特別な SE を一回だけ再生
                sfxAudioSource.PlayOneShot(SE);  // specialSE は Inspector から割り当てる AudioClip
            }

            // ここで lastState を更新
            lastState = ghostAI.currentState;

            if (ghostAI.currentState == State.Chase)
            {
                // Chase 状態ならフェードアウトが走っていたら止めて、通常再生する
                if (fadeOutCoroutine != null)
                {
                    StopCoroutine(fadeOutCoroutine);
                    fadeOutCoroutine = null;
                    bgmAudioSource.volume = 1f;  // 元の音量にリセット（仮に1が元の値）
                }

                if (!isPlayingBGM)
                {
                    bgmAudioSource.Play();
                    isPlayingBGM = true;
                }

                if (!hasPlayedSFX)
                {
                    sfxAudioSource.PlayOneShot(sfxAudioSource.clip);
                    hasPlayedSFX = true;
                }
            }
            else  // Chase 状態でない場合
            {
                if (isPlayingBGM && fadeOutCoroutine == null)
                {
                    // フェードアウトを開始する
                    fadeOutCoroutine = StartCoroutine(FadeOutBGM());
                }

                hasPlayedSFX = false;
            }
        }
    }


    /// <summary>
    /// BGMをfadeOutDuration秒かけて徐々にフェードアウトさせるコルーチン
    /// </summary>
    private IEnumerator FadeOutBGM()
    {
        float startVolume = bgmAudioSource.volume;
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutDuration);
            yield return null;
        }

        bgmAudioSource.volume = 0f;
        bgmAudioSource.Stop();
        isPlayingBGM = false;
        fadeOutCoroutine = null;

        // 次回再生のために音量を元に戻す（必要に応じて）
        bgmAudioSource.volume = startVolume;
    }

    #endregion
}
