using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DampenCamTarget : MonoBehaviour
{
    public float yOffset;
    public Transform target;
    public float defaultSmoothTime;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float smoothTime = defaultSmoothTime;

        if (PlayerManager.instance.playerCameraManager.isLockedOn)
        {
            smoothTime = 0;
        }

        Vector3 updatedPosition = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);

        Vector3 vel = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, updatedPosition, ref vel, smoothTime * Time.deltaTime);
    }
}
