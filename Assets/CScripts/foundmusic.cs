using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foundmusic : MonoBehaviour
{
    private GhostAI ghostAI; // GhostAI�X�N���v�g�ւ̎Q��
    private AudioSource bgmAudioSource; // BGM�Đ��p��AudioSource
    private AudioSource sfxAudioSource; // ���ʉ��Đ��p��AudioSource
    private bool isPlayingBGM = false; // BGM�Đ������ǂ������Ǘ�
    private bool hasPlayedSFX = false; // ���ʉ����Đ��������ǂ������Ǘ�

    // Start is called before the first frame update
    void Start()
    {
        // �e�I�u�W�F�N�g Ghost1 ���� GhostAI �X�N���v�g���擾
        Transform ghost1Transform = transform.parent; // "Ghost1" �� Transform
        if (ghost1Transform != null)
        {
            ghostAI = ghost1Transform.GetComponent<GhostAI>();
        }

        // �q�I�u�W�F�N�g "Time_limit" ��T���� BGM �p AudioSource ���擾
        Transform timeLimitObject = transform.Find("Time_limit");
        if (timeLimitObject != null)
        {
            bgmAudioSource = timeLimitObject.GetComponent<AudioSource>();
        }

        // �q�I�u�W�F�N�g "found" ��T���Č��ʉ��p AudioSource ���擾
        Transform foundObject = transform.Find("found");
        if (foundObject != null)
        {
            sfxAudioSource = foundObject.GetComponent<AudioSource>();
        }

        // BGM�̃��[�v�ݒ�
        if (bgmAudioSource != null)
        {
            bgmAudioSource.loop = true; // BGM�����[�v�Đ�
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ghostAI != null && bgmAudioSource != null && sfxAudioSource != null)
        {
            if (ghostAI.sensor) // sensor��true�Ȃ�BGM���Đ�
            {
                if (!isPlayingBGM)
                {
                    bgmAudioSource.Play();
                    isPlayingBGM = true;
                }

                if (!hasPlayedSFX) // ���ʉ����܂��Đ�����Ă��Ȃ��ꍇ
                {
                    sfxAudioSource.PlayOneShot(sfxAudioSource.clip); // ���ʉ�����x�����Đ�
                    hasPlayedSFX = true;
                }
            }
            else // sensor��false�Ȃ�BGM���~
            {
                if (isPlayingBGM)
                {
                    bgmAudioSource.Stop();
                    isPlayingBGM = false;
                }

                hasPlayedSFX = false; // ���ʉ��̍Đ���Ԃ����Z�b�g
            }
        }
    }
}
