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
    public FootIK footIK;
    public Rigidbody rb;
    public MeshTrail meshTrail;
    public Animator animator;
    public Camera mainCam;

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

    [Header("Debug")]
    public bool showDebug;
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
        footIK = GetComponent<FootIK>();
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

    #region Animation Events
    public void IsPerformingAction()
    {
        isPerformingAction = true;
    }

    public void NotPerformingAction()
    {
        isPerformingAction = false;
    }

    public void ActivateGravity()
    {
        useGravity = true;
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }


    
}
