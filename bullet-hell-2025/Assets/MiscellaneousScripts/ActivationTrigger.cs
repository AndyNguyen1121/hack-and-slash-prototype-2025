using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ActivationTrigger : MonoBehaviour
{
    public string validTag = "Player";
    public UnityEvent onTriggerEnter;

    private bool activated = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(validTag) && !activated)
        {
            activated = true;
            onTriggerEnter.Invoke();
        }
    }
}
