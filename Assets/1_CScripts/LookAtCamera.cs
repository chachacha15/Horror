using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (mainCam != null)
        {
            transform.forward = mainCam.transform.forward;
        }
    }
}

