using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    public float Health { get; }
    public float MaxHealth { get; }
    public UnityEvent OnHealthChanged { get; set; }

    public void TakeDamage(float value);
    public void IncreaseHealth(float value);
    public void SetHealthValue(float value);
}
