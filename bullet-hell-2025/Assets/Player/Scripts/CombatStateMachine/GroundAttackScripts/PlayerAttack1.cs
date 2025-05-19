using EditorAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttack1 : PlayerCombatBaseState
{
    public PlayerAttack1(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation(
            animationName: "Attack1",
            isPerformingAction: true, 
            applyRootMotion: true, 
            rotateTowardsPlayerInput: !playerManager.playerCameraManager.isLockedOn, // do not follow input rotation when locked on
            canRotate: playerManager.playerCameraManager.isLockedOn, // allow lock on rotations to occur during attack
            canMove: false, 
            useGravity: true);

        if (playerManager.playerCameraManager.isLockedOn)
        {
            Vector3 enemyPosition = playerManager.playerCameraManager.currentLockOnTarget.position;
            enemyPosition = new Vector3(enemyPosition.x, playerManager.transform.position.y, enemyPosition.z);

            stateMachine.GravitateTowardsPosition(
                position: enemyPosition, 
                distanceToStop: 1.5f, 
                minimumDistance: 1.5f, 
                maximumDistance: 5f, 
                duration: 0.1f);
        }
    }

    public override void UpdateState()
    {
        foreach (CombatScriptableObj criteria in factory.stateList[PlayerCombatState.Attack1].stateInformation.nextStates)
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
