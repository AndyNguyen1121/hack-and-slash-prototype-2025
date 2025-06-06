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
    public void OnWeaponTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")
            && !colliderIds.Contains(other.gameObject.GetInstanceID()))
        {
            IDamageable playerHealthManager;
            if (other.gameObject.TryGetComponent<IDamageable>(out playerHealthManager))
            {
                playerHealthManager.TakeDamage(damage);
                Debug.Log(other.name);
            }

            colliderIds.Add(other.gameObject.GetInstanceID());
        }

    }

    public void OpenWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    public void CloseWeaponCollider()
    {
        colliderIds.Clear();
        weaponCollider.enabled = false;
    }
}
