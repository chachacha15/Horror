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
        // 自身のオブジェクトにアタッチされているImageコンポーネントを取得
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

        //三日月型のスプライト使っているときのみ
        fillAmount = Mathf.Min(fillAmount, 0.99f); // 最大値を99%に制限
        fillAmount = Mathf.Max(fillAmount, 0.26f); // 最小値を26%に制限

        staminaBarImage.fillAmount = fillAmount;
    }
    public bool CanStartRunning()
    {
        return currentStamina >= 50; // スタミナが50以上ある場合にダッシュ開始可能
    }

    public bool CanContinueRunning()
    {
        return currentStamina > 26; // スタミナが0以上ある場合にダッシュ継続可能
    }
}
