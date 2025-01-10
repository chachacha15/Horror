using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Text�p
using TMPro; //TextMeshPro�p


public class GameOverScript : MonoBehaviour
{
    public GameObject gameOverUI; // GameOver UI��R�Â�
    public TextMeshProUGUI gameOverText; // GameOver Text�I�u�W�F�N�g
    private Color textColor;

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

        // GameOverText��T��
        if (gameOverUI != null)
        {
            gameOverText = gameOverUI.GetComponentInChildren<TextMeshProUGUI>();
            textColor = gameOverText.color;

        }

        // �G���[���O�̊m�F
        if (gameOverUI == null)
        {
            Debug.LogError("GameOver UI (Canvas) ��������܂���ł����B");
        }
        if (gameOverText == null)
        {
            Debug.LogError("GameOver Text ��������܂���ł����B");
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
            StartCoroutine(FadeTextAlpha()); // �K���}�l�i�A���t�@�l�j���t�F�[�h
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

    private IEnumerator FadeTextAlpha()
    {
        if (gameOverText == null)
            yield break;

        float alpha = 0; // �����A���t�@�l
        bool increasing = true; // �A���t�@�l�𑝉������邩���������邩

        while (true)
        {
            // �A���t�@�l�̑���
            alpha += (increasing ? 1f : -1f) * Time.deltaTime * 2f; // �����\�ȑ��x

            // �A���t�@�l�͈̔͂𐧌�
            if (alpha > 1f)
            {
                alpha = 1f;
                increasing = false; // �����ɐ؂�ւ�
            }
            else if (alpha < 0f)
            {
                alpha = 0f;
                increasing = true; // �����ɐ؂�ւ�
            }

            // �e�L�X�g�̃J���[���X�V
            textColor.a = alpha; // �A���t�@�l���X�V
            gameOverText.color = textColor;

            yield return null; // ���̃t���[���܂őҋ@
        }
    }
}
