using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class foundmusic : MonoBehaviour
{
    private GhostAI ghostAI; // GhostAIスクリプトへの参照
    private AudioSource audioSource; // 音楽再生用のAudioSource
    private bool isPlaying = false; // 音楽再生中かどうかを管理

    // Start is called before the first frame update
    void Start()
    {
        // Ghost1の親オブジェクトからGhostAIスクリプトを取得
        Transform ghost1Transform = transform.parent; // "Ghost1" の Transform
        if (ghost1Transform != null)
        {
            ghostAI = ghost1Transform.GetComponent<GhostAI>();
            if (ghostAI == null)
            {
               
            }
        }
        else
        {
          
        }

        // 子オブジェクト "Time_limit" を探してAudioSourceを取得
        Transform timeLimitObject = transform.Find("Time_limit");
        if (timeLimitObject != null)
        {
            audioSource = timeLimitObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
              
            }
        }
        else
        {
           
        }

        // 音楽のループ設定
        if (audioSource != null)
        {
            audioSource.loop = true; // 音楽をループ再生
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ghostAI != null && audioSource != null)
        {
            if (ghostAI.sensor) // sensorがtrueなら音楽を再生
            {
                if (!isPlaying)
                {
                    audioSource.Play();
                    isPlaying = true;
                }
            }
            else // sensorがfalseなら音楽を停止
            {
                if (isPlaying)
                {
                    audioSource.Stop();
                    isPlaying = false;
                }
            }
        }
    }
}
