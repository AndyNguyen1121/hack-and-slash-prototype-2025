using System;
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
    public EnemyInteractionManager enemyInteractionManager;
    public EnemyBehavior enemyBehavior;
    public Animator animator;
    public NavMeshAgent agent;

    public float moveSpeed = 1f;

    [Header("Attack Settings")]
    private Coroutine cooldownCoroutine;
    public float minCooldown = 0.3f;
    public float maxCooldown = 1f;

    [Header("Flags")]
    public bool isPerformingAction;
    public bool canAttack = true;

    public event Action<EnemyManager> SendAttackSignal;

    public void Awake()
    {
        enemyAnimationManager = GetComponent<EnemyAnimationManager>();
        enemyBehavior = GetComponent<EnemyBehavior>();
        enemyInteractionManager = GetComponent<EnemyInteractionManager>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        Health = MaxHealth;

        
        ActivateRootMotion();
    }

    private void Start()
    {
        WorldEnemySpawnerManager.Instance.RegisterEnemy(this);
        ActivateCooldown();
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
        agent.updatePosition = true;
        agent.updateRotation = false;
        agent.speed = 0f;
    }

    #region Health
    public void TakeDamage(float value, Vector3 attackDir, GameObject attackSource)
    {
        Health -= value;
    }

    public void IncreaseHealth(float value)
    {
        Health += value;
    }

    public void SetHealthValue(float value)
    {
        Health = value;
    }

    #endregion
    public void ActivateCooldown(float time = -1)
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        float cooldown = time;

        if (time < 0)
        {
            cooldown = UnityEngine.Random.Range(minCooldown, maxCooldown);
        }

        cooldownCoroutine = StartCoroutine(ResetAttack(cooldown));
    }
    public void EndAttack()
    {
        canAttack = false;
        WorldEnemySpawnerManager.Instance.RemoveFromAttackingPool(this);
    }

    public IEnumerator ResetAttack(float time)
    {
        yield return new WaitForSeconds(time);
        SendAttackSignal?.Invoke(this);
    }

    private void OnDrawGizmos()
    {
        if (canAttack)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
