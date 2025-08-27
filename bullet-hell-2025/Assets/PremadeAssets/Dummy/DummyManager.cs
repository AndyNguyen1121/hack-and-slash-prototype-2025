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
    public EnemyInteractionManager enemyInteractionManager;

    private void Awake()
    {
        Health = MaxHealth;
        animator = GetComponent<Animator>();
        enemyInteractionManager = GetComponent<EnemyInteractionManager>();

        Debug.Log(transform.parent.name);
    }

    public bool TakeDamage(float value, Vector3 attackDir, GameObject attackSource)
    {
        if (!enemyInteractionManager.inKnockUpAnimation)
            animator.Play("Hit", 0 , 0);

        return true;
        //enemyInteractionManager.JumpToHeightInTime(0.5f);
    }

    public void IncreaseHealth(float value)
    {

    }

    public void SetHealthValue(float value)
    {
        Health = value;
    }

}
