using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttack : PlayerCombatBaseState
{
    public PlayerCounterAttack(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation(
            animationName: "CounterAttack",
            isPerformingAction: true,
            applyRootMotion: true,
            rotateTowardsPlayerInput: !playerManager.playerCameraManager.isLockedOn, // do not follow input rotation when locked on
            canRotate: playerManager.playerCameraManager.isLockedOn, // allow lock on rotations to occur during attack
            canMove: false,
            useGravity: true);


        playerManager.playerCombatManager.canBreakShield = true;
        playerManager.playerHealthManager.isInvincible = true;
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
        playerManager.playerCombatManager.canBreakShield = false;
        playerManager.playerHealthManager.isInvincible = false;
    }
}
