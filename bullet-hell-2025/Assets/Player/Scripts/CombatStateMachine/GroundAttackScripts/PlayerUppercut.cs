using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUppercut : PlayerCombatBaseState
{
    public PlayerUppercut(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.ChangeRootMotionMultiplier(1, 1.5f, 1);
        playerManager.playerAnimationManager.PlayActionAnimation(
            animationName: "Uppercut",
            isPerformingAction: true,
            applyRootMotion: true,
            rotateTowardsPlayerInput: !playerManager.playerCameraManager.isLockedOn, // do not follow input rotation when locked on
            canRotate: playerManager.playerCameraManager.isLockedOn, // allow lock on rotations to occur during attack
            canMove: false,
            useGravity: false);

        stateMachine.playerManager.playerMovementManager.isJumping = true;
        PlayerManager.instance.EnableEnemyLayerCollision();
    }

    public override void UpdateState()
    {
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

    }
}
