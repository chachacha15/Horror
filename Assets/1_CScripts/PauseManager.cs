using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false; // �ꎞ��~�����ǂ�����ǐ�

    void Update()
    {
        // P�L�[���������Ƃ��ɏ�����؂�ւ���
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame(); // �Q�[�����ĊJ
            }
            else
            {
                PauseGame(); // �Q�[�����ꎞ��~
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // ���Ԃ��~
        isPaused = true;
        Debug.Log("�Q�[�����ꎞ��~���܂���");
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // ���Ԃ��ĊJ
        isPaused = false;
        Debug.Log("�Q�[�����ĊJ���܂���");
    }
}
