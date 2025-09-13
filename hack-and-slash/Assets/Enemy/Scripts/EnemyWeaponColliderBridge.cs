using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponColliderBridge : MonoBehaviour, IColliderBridge
{
    public EnemyCombatManager enemyCombatManager;

    void Start()
    {
        enemyCombatManager = GetComponentInParent<EnemyCombatManager>();
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
        if (!this.enabled)
            return;

        if (enemyCombatManager != null)
            enemyCombatManager.OnWeaponTriggerEnter(other);
    }
    public void OnTriggerStay(Collider other)
    {

    }
    public void OnTriggerExit(Collider other)
    {

    }
}
