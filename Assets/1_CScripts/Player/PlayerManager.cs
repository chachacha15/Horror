using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables

    public bool isChased = false;

    // ‘¼ƒNƒ‰ƒX
    private Inventory inventory;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isChased == true)
        {
            inventory.shakeSpeed = 15f;
        }
        else
        {
            inventory.shakeSpeed = 1f;
        }
    }
}
