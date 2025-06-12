using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ikAdjustments : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerManager.instance.footIK.isRunning = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerManager.instance.footIK.isRunning = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        FootIK footIK = PlayerManager.instance.footIK;


        /*if (!footIK.onSlope)
            return;

        Vector3 localBodyPos = animator.transform.InverseTransformPoint(animator.bodyPosition);

        // Target local Y is the center.y
        float targetLocalY = footIK.characterController.center.y;

        // Smooth blend in local space
        localBodyPos.y = Mathf.Lerp(footIK.lastIkPosBeforeDisabled, targetLocalY, 20f * Time.deltaTime);
        footIK.lastPelvisYPos = localBodyPos.y;

        // Convert back to world space and assign
        animator.bodyPosition = animator.transform.TransformPoint(localBodyPos);*/
    }
}
