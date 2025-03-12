using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    #region

    // シーン上にある開閉可能な障害物を取得する
    public GameObject[] doors;
    public GameObject[] windows;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        doors = GameObject.FindGameObjectsWithTag("Door");
        windows = GameObject.FindGameObjectsWithTag("Window");

        foreach (GameObject door in doors)
        {
            door.GetComponent<DoorController>().isLockedDoor = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
