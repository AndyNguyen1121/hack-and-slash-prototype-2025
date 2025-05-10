using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DummyManager : MonoBehaviour, IDamageable
{
    [field: SerializeField]
    public float Health { get; set; }

    [field: SerializeField]
    public float MaxHealth { get; set; }
    public UnityEvent OnHealthChanged { get; set; }


    public Animator animator;

    private void Awake()
    {
        Health = MaxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float value)
    {
        animator.Play("Hit", 1, 0f);
    }

    public void IncreaseHealth(float value)
    {

    }

}
