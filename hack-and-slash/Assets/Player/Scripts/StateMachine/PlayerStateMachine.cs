using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    [Header("Auto-Initizalized References")]
    public PlayerManager playerManager;
    public PlayerInputManager playerInputManager;

    [Header("Movement Attributes")]
    public float speedAcceleration;
    public float currentSpeed;
    public float maxSprintSpeed;
    public float maxRunSpeed;
    public float maxWalkSpeed;
    public float rootMotionSpeedMultiplierXZ = 1;
    public float rootMotionSpeedMultiplierY = 1;
    public float airGravityScale = -5f;
    public float groundGravityScale = -9.81f;
    public Vector3 verticalVelocity;
    public Vector3 XZvelocity;

    [Header("Rotation Attributes")]
    public float rotationSlerpSpeed;
    public Vector3 playerDirection;

    [Header("Action Flags")]
    public bool useGravity = true;

    [Header("Jumping Settings")]
    public float jumpingHeight;
    public bool isJumping = false;
    private float timeAboveGround;
    private bool fallingWithoutJump = false;

    // state variables
    public PlayerBaseState currentState;
    public PlayerStateFactory stateFactory;

    private void Awake()
    {
        stateFactory = new PlayerStateFactory(this);
        currentState = stateFactory.Grounded();
        currentState.OnStateEnter();
    }
    private void Update()
    {
        currentState.UpdateStates();

        HandleGroundedMovements();
        HandleMovementRotations();
        HandleGravity();
        HandleJumping();
        CalculatePlayerInputDirection();
    }

    #region Movement

    private void CalculatePlayerInputDirection()
    {
        playerDirection = playerManager.mainCam.transform.right * playerInputManager.movementDirection.x;
        playerDirection += playerManager.mainCam.transform.forward * playerInputManager.movementDirection.y;

        playerDirection.y = 0;
        playerDirection.Normalize();
    }
    public void HandleGroundedMovements()
    {
        if (!playerManager.canMove || !playerManager.characterController.enabled)
            return;

        if (!playerManager.animator.applyRootMotion && playerInputManager.clampedDirection != Vector2.zero)
        {


            float speedCap = 0f;

            if (playerInputManager.moveAmount > 1f)
            {
                speedCap = maxSprintSpeed;
            }
            else if (playerInputManager.moveAmount > 0.5f)
            {
                speedCap = maxRunSpeed;
            }
            else
            {
                speedCap = maxWalkSpeed;
            }

            if (currentSpeed < speedCap)
            {
                currentSpeed += speedAcceleration * Time.deltaTime;
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, speedCap, speedAcceleration * Time.deltaTime);
            }

        }
        else if (playerManager.isGrounded)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, speedAcceleration * Time.deltaTime);
        }


        // HandleJumping(ref velocity);

        playerManager.characterController.Move(playerDirection * currentSpeed * Time.deltaTime);
    }


    public void HandleGravity()
    {
        if (!useGravity)
            return;


        if (verticalVelocity.y < 0 && !isJumping)
        {
            verticalVelocity.y = groundGravityScale;
        }
        else
        {

            verticalVelocity.y += airGravityScale * Time.deltaTime;
        }

        if (playerManager.characterController.enabled)
        {
            playerManager.characterController.Move(verticalVelocity * Time.deltaTime);
        }


        // Reset to origin state on landing
        if (playerManager.isGrounded && ((isJumping && verticalVelocity.y < 0) || fallingWithoutJump))
        {
            isJumping = false;
            fallingWithoutJump = false;
            //playerManager.animator.CrossFade("MovementBlend", 0.1f);
        }


        timeAboveGround = playerManager.isGrounded ? 0 : timeAboveGround + Time.deltaTime;



        // Start jump loop if falling too long without jumping
        if (!playerManager.isGrounded && !isJumping && timeAboveGround > 0.2f && !fallingWithoutJump)
        {
            //playerManager.playerAnimationManager.PlayActionAnimation("JumpCycle", true, false, false, true, true, true);
            fallingWithoutJump = true;
        }
    }

    public void HandleJumping()
    {
        if (!playerManager.isPerformingAction && playerManager.isGrounded && playerInputManager.jumpPressed)
        {
            playerManager.playerAnimationManager.PlayActionAnimation("JumpUp", true, false, false, true, true, true, 0.05f);
        }
    }

    public void ApplyJumpForce()
    {
        verticalVelocity.y = Mathf.Sqrt(-2 * airGravityScale * jumpingHeight);
        isJumping = true;
    }

    public void HandleMovementRotations()
    {
        if (!playerManager.canRotate)
            return;

        if (playerInputManager.clampedDirection != Vector2.zero)
        {

            if (playerDirection != Vector3.zero)
            {
                Quaternion desiredDirection = Quaternion.LookRotation(playerDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredDirection, rotationSlerpSpeed * Time.deltaTime);
            }

        }
        else
        {
            playerDirection = Vector3.zero;
        }
    }
    #endregion
}
