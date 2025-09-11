using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHelmbreak : PlayerCombatBaseState
{
    public PlayerHelmbreak(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation(
            animationName: "Helmbreak",
            isPerformingAction: true,
            applyRootMotion: true,
            rotateTowardsPlayerInput: !playerManager.playerCameraManager.isLockedOn, // do not follow input rotation when locked on
            canRotate: playerManager.playerCameraManager.isLockedOn, // allow lock on rotations to occur during attack
            canMove: false,
            useGravity: false);

        playerManager.playerMovementManager.isJumping = false;

        playerManager.IgnoreEnemyLayerCollision();
    }

    public override void UpdateState()
    {
        if (!playerManager.playerCameraManager.isLockedOn)
        {
            playerManager.canRotate = false;
        }

        playerManager.playerMovementManager.timeAboveGround = 0;
        foreach (CombatScriptableObj criteria in stateMachine.currentStateObj.nextStates)
        {
            if (stateMachine.ValidateCombatStateCriteria(criteria))
            {
                stateMachine.SwitchState(criteria.stateID);
            }
        }
    }

    public override void ExitState()
    {
        playerManager.AttemptToEnableEnemyCollision();
    }
}
