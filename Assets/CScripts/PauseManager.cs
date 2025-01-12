using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false; // ˆê’â~’†‚©‚Ç‚¤‚©‚ğ’ÇÕ

    void Update()
    {
        // PƒL[‚ğ‰Ÿ‚µ‚½‚Æ‚«‚Éˆ—‚ğØ‚è‘Ö‚¦‚é
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame(); // ƒQ[ƒ€‚ğÄŠJ
            }
            else
            {
                PauseGame(); // ƒQ[ƒ€‚ğˆê’â~
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // ŠÔ‚ğ’â~
        isPaused = true;
        Debug.Log("ƒQ[ƒ€‚ªˆê’â~‚µ‚Ü‚µ‚½");
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // ŠÔ‚ğÄŠJ
        isPaused = false;
        Debug.Log("ƒQ[ƒ€‚ªÄŠJ‚µ‚Ü‚µ‚½");
    }
}
