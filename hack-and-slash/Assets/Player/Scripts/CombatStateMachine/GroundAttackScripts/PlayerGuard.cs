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
            useGravity: true,
            normalizedTime: 0.01f);

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

        if (playerManager.playerCombatManager.elapsedFrames <= playerManager.playerCombatManager.windowFrames)
        {
            playerManager.playerCombatManager.parryWindowActive = true;
        }
        else
        {
            playerManager.playerCombatManager.parryWindowActive = false;
        }

        if (playerManager.playerCombatManager.canCounterAttack && PlayerInputManager.instance.attackPressed && playerManager.isGrounded)
        {
            stateMachine.SwitchState(PlayerCombatState.CounterAttack);
        }
    }

    public override void ExitState()
    {
        playerManager.playerCombatManager.elapsedFrames = 0;
        playerManager.playerCombatManager.parryWindowActive = false;
    }
}
