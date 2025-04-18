using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ValveManager : MonoBehaviour
{
    #region Variables

    public int valveTurnedCount = 0; // バルブが回された回数
    private int valveRequiredCount = 4; // ギミックをクリアするのに必要な、バルブを回す回数

    private AudioSource valveAS;
    private CameraSwitcher cameraSwitcher;

    #endregion



    #region Methods


    void Start()
    {
        valveTurnedCount = 0;

        valveAS = GameObject.Find("ValveSoundAS").GetComponent<AudioSource>();
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();

    }


    void Update()
    {

    }

    public void ValveCountIncrement()
    {
        valveTurnedCount++;
        valveAS.Play();
        ValveCountCheck();
    }

    public void ValveCountCheck()
    {
        if (valveTurnedCount >= valveRequiredCount)
        {
            Debug.Log("!");
        }

    }

    #endregion 
}
