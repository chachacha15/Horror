using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionProbeUpdater : MonoBehaviour
{
    ReflectionProbe reflectionProbe;
    void Start()
    {
        reflectionProbe = GetComponent<ReflectionProbe>();
    }

    // Update is called once per frame
    void Update()
    {
        reflectionProbe.transform.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y * -1f,
            Camera.main.transform.position.z
        );

        reflectionProbe.RenderProbe();
    }
}
