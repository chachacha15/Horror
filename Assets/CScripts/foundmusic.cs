using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class foundmusic : MonoBehaviour
{
    private GhostAI ghostAI; // GhostAI�X�N���v�g�ւ̎Q��
    private AudioSource audioSource; // ���y�Đ��p��AudioSource
    private bool isPlaying = false; // ���y�Đ������ǂ������Ǘ�

    // Start is called before the first frame update
    void Start()
    {
        // Ghost1�̐e�I�u�W�F�N�g����GhostAI�X�N���v�g���擾
        Transform ghost1Transform = transform.parent; // "Ghost1" �� Transform
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

        // �q�I�u�W�F�N�g "Time_limit" ��T����AudioSource���擾
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

        // ���y�̃��[�v�ݒ�
        if (audioSource != null)
        {
            audioSource.loop = true; // ���y�����[�v�Đ�
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ghostAI != null && audioSource != null)
        {
            if (ghostAI.sensor) // sensor��true�Ȃ特�y���Đ�
            {
                if (!isPlaying)
                {
                    audioSource.Play();
                    isPlaying = true;
                }
            }
            else // sensor��false�Ȃ特�y���~
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
