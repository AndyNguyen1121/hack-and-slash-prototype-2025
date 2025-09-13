using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAnimationManager : MonoBehaviour
{
    [Header("Root Motion Multipliers")]
    public float rootMotionSpeedMultiplierX = 1;
    public float rootMotionSpeedMultiplierY = 1;
    public float rootMotionSpeedMultiplierZ = 1;

    public PlayerInputManager playerInputManager;
    public PlayerManager playerManager;
    public Animator animator;
    Gamepad gamepad;

    public Coroutine delayCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        playerInputManager = PlayerInputManager.instance;
        playerManager = PlayerManager.instance;
        animator = playerManager.animator;

        gamepad = Gamepad.current;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAnimationMovementParameters(float horizontal, float vertical)
    {
        if (playerInputManager.clampedDirection == Vector2.zero && playerInputManager.joystickIdleTime > 0.075f)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }


        if (!playerManager.canMove)
            return; 

        animator.SetFloat("horizontal", horizontal, 1.2f, Time.deltaTime * 6f);
        animator.SetFloat("vertical", vertical, 1.2f, Time.deltaTime * 6f);
        
        
    }

    public void PlayActionAnimation(
        string animationName, 
        bool isPerformingAction, 
        bool applyRootMotion = true, 
        bool rotateTowardsPlayerInput = false,
        bool canRotate = false,
        bool canMove = false,
        bool useGravity = true,
        float normalizedTime = 0.1f)
    {

        if (rotateTowardsPlayerInput && playerInputManager.movementDirection != Vector2.zero)
        {
            Vector3 playerDirection = playerManager.mainCam.transform.right * playerInputManager.movementDirection.x;
            playerDirection += playerManager.mainCam.transform.forward * playerInputManager.movementDirection.y;

            playerDirection.y = 0;
            playerDirection.Normalize();

            StartCoroutine(SlerpDuringAction(Quaternion.LookRotation(playerDirection), 0.05f));
            //playerManager.transform.rotation = Quaternion.LookRotation(playerDirection);
        }
        animator.CrossFade(animationName, normalizedTime);
        playerManager.isPerformingAction = isPerformingAction;
        playerManager.canMove = canMove;
        playerManager.canRotate = canRotate;
        animator.applyRootMotion = applyRootMotion;
        playerManager.useGravity = useGravity;
    }

    public void ChangeRootMotionMultiplier(float x, float y, float z)
    {
        rootMotionSpeedMultiplierX = x;
        rootMotionSpeedMultiplierY = y;
        rootMotionSpeedMultiplierZ = z;
    }

    public IEnumerator SlerpDuringAction (Quaternion rotation, float dampTime)
    {
        float timer = 0;
        Quaternion initalRotation = transform.rotation;

        while (timer < dampTime)
        {
            playerManager.transform.rotation = Quaternion.Slerp(initalRotation, rotation, timer / dampTime);
            timer += Time.deltaTime;
            yield return null;
        }

    }

    private void OnAnimatorMove()
    {
        if (animator.applyRootMotion)
        {
            
            Vector3 velocity = animator.deltaPosition;
            velocity.x *= rootMotionSpeedMultiplierX;
            velocity.y *= rootMotionSpeedMultiplierY;
            velocity.z *= rootMotionSpeedMultiplierZ;



            if (playerManager.useGravity)
                velocity.y += playerManager.playerMovementManager.airGravityScale * Time.deltaTime;

            if (playerManager.characterController.enabled)
            {
                playerManager.characterController.Move(velocity);
            }
        }
    }

    public void HitStop()
    {
        StartCoroutine(ProcessHitStop(0.04f));

        if (gamepad != null)
            gamepad.SetMotorSpeeds(0.1f, 0.05f);
    }

    public IEnumerator ProcessHitStop(float time)
    {
        animator.speed = 0;

        yield return new WaitForSeconds(time);

        animator.speed = 1;
    }

    public bool CheckIfAnimationIsPlaying(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    #region AnimationEvents
    public void PerformingActionTrue()
    {
        playerManager.isPerformingAction = true;
    }
    public void PerformingActionFalse()
    {
        playerManager.isPerformingAction = false;
    }

    public void ActivateGravity()
    {
        playerManager.useGravity = true;
    }

    public void DeactivateGravity()
    {
        playerManager.useGravity = false;
    }

    /*public void CanAttackTrue()
    {
        playerManager.canAttack = true;
    }

    public void CanAttackFalse()
    {
        playerManager.canAttack = false; 
    }

    public void OpenWeaponCollider()
    {
        playerManager.weaponCollider.enabled = true;
    }

    public void CloseWeaponCollider()
    {
        playerManager.weaponCollider.enabled = false;
        playerManager.currentEquippedWeapon.ClearDamagedTargetList();
    }*/
    #endregion
}
