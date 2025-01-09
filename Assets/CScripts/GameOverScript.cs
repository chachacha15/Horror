using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public GameObject gameOverUI; // GameOver UI��R�Â�

    private void Start()
    {
        // ���̃I�u�W�F�N�g�̍ŏ��̎q�I�u�W�F�N�g���擾���ACanvas���ǂ������m�F���Đݒ�
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Canvas>() != null) // �q�I�u�W�F�N�g��Canvas�������Ă��邩�m�F
            {
                gameOverUI = child.gameObject; // �Y���I�u�W�F�N�g��gameOverUI�ɐݒ�
                break;
            }
        }

        // �O�̂��߁AgameOverUI���ݒ肳��Ă��Ȃ��ꍇ�ɃG���[���O���o��
        if (gameOverUI == null)
        {
            Debug.LogError("GameOver UI (Canvas) ��������܂���ł����B");
        }
    }

    private void Update()
    {
        // �X�y�[�X�L�[�������ꂽ��RestartGame���Ăяo��
        if (gameOverUI != null && gameOverUI.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    public void TriggerGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // GameOver UI��\��
            //Time.timeScale = 0f; // �Q�[�����ꎞ��~
        }
        else
        {
            Debug.LogError("GameOver UI ���ݒ肳��Ă��܂���B");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // ���Ԃ��ĊJ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���݂̃V�[�����ēǂݍ���
    }
}
