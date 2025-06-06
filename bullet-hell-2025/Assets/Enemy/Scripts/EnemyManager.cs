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

    [Header("Attack Settings")]
    private Coroutine cooldownCoroutine;
    public float minCooldown = 0.3f;
    public float maxCooldown = 1f;

    [Header("Flags")]
    public bool isPerformingAction;
    public bool canAttack = true;
    public bool isSendingAttackSignal = false;

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
        agent.updatePosition = true;
        agent.updateRotation = false;
        agent.speed = 0f;
    }

    #region Health
    public void TakeDamage(float value)
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
            cooldown = Random.Range(minCooldown, maxCooldown);
        }

        cooldownCoroutine = StartCoroutine(ResetAttack(cooldown));
    }
    public void EndAttack()
    {
        canAttack = false;
        isSendingAttackSignal = false;
        WorldEnemySpawnerManager.Instance.RemoveFromAttackingPool(this);
    }

    public IEnumerator ResetAttack(float time)
    {
        yield return new WaitForSeconds(time);
        canAttack = true;
    }
}
