using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponColliderBridge : MonoBehaviour, IColliderBridge
{
    public PlayerCombatManager playerCombatManager;

    void Start()
    {
        playerCombatManager = GetComponentInParent<PlayerCombatManager>();
    }
    public void OnCollisionEnter(Collision collision)
    {

    }
    public void OnCollisionStay(Collision collision)
    {

    }
    public void OnCollisionExit(Collision collision)
    {

    }
    public void OnTriggerEnter(Collider other)
    {
        playerCombatManager.OnWeaponTriggerEnter(other);
    }
    public void OnTriggerStay(Collider other)
    {

    }
    public void OnTriggerExit(Collider other)
    {

    }
}
