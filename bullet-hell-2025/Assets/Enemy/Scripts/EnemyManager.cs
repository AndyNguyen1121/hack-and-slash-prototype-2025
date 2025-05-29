using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour, IDamageable
{
    [field: SerializeField]
    public float Health { get; set; }

    [field: SerializeField]
    public float MaxHealth { get; set; }
    public UnityEvent OnHealthChanged { get; set; }

    public EnemyAnimationManager enemyAnimationManager;
    public EnemyBehavior enemyBehavior;
    public Animator animator;
    public NavMeshAgent agent;

    public float moveSpeed = 1f;


    public void Awake()
    {
        enemyAnimationManager = GetComponent<EnemyAnimationManager>();
        enemyBehavior = GetComponent<EnemyBehavior>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        Health = MaxHealth;

        ActivateRootMotion();
    }

    public void ActivateRootMotion()
    {
        animator.applyRootMotion = true;
        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    public void DeactivateRootMotion()
    {
        animator.applyRootMotion = false;
        agent.updatePosition = false;
        agent.updateRotation = false;
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
