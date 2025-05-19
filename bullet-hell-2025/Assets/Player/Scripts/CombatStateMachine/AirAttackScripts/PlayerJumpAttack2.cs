using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttack2 : PlayerCombatBaseState
{
    public PlayerJumpAttack2(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation(
            animationName: "JumpAttack2",
            isPerformingAction: true,
            applyRootMotion: true,
            rotateTowardsPlayerInput: !playerManager.playerCameraManager.isLockedOn, // do not follow input rotation when locked on
            canRotate: playerManager.playerCameraManager.isLockedOn, // allow lock on rotations to occur during attack
            canMove: false,
            useGravity: false);

    }

    public override void UpdateState()
    {
        foreach (CombatScriptableObj criteria in factory.stateList[PlayerCombatState.JumpAttack2].stateInformation.nextStates)
        {
            if (stateMachine.ValidateCombatStateCriteria(criteria))
            {
                stateMachine.SwitchState(criteria.stateID);
            }
        }
    }

    public override void ExitState()
    {

    }
}
