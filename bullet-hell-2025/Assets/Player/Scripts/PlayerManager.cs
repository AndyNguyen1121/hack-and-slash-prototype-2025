using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

//[ExecuteAlways]
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("Auto-Initizalized References")]
    public PlayerMovementManager playerMovementManager;
    public PlayerAnimationManager playerAnimationManager;
    public PlayerCombatStateMachine playerCombatStateMachine;
    public PlayerCombatManager playerCombatManager;
    public PlayerCameraManager playerCameraManager;
    public CharacterController characterController;
    public PlayerUIManager playerUIManager;
    public SwordSlashManager swordSlashManager; 
    public FootIK footIK;
    public Rigidbody rb;
    public MeshTrail meshTrail;
    public Animator animator;
    public Camera mainCam;

    [Header("Assign in Inspector")]
    public GameObject playerCenter;

    [Header("Action Flags")]
    public bool isPerformingAction = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool canAttack = true;
    public bool useGravity = true;
    public bool inFinisher = false;

    [Header("Ground Check")]
    public bool isGrounded = true;
    public float groundCheckRadius;
    public Vector3 groundCheckOffset;
    public LayerMask whatIsGround;



    public List<Collider> colliders = new();
    public Collider[] enemyCollided;
    public Coroutine enemyCollisionCheck;
    public bool collisionCheckCoroutineActive = false;
    [Header("Debug")]
    public bool showDebug;
    private float timeControllerIsDisabled;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainCam = Camera.main;
        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimationManager = GetComponent<PlayerAnimationManager>();
        playerCombatStateMachine = GetComponent<PlayerCombatStateMachine>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerCameraManager = GetComponent<PlayerCameraManager>();
        characterController = GetComponent<CharacterController>();
        swordSlashManager = GetComponent<SwordSlashManager>();
        footIK = GetComponent<FootIK>();
        playerUIManager = GetComponent<PlayerUIManager>();
        rb = GetComponent<Rigidbody>();
        meshTrail = GetComponent<MeshTrail>();  
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        IgnoreMyOwnColliders();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGroundedState();
        DetectEnemyCollisions();

        // safeguard
        if (!characterController.enabled)
        {
            timeControllerIsDisabled += Time.deltaTime;
        }
        else
        {
            timeControllerIsDisabled = 0f;
        }

        if (timeControllerIsDisabled > 1f)
        {
            characterController.enabled = true;
        }


    }

    private void CheckGroundedState()
    {
        isGrounded = Physics.CheckSphere(transform.position + groundCheckOffset, groundCheckRadius, whatIsGround);
        animator.SetBool("isGrounded", isGrounded);
    }


    public void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] currentColliders = GetComponentsInChildren<Collider>();

        colliders.AddRange(currentColliders);
        colliders.Add(characterControllerCollider);

        foreach (var collider in colliders)
        {
            foreach (var otherCollider in colliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }

        if (colliders.Contains(playerCombatManager.weaponCollider))
        {
            colliders.Remove(playerCombatManager.weaponCollider);
        }
    }

    public void IgnorePlayerCollider(Collider collider)
    {
        foreach (var col in colliders)
        {
            Physics.IgnoreCollision(collider, col, true);
        }
        Collider characterControllerCollider = GetComponent<Collider>();
        Physics.IgnoreCollision(collider, characterControllerCollider, true);
    }

    public void EnableCollisionWithPlayerColliders(Collider collider)
    {
        foreach (var col in colliders)
        {
            Physics.IgnoreCollision(collider, col, false);
        }
        Collider characterControllerCollider = GetComponent<Collider>();
        Physics.IgnoreCollision(collider, characterControllerCollider, false);
    }
    public void IgnoreEnemyLayerCollision()
    {
        characterController.excludeLayers = playerCombatManager.whatIsDamageable;
    }

    public void EnableEnemyLayerCollision()
    {
        characterController.excludeLayers = 0;
    }

    public void DetectEnemyCollisions()
    {
        CapsuleCollider capsule = characterController.GetComponent<CapsuleCollider>();

        // Get capsule start and end points in world space
        Vector3 point1 = characterController.transform.position + capsule.center + Vector3.up * (capsule.height / 2 - capsule.radius);
        Vector3 point2 = characterController.transform.position + capsule.center - Vector3.up * (capsule.height / 2 - capsule.radius);

        float radius = capsule.radius;

        enemyCollided = Physics.OverlapCapsule(point1, point2, radius, playerCombatManager.whatIsDamageable);
    }
    public void AttemptToEnableEnemyCollision()
    {
        if (collisionCheckCoroutineActive)
            StopCoroutine(enemyCollisionCheck);
        enemyCollisionCheck = StartCoroutine(CheckForEnemyCollision());
    }

    public void StopEnemyCollisionCoroutine()
    {
        StopCoroutine(enemyCollisionCheck);
        collisionCheckCoroutineActive = false;
    }

    private IEnumerator CheckForEnemyCollision()
    {
        collisionCheckCoroutineActive = true;
        while (enemyCollided.Length > 0)
        {   
            yield return null;
        }

        EnableEnemyLayerCollision();
        collisionCheckCoroutineActive = false;
    }

    #region Animation Events
    public void IsPerformingAction()
    {
        isPerformingAction = true;
    }

    public void NotPerformingAction()
    {
        isPerformingAction = false;
    }


    public void PlayerFootstepSFX(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight < 0.57f)
            return;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.footSteps, transform.position);
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }


    
}
