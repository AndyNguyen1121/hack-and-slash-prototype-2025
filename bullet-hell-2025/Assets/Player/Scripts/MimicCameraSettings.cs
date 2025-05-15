using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicCameraSettings : MonoBehaviour
{
    public Camera UICam;
    public Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        UICam = GetComponent<Camera>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UICam.fieldOfView = mainCam.fieldOfView;
    }
}
