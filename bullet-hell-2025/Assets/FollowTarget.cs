using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 vel = Vector3.zero;
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref vel, Time.deltaTime, speed);
        }
    }
}
