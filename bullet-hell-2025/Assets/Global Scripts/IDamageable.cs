using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    public float Health { get; }
    public float MaxHealth { get; }
    public UnityEvent OnHealthChanged { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="attackDir"></param>
    /// <param name="attackSource"></param>
    /// <returns>True if damage successfully registers. False if attack is mitigated, parried, etc.</returns>
    public bool TakeDamage(float value, Vector3 attackDir, GameObject attackSource);
    public void IncreaseHealth(float value);
    public void SetHealthValue(float value);
}
