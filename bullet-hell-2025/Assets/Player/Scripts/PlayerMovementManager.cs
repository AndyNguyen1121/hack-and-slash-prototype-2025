using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using DG.Tweening;

public class PlayerMovementManager : MonoBehaviour
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
    public float maxLockOnSpeed;
    public float airGravityScale = -5f;
    public float groundGravityScale = -9.81f;
    public Vector3 verticalVelocity;
    public Vector3 XZvelocity;

    [Header("Rotation Attributes")]
    public float rotationSlerpSpeed;
    public Vector3 playerDirection;
    public Vector3 localDirection;

    [Header("Jumping Settings")]
    public float firstJumpHeight;
    public float secondJumpHeight;
    public bool isJumping = false;
    public float timeAboveGround;
    public bool fallingWithoutJump = false;
    private bool canDoubleJump = true;

    [Header("Dashing")]
    public float dashSpeed;
    public AnimationCurve dashMovementCurve;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = PlayerManager.instance;
        playerInputManager = PlayerInputManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        HandleGroundedMovements();
        HandleMovementRotations();
        HandleDodgeMovements();
        HandleGravity();
        HandleJumping();
        CalculatePlayerInputDirection();
    }

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
            if (playerManager.playerCameraManager.isLockedOn)
            {
                speedCap = maxLockOnSpeed;
            }
            else if (playerInputManager.moveAmount > 1f)
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
        if (!playerManager.useGravity)
        {
            verticalVelocity = Vector3.zero;
            return;
        }


        if (verticalVelocity.y < 0 && !isJumping && playerManager.isGrounded)
        {
            verticalVelocity = Vector3.zero;
            //verticalVelocity.y += groundGravityScale * Time.deltaTime;
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
        if (playerManager.isGrounded 
            && ((isJumping && verticalVelocity.y < 0) || fallingWithoutJump) 
            && !playerManager.inFinisher 
            && !playerManager.playerAnimationManager.CheckIfAnimationIsPlaying("Uppercut"))
        {
            isJumping = false;
            fallingWithoutJump = false;
            canDoubleJump = true;
            //playerManager.animator.CrossFade("MovementBlend", 0.01f);
            playerManager.playerAnimationManager.PlayActionAnimation("MovementBlend", false, false, false, true, true, true);
        }


        timeAboveGround = playerManager.isGrounded ? 0 : timeAboveGround + Time.deltaTime;



        // Start jump loop if falling too long without jumping
        if (!playerManager.isGrounded && !isJumping && timeAboveGround > 0.2f && !fallingWithoutJump)
        { 
            playerManager.playerAnimationManager.PlayActionAnimation("JumpCycle", false, false, false, true, true, true);
            fallingWithoutJump = true;  
        }
    }

    public void HandleJumping()
    {
        if (!playerManager.isPerformingAction && playerManager.isGrounded && playerInputManager.jumpPressed)
        {
            playerManager.animator.SetBool("FootIK", false);
            playerManager.playerAnimationManager.PlayActionAnimation("JumpUp", false, false, false, true, true, true, 0.05f);
        }
        else if (isJumping && canDoubleJump && playerInputManager.jumpPressed)
        {
            playerManager.playerAnimationManager.PlayActionAnimation("DoubleJump", false, false, false, true, true, true, 0.2f);
            canDoubleJump = false;
        }
    }

    public void ApplyJumpForce(float height)
    {
        float g = Mathf.Abs(airGravityScale); // Ensure gravity is positive
        float t = 0.5f;

        // Calculate vertical velocity needed to reach height in time
        float velocity = (height + 0.5f * g * t * t) / t;

        verticalVelocity.y = velocity;
        isJumping = true;
    }

    public void ApplyFirstJumpForce()
    {
        ApplyJumpForce(firstJumpHeight);
    }

    public void ApplySecondJumpForce()
    {
        ApplyJumpForce(secondJumpHeight);
    }

    public void HandleMovementRotations()
    {
        if (!playerManager.canRotate)
            return;

        if (playerManager.playerCameraManager.isLockedOn)
        {
            Vector3 lookAtDir = playerManager.playerCameraManager.currentLockOnTarget.position - transform.position;
            lookAtDir.y = 0;
            lookAtDir.Normalize();

            Quaternion desiredDirection = Quaternion.LookRotation(lookAtDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredDirection, rotationSlerpSpeed * Time.deltaTime);
        }
        else if (playerInputManager.clampedDirection != Vector2.zero)
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

    public void HandleDodgeMovements()
    {
        if (playerInputManager.dodgePressed && playerManager.isGrounded && !playerManager.isPerformingAction)
        {
            string dashDirection = "DodgeFront";
            bool canRotateInInputDir = true;

            if (playerManager.playerCameraManager.isLockedOn)
            {
                if (playerInputManager.localInputDirection == Vector2.left)
                {
                    dashDirection = "DodgeLeft";
                }
                else if (playerInputManager.localInputDirection == Vector2.right)
                {
                    dashDirection = "DodgeRight";
                }
                else if (playerInputManager.localInputDirection == Vector2.down)
                {
                    dashDirection = "DodgeBack";
                }

                canRotateInInputDir = false;
            }

            playerManager.playerAnimationManager.ChangeRootMotionMultiplier(dashSpeed, 1f, dashSpeed);
            playerManager.playerAnimationManager.PlayActionAnimation(
                animationName: dashDirection,
                isPerformingAction: true,
                rotateTowardsPlayerInput: canRotateInInputDir
                );

            //playerManager.meshTrail.AttemptToExecuteDash();
        }
    }

    public void MoveTowardsPosition(Vector3 position, Quaternion rotation, float duration)
    {
        playerManager.characterController.enabled = false;
        transform.DOMove(position, duration).SetEase(Ease.OutSine).OnComplete( () => playerManager.characterController.enabled = true);
    }

    private void OnTriggerEnter(Collider hit) 
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            Vector3 collisionDir = hit.ClosestPoint(transform.position);

            if (transform.position.y > hit.transform.position.y + hit.bounds.size.y
                || transform.position.y < hit.bounds.min.y)
            {
                /*Vector3 pushDir = new Vector3(collisionDir.x, 0, collisionDir.z).normalized;
                playerManager.characterController.Move(pushDir * 1f * Time.deltaTime);*/

                if (playerManager.collisionCheckCoroutineActive)
                {
                    playerManager.StopEnemyCollisionCoroutine();
                }

                playerManager.IgnoreEnemyLayerCollision();
            }
        }

    }

    private void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if (!playerManager.collisionCheckCoroutineActive)
            {
                playerManager.AttemptToEnableEnemyCollision();
            }
        }
    }


}
