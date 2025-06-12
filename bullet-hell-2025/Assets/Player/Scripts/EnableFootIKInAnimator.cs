using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnableFootIKInAnimator : StateMachineBehaviour
{
    private PlayerManager playerManager;
    private int runningHash;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerManager == null)
        {
            playerManager = animator.GetComponent<PlayerManager>();
        }
        runningHash = Animator.StringToHash("Base Layer.MovementState.NormalMovement.Movement.MovementBlend");
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!playerManager.playerMovementManager.isJumping)
            animator.SetBool("FootIK", true);
        else
            animator.SetBool("FootIK", false);

       /* Debug.Log("Current:" + animator.GetCurrentAnimatorStateInfo(0).IsTag("Movement"));
        Debug.Log("Want: " + runningHash);

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Movement"))
        {
            
            if (animator.GetBool("isMoving") && playerManager.footIK.onSlope)
            {
                animator.SetBool("FootIK", true);
            }
            else
            {
                animator.SetBool("FootIK", false);
            }
        }*/
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("FootIK", false);
    }
}
