using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthManager : MonoBehaviour, IDamageable
{
    [field: SerializeField]
    public float Health { get; private set; }

    [field: SerializeField]
    public float MaxHealth { get; private set; }
    public UnityEvent OnHealthChanged { get; set; }

    private void Awake()
    {
        Health = MaxHealth;
    }

    public void TakeDamage(float value)
    {
        value = -value;
        Debug.Log($"Bro really got hit for {value} damage");
    }

    public void IncreaseHealth(float value)
    {

    }

    public void SetHealthValue(float value)
    {
        Health = value;
    }

}
