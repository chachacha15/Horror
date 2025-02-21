using UnityEngine;
using UnityEngine.Audio;

public class OptionLaunch : MonoBehaviour
{
    [SerializeField]
    private GameObject soundOptionCanvas;

    [SerializeField]
    private AudioMixerSnapshot gameSoundShot;
    [SerializeField]
    private AudioMixerSnapshot optionSoundShot;

    [SerializeField]
    private AudioMixer audioMixer;

    void Update()
    {
        if (Input.GetKeyDown("0"))
        {
            bool isActive = soundOptionCanvas.activeSelf;
            soundOptionCanvas.SetActive(!isActive);

            if (isActive)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        optionSoundShot.TransitionTo(0.01f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        gameSoundShot.TransitionTo(0.01f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetMaster(float sliderValue)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Lerp(-80f, 0f, sliderValue));
    }

    public void SetBGM(float sliderValue)
    {
        audioMixer.SetFloat("BGMVol", Mathf.Lerp(-80f, 0f, sliderValue));
    }

    public void SetSE(float sliderValue)
    {
        audioMixer.SetFloat("SEVol", Mathf.Lerp(-80f, 0f, sliderValue));
    }
}
