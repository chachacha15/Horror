using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Variables

    public GameObject[] enemies;


    // 他クラス
    private PlayerManager playerManager;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
       playerManager = FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindChasingEnemy()
    {
        for (int i = 0; i < enemies.Length; ++i)
        {
            GhostAI ghostAI = enemies[i].GetComponent<GhostAI>();

            if (ghostAI != null && ghostAI.currentState == State.Chase)
            {
                playerManager.isChased = true;
                return;
            }
        }

        // 追跡状態の敵がいない場合はフラグをオフにする
        playerManager.isChased = false;
    }
}
