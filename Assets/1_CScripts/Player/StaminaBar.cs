using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StaminaBar : MonoBehaviour
{
    private Image staminaBarImage;
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 13f;
    public float staminaRecoveryRate = 5f;
    private bool isRunning;

    void Start()
    {
        // ���g�̃I�u�W�F�N�g�ɃA�^�b�`����Ă���Image�R���|�[�l���g���擾
        staminaBarImage = this.GetComponent<Image>();

        currentStamina = maxStamina;
        UpdateStaminaBar();
    }

    void Update()
    {
        if (isRunning)
        {
            DrainStamina();
        }
        else
        {
            RecoverStamina();
        }

        UpdateStaminaBar();
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
    }

    void DrainStamina()
    {
        if (currentStamina > 26)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina < 26)
            {
                currentStamina = 26;
            }
        }
    }

    void RecoverStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }
    }

    void UpdateStaminaBar()
    {
        float fillAmount = currentStamina / maxStamina;

        //�O�����^�̃X�v���C�g�g���Ă���Ƃ��̂�
        fillAmount = Mathf.Min(fillAmount, 0.99f); // �ő�l��99%�ɐ���
        fillAmount = Mathf.Max(fillAmount, 0.26f); // �ŏ��l��26%�ɐ���

        staminaBarImage.fillAmount = fillAmount;
    }
    public bool CanStartRunning()
    {
        return currentStamina >= 50; // �X�^�~�i��50�ȏ゠��ꍇ�Ƀ_�b�V���J�n�\
    }

    public bool CanContinueRunning()
    {
        return currentStamina > 26; // �X�^�~�i��0�ȏ゠��ꍇ�Ƀ_�b�V���p���\
    }
}
