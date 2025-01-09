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
        //  Ghost1 ���� GhostAI �X�N���v�g���擾
        //if (ghost1Transform != null)
        {

            ghostAI = GetComponent<GhostAI>();
            if (ghostAI != null)
            {
                Debug.Log("GhostAI �X�N���v�g������Ɏ擾����܂���");
            }
            else
            {
                Debug.LogWarning("GhostAI �X�N���v�g��������܂���");
            }
        }
        

        // �q�I�u�W�F�N�g "EnemyBGM" ��T���� BGM �p AudioSource ���擾
        Transform timeLimitObject = transform.Find("EnemyBGM");
        if (timeLimitObject != null)
        {
            bgmAudioSource = timeLimitObject.GetComponent<AudioSource>();
            if (bgmAudioSource != null)
            {
                Debug.Log("BGM�p��AudioSource������Ɏ擾����܂���");
            }
            else
            {
                Debug.LogWarning("EnemyBGM �I�u�W�F�N�g�� AudioSource ��������܂���");
            }
        }
        

        // �q�I�u�W�F�N�g "FoundSound" ��T���Č��ʉ��p AudioSource ���擾
        Transform foundObject = transform.Find("FoundSound");
        if (foundObject != null)
        {
            sfxAudioSource = foundObject.GetComponent<AudioSource>();
            if (sfxAudioSource != null)
            {
                Debug.Log("���ʉ��p��AudioSource������Ɏ擾����܂���");
            }
            else
            {
                Debug.LogWarning("found �I�u�W�F�N�g�� AudioSource ��������܂���");
            }
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

            if (ghostAI.currentState == GhostAI.State.Chase) // currentState��chase�Ȃ�BGM���Đ�
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
