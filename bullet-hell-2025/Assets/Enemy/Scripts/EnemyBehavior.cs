using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{

    public enum MovementState
    {
        Retreat,
        Chase,
        Idle,
    }

    private BehaviorTree enemyBehaviorTree;
    protected EnemyManager enemyManager;
    public Transform player;
    public MovementState movementState;

    [Header("Settings")]
    public float attackDistance = 2f;
    public Vector2 minimumDistanceRange;
    public float maxDistFromPlayer;
    public float rotationSpeed;


    [Header("Information")]
    public bool targetInRange = false;
    public float distanceFromPlayer;
    public float defaultDeadzoneBuffer = 0.5f;
    [SerializeField]
    private float deadzoneBuffer;
    

    [Header("Flags")]
    public bool isStunned = false;
    public bool canMove = true;
    public bool isStopped = false;

    // Start is called before the first frame update
    public virtual void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
    }
    public virtual void Start()
    {
        minDistance = minimumDistanceRange.x;
        CreateEnemyBehaviorTree();
        movementState = MovementState.Retreat;
        player = PlayerManager.instance.transform;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!enemyManager.isAlive && deathSequenceStarted)
            return;

        UpdateRangeToTarget();
        HandleStunnedState();
        enemyBehaviorTree.Process();

        if (Input.GetKey(KeyCode.T))
        {
            isStunned = true;
        }
    }

    void UpdateRangeToTarget()
    {
        Vector3 adjustedTargetPos = new Vector3(player.position.x, 0f, player.position.z);
        Vector3 adjustedSelfPos = new Vector3(transform.position.x, 0f, transform.position.z);

        distanceFromPlayer = Vector3.Distance(adjustedTargetPos, adjustedSelfPos);
        targetInRange =  distanceFromPlayer <= attackDistance;

    }
    public void CreateEnemyBehaviorTree()
    {
        enemyBehaviorTree = new BehaviorTree("EnemyBehaviorTree");

        Selector activeStatus = new Selector("IsEnemyAlive");
        enemyBehaviorTree.AddChild(activeStatus);

        // Alive
        Sequence isAliveSequence = new Sequence("AliveSequence");
        activeStatus.AddChild(isAliveSequence);
        isAliveSequence.AddChild(new Leaf("CheckIsAlive", new Condition(() => enemyManager.isAlive)));

        // Handles Attack and Movement
        isAliveSequence.AddChild(ActivityTree());


        // Dead
        Sequence isDeadSequence = new Sequence("IsDead");
        isDeadSequence.AddChild(new Leaf("CheckIfDead", new Condition(() => !enemyManager.isAlive)));
        activeStatus.AddChild(isDeadSequence);
        isDeadSequence.AddChild(new Leaf("PrintIsDead", new ActionStrategy(StartDeathSequence)));
    }

    private bool deathSequenceStarted;
    private bool deathAnimationPlayed;
    public virtual void StartDeathSequence()
    {
        if (deathSequenceStarted)
            return;
        deathSequenceStarted = true;

        
        StopMovement();
        WorldEnemySpawnerManager.Instance.UnregisterEnemy(enemyManager);
        enemyManager.enemyCombatManager.UnparentWeapon();

        if (!enemyManager.enemyInteractionManager.isGrounded)
        {
            DissolveObject dissolveScript = GetComponent<DissolveObject>();

            if (dissolveScript != null)
            {
                dissolveScript.StartDissolve();
                DOVirtual.DelayedCall(dissolveScript.fadeSpeed, () => enemyManager.DestroySelf());
              
            }
        }
        else
        {
            enemyManager.enemyAnimationManager.PlayActionAnimation("EnemyDeath"); 
        }
    }

    private Node ActivityTree()
    {
        Sequence attackAndMovementSeq = new Sequence("AttackAndMovement");
        attackAndMovementSeq.AddChild(new Leaf("IsStunned", new Condition(() => !isStunned)));

        Selector attackOrMove = new Selector("AttackOrMove");

        // Attack Sequence
        Sequence attackSeq = new Sequence("AttackSequence");
        attackSeq.AddChild(new Leaf("CanAttack", new Condition(() => enemyManager.canAttack)));
        attackSeq.AddChild(new Leaf("IsInRange", new Condition(() => targetInRange)));
        attackSeq.AddChild(new Leaf("HandleAttacks", new ActionStrategy(HandleAttacks)));
        attackOrMove.AddChild(attackSeq);

        // Movement Sequence
        attackOrMove.AddChild(new Leaf("HandleMovement", new ActionStrategy(HandleMovements)));
        attackAndMovementSeq.AddChild(attackOrMove);

        return attackAndMovementSeq;
    }

    protected virtual void HandleAttacks()
    {
        if (!enemyManager.isPerformingAction && !enemyManager.enemyInteractionManager.inKnockUpAnimation)
        {
            enemyManager.canAttack = false;
            enemyManager.enemyAnimationManager.PlayActionAnimation("EnemyAttack1");
        }
    }

    protected virtual void HandleMovements()
    {
        if (!enemyManager.agent.enabled)
        {
            return;
        }
        
        
        switch (movementState)
        {
            case MovementState.Chase:
                HandleChaseMovement();
                break;
            
            case MovementState.Retreat:
                HandleRetreatMovement();
                break;
        };

        


    }

    private bool choseRetreatDirection;
    private bool minDistanceRangeChosen;
    public float minDistance;
    private int retreatAxisDirection;
    private void HandleChaseMovement()
    {
        choseRetreatDirection = false;
        minDistanceRangeChosen = false;

        if (!enemyManager.canAttack && distanceFromPlayer < minDistance)
        {
            StopMovement();
            movementState = MovementState.Retreat;
            return;
        }

        enemyManager.agent.updateRotation = false;
        enemyManager.agent.updatePosition = true;

        LookTowardsPath();
        enemyManager.agent.SetDestination(player.position);
        Vector3 velocity = transform.InverseTransformDirection(enemyManager.agent.velocity).normalized;
        enemyManager.enemyAnimationManager.SetMovementParameters(velocity.x, velocity.z);
    }

    private void HandleRetreatMovement()
    {
        LookTowardsPlayer();
        enemyManager.agent.updateRotation = false;
        enemyManager.agent.updatePosition = true;

        if (!minDistanceRangeChosen)
        {
            minDistance = Random.Range(minimumDistanceRange.x, minimumDistanceRange.y);
            minDistanceRangeChosen = true;

        }

        if (distanceFromPlayer > maxDistFromPlayer || enemyManager.canAttack)
        {
            movementState = MovementState.Chase;
            ResumeMovement();
            return;
        }

        if (distanceFromPlayer > minDistance - deadzoneBuffer && distanceFromPlayer < maxDistFromPlayer)
        {
            // Walk Sideways
            if (!choseRetreatDirection)
            {
                retreatAxisDirection = Random.Range(0, 2) * 2 - 1;
                choseRetreatDirection = true;
                //StopMovement();
            }
            
            Vector3 movementDir = Vector3.zero;

            if (retreatAxisDirection == 1)
            {
                movementDir = transform.right;
            }
            else
            {
                movementDir = -transform.right;
            }

            enemyManager.agent.Move(movementDir * 0.01f * Time.deltaTime);
            Vector3 vel = transform.InverseTransformDirection(movementDir);
            enemyManager.enemyAnimationManager.SetMovementParameters(retreatAxisDirection, 0);
            return;
        }

        ResumeMovement();
        enemyManager.agent.Move(-transform.forward * 0.01f * Time.deltaTime);
        Vector3 localVelocity = transform.InverseTransformDirection(-transform.forward);
        enemyManager.enemyAnimationManager.SetMovementParameters(localVelocity.x, localVelocity.z);
    }

    private void HandleStunnedState()
    {
        if (isStunned || enemyManager.enemyInteractionManager.inKnockUpAnimation || !canMove)
        {
            enemyManager.agent.enabled = false;
            enemyManager.animator.SetBool("isMoving", false);
        }
        else if (canMove) 
        {
            enemyManager.agent.enabled = true;
            enemyManager.animator.SetBool("isMoving", true);
        }
    }

    private void ChangeDeadzoneBuffer()
    {
        deadzoneBuffer = defaultDeadzoneBuffer;
    }
    public void StopMovement()
    {
        /*deadzoneBuffer = defaultDeadzoneBuffer;
        isStopped = true;
        enemyManager.agent.updatePosition = false;
        canMove = false;*/
    }

    private void ResumeMovement()
    {
        deadzoneBuffer = 0;
        isStopped = false;
    }

    private void LookTowardsPlayer()
    {
        Vector3 dirTowardsPlayer = player.position - transform.position;
        dirTowardsPlayer.y = 0;
        Quaternion lookDir = Quaternion.LookRotation(dirTowardsPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, rotationSpeed * Time.deltaTime);
    }

    private void LookTowardsPath()
    {
        Vector3 dirTowardsPath = enemyManager.agent.desiredVelocity.normalized;

        if (dirTowardsPath.sqrMagnitude > 0.001f)
        {
            Quaternion lookDir = Quaternion.LookRotation(dirTowardsPath.normalized);
            lookDir = Quaternion.Euler(0f, lookDir.eulerAngles.y, 0f);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, rotationSpeed * Time.deltaTime);
        }

    }

}