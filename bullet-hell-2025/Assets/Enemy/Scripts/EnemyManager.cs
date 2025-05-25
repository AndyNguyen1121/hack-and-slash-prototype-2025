using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour, IDamageable
{
    [field: SerializeField]
    public float Health { get; set; }

    [field: SerializeField]
    public float MaxHealth { get; set; }
    public UnityEvent OnHealthChanged { get; set; }

    public Animator animator;

    public void Awake()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(float value)
    {
        Health -= value;
    }

    public void IncreaseHealth(float value)
    {
        Health += value;
    }
}
