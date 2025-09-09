using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class DummyManager : MonoBehaviour, IDamageable
{
    public EnemyClassification enemyClassification;
    [field: SerializeField]
    public float Health { get; set; }

    [field: SerializeField]
    public float MaxHealth { get; set; }
    public UnityEvent OnHealthChanged { get; set; }

    public EnemyAnimationManager enemyAnimationManager;
    public EnemyInteractionManager enemyInteractionManager;
    public EnemyCombatManager enemyCombatManager;
    public EnemyBehavior enemyBehavior;
    public Animator animator;
    public NavMeshAgent agent;
    public Collider enemyCollider;

    public float moveSpeed = 1f;
    public GameObject bloodParticle;

    [Header("Attack Settings")]
    private Coroutine cooldownCoroutine;
    public float minCooldown = 0.3f;
    public float maxCooldown = 1f;

    [Header("Flags")]
    public bool isPerformingAction;
    public bool canAttack = true;
    public bool isAlive = true;
    public bool canGrapple = true;
    public bool canMove = true;

    public event Action<EnemyManager> SendAttackSignal;
    public event Action OnDeath;

    public virtual void Awake()
    {
        enemyAnimationManager = GetComponent<EnemyAnimationManager>();
        enemyBehavior = GetComponent<EnemyBehavior>();
        enemyInteractionManager = GetComponent<EnemyInteractionManager>();
        enemyCombatManager = GetComponent<EnemyCombatManager>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponent<Collider>();
        Health = MaxHealth;

        agent.enabled = false;
        agent.enabled = true;
        ActivateRootMotion();
    }

    public virtual void Start()
    {
        LockOnTarget lockOnScript = GetComponent<LockOnTarget>();

        if (lockOnScript != null)
        {
            OnDeath += () => {
                lockOnScript.enabled = false;

                if (PlayerManager.instance.playerCameraManager.currentLockOnTarget == this.transform)
                {
                    PlayerManager.instance.playerCameraManager.DisableCurrentTarget();
                }

                enemyCombatManager.CloseWeaponCollider();

            };
        }
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
    public virtual bool TakeDamage(float value, Vector3 attackLocation, GameObject attackSource)
    {
        if (!isAlive)
            return false;

        Health = Mathf.Max(Health - value, 0);

        Vector3 hitDirection = attackLocation - transform.position;
        hitDirection.Normalize();
        hitDirection.y = 0;
        Instantiate(bloodParticle, attackLocation + (0.3f * -hitDirection), Quaternion.identity);

        if (Health == 0)
        {
            isAlive = false;
            OnDeath?.Invoke();
            return false;
        }

        float horizontalHitDir = Vector3.Dot(hitDirection, transform.right);
        float verticalHitDir = Vector3.Dot(transform.forward, hitDirection);


        if (Mathf.Abs(horizontalHitDir) > Mathf.Abs(verticalHitDir))
        {
            if (horizontalHitDir < 0)
            {
                animator.Play("EnemyReactLeft", 0, 0f);
            }
            else
            {
                animator.Play("EnemyReactRight", 0, 0f);
            }
        }
        else
        {
            if (verticalHitDir < 0)
            {
                animator.Play("EnemyReactBack", 0, 0f);
            }
            else
            {
                animator.Play("EnemyReactFront", 0, 0f);
            }
        }

        return true;

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
  

  
    public void DestroySelf()
    {
        Destroy(gameObject.transform.parent.gameObject);
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
