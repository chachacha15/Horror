using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foundmusic : MonoBehaviour
{
    private GhostAI ghostAI; // GhostAIスクリプトへの参照
    private AudioSource bgmAudioSource; // BGM再生用のAudioSource
    private AudioSource sfxAudioSource; // 効果音再生用のAudioSource
    private bool isPlayingBGM = false; // BGM再生中かどうかを管理
    private bool hasPlayedSFX = false; // 効果音を再生したかどうかを管理

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


    // Update is called once per frame
    void Update()
    {
        if (ghostAI != null && bgmAudioSource != null && sfxAudioSource != null)
        {

            if (ghostAI.currentState == GhostAI.State.Chase) // currentStateがchaseならBGMを再生
            {

                if (!isPlayingBGM)
                {

                    bgmAudioSource.Play();
                    isPlayingBGM = true;
                }

                if (!hasPlayedSFX) // 効果音がまだ再生されていない場合
                {
                    sfxAudioSource.PlayOneShot(sfxAudioSource.clip); // 効果音を一度だけ再生
                    hasPlayedSFX = true;
                }
            }
            else // sensorがfalseならBGMを停止
            {
                if (isPlayingBGM)
                {
                    bgmAudioSource.Stop();
                    isPlayingBGM = false;
                }

                hasPlayedSFX = false; // 効果音の再生状態をリセット
            }
        }
    }
}
