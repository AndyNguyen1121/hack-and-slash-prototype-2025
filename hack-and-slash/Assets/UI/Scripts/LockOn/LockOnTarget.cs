using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    public Transform cursorPosition;
    void Start()
    {
        if (cursorPosition == null)
        {
            cursorPosition = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
