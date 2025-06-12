using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    private EnemyManager enemyManager;
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
    public bool isAlive = true;
    public bool isStunned = false;
    public bool canMove = true;
    public bool isStopped = false;

    // Start is called before the first frame update
    private void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
    }
    void Start()
    {
        minDistance = minimumDistanceRange.x;
        CreateEnemyBehaviorTree();
        movementState = MovementState.Retreat;
        player = PlayerManager.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRangeToTarget();
        HandleStunnedState();
        enemyBehaviorTree.Process();
    }

    void UpdateRangeToTarget()
    {
        Vector3 adjustedTargetPos = new Vector3(player.position.x, 0f, player.position.z);
        Vector3 adjustedSelfPos = new Vector3(transform.position.x, 0f, transform.position.z);

        distanceFromPlayer = Vector3.Distance(adjustedTargetPos, adjustedSelfPos);
        targetInRange =  distanceFromPlayer <= attackDistance;

    }

    public void ToggleEnemy()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            isAlive = !isAlive;
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = isAlive;
        }

        if (!isAlive)
        {
            Debug.Log("Dead");
        }
    }

    public void CreateEnemyBehaviorTree()
    {
        enemyBehaviorTree = new BehaviorTree("EnemyBehaviorTree");

        Selector activeStatus = new Selector("IsEnemyAlive");
        enemyBehaviorTree.AddChild(activeStatus);

        // Alive
        Sequence isAliveSequence = new Sequence("AliveSequence");
        activeStatus.AddChild(isAliveSequence);
        isAliveSequence.AddChild(new Leaf("CheckIsAlive", new Condition(() => isAlive)));

        // Handles Attack and Movement
        isAliveSequence.AddChild(ActivityTree());


        // Dead
        Sequence isDeadSequence = new Sequence("IsDead");
        isDeadSequence.AddChild(new Leaf("CheckIfDead", new Condition(() => !isAlive)));
        activeStatus.AddChild(isDeadSequence);
        isDeadSequence.AddChild(new Leaf("PrintIsDead", new ActionStrategy(ToggleEnemy)));
    }

    public Node ActivityTree()
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

    public void HandleAttacks()
    {
        if (!enemyManager.isPerformingAction)
        {
            enemyManager.canAttack = false;
            enemyManager.enemyAnimationManager.PlayActionAnimation("EnemyAttack1");
        }
    }

    public void HandleMovements()
    {
        if (!enemyManager.agent.enabled)
            return;
        
        
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
    public void HandleChaseMovement()
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

    public void HandleRetreatMovement()
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

    public void HandleStunnedState()
    {
        if (isStunned || enemyManager.enemyInteractionManager.inKnockUpAnimation)
        {
            enemyManager.agent.enabled = false;
        }
        else if (canMove) 
        {
            enemyManager.agent.enabled = true;
        }
    }

    public void ChangeDeadzoneBuffer()
    {
        deadzoneBuffer = defaultDeadzoneBuffer;
    }
    public void StopMovement()
    {
        deadzoneBuffer = defaultDeadzoneBuffer;
        isStopped = true;
        enemyManager.agent.updatePosition = false;
    }

    public void ResumeMovement()
    {
        deadzoneBuffer = 0;
        isStopped = false;
    }

    public void LookTowardsPlayer()
    {
        Vector3 dirTowardsPlayer = player.position - transform.position;
        dirTowardsPlayer.y = 0;
        Quaternion lookDir = Quaternion.LookRotation(dirTowardsPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, rotationSpeed * Time.deltaTime);
    }

    public void LookTowardsPath()
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