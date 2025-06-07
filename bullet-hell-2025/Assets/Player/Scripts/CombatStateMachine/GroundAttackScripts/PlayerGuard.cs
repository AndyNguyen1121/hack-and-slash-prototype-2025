using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGuard : PlayerCombatBaseState
{
    public PlayerGuard(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation(
            animationName: "GuardStart",
            isPerformingAction: true,
            applyRootMotion: false,
            rotateTowardsPlayerInput: !playerManager.playerCameraManager.isLockedOn, // do not follow input rotation when locked on
            canRotate: false, // allow lock on rotations to occur during attack
            canMove: false,
            useGravity: true);
    }

    public override void UpdateState()
    {
 /*       foreach (CombatScriptableObj criteria in stateMachine.currentStateObj.nextStates)
        {
            if (stateMachine.ValidateCombatStateCriteria(criteria))
            {
                stateMachine.SwitchState(criteria.stateID);
            }
        }*/

        if (PlayerInputManager.instance.isGuarding)
        { 
            playerManager.animator.SetBool("isGuarding", true);

        }
        else
        {
            playerManager.animator.SetBool("isGuarding", false);
        }
    }

    public override void ExitState()
    {

    }
}
