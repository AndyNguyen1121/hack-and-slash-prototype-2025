using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatManager : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Collider weaponCollider;

    [Header("Damage Attributes")]
    public float damage = 5f;

    [SerializeField]
    private List<int> colliderIds = new();
    public virtual void OnWeaponTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")
            && !colliderIds.Contains(other.gameObject.GetInstanceID()))
        {
            IDamageable playerHealthManager;
            if (other.gameObject.TryGetComponent<IDamageable>(out playerHealthManager))
            {
                bool hitRegistered = playerHealthManager.TakeDamage(damage, other.ClosestPointOnBounds(weaponCollider.transform.position), gameObject);
                
                if (hitRegistered)
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyAxeHit, transform.position);
            }

            colliderIds.Add(other.gameObject.GetInstanceID());
        }

    }

    public virtual void Attack()
    {
        if (weaponCollider == null)
            return;

        weaponCollider.enabled = true;
    }

    public virtual void CloseWeaponCollider()
    {
        if (weaponCollider == null)
            return;

        colliderIds.Clear();
        weaponCollider.enabled = false;
    }

    public void UnparentWeapon()
    {
        if (weaponCollider == null)
            return;

        weaponCollider.transform.parent = null;

        Rigidbody weaponRb = weaponCollider.GetComponent<Rigidbody>();
        if (weaponRb != null)
        {
            weaponRb.isKinematic = false;
        }

        IColliderBridge weaponColliderBridge = weaponCollider.GetComponent<IColliderBridge>();
        if (weaponColliderBridge != null)
        {
            ((MonoBehaviour)weaponColliderBridge).enabled = false;
        }

        weaponCollider.enabled = true;
        weaponCollider.isTrigger = false;
    }
}
