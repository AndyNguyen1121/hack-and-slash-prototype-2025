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

    public Collider[] colliders;

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
    }

    public void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
       colliders = GetComponentsInChildren<Collider>();

        List<Collider> ignoreColliders = new List<Collider>();

        foreach (var collider in colliders)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(characterControllerCollider);

        foreach (var collider in ignoreColliders)
        {
            foreach (var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
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

    #region Animation Events
    public void IsPerformingAction()
    {
        isPerformingAction = true;
    }

    public void NotPerformingAction()
    {
        isPerformingAction = false;
    }

    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }
}
