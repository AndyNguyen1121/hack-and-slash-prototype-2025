
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashStab : PlayerCombatBaseState
{
    public Tween dashTween;
    public PlayerDashStab(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation(
            animationName: "DashStab",
            isPerformingAction: true,
            applyRootMotion: false,
            rotateTowardsPlayerInput: !playerManager.playerCameraManager.isLockedOn, // do not follow input rotation when locked on
            canRotate: playerManager.playerCameraManager.isLockedOn, // allow lock on rotations to occur during attack
            canMove: false,
            useGravity: true);


        // Setup Variables
        float duration = 0.5f;
        float distanceToStopInFrontOfEnemy = 2f;
        float maxDistance = playerManager.playerCameraManager.lockOnRadius;
        playerManager.canMove = false;
        Vector3 endPosition = Vector3.zero;

        // Clamp to max distance if target not in range
        Vector3 dirToPlayer = playerManager.transform.position - playerManager.playerCameraManager.currentLockOnTarget.position;
        dirToPlayer.Normalize();

        float distanceFromPlayer = Vector3.Distance(playerManager.transform.position, playerManager.playerCameraManager.currentLockOnTarget.position);
        bool inRange = distanceFromPlayer <= playerManager.playerCameraManager.lockOnRadius;

        if (!inRange)
        {
            float distanceAwayFromEnemy = Mathf.Abs(distanceFromPlayer - playerManager.playerCameraManager.lockOnRadius);
            endPosition = playerManager.playerCameraManager.currentLockOnTarget.position + (distanceAwayFromEnemy * dirToPlayer);
        }
        Vector3 startPos = playerManager.transform.position;

        dashTween = DOTween.To(() => 0f, x =>
        {
            
            if (inRange)
            {
                endPosition = playerManager.playerCameraManager.currentLockOnTarget.position + (dirToPlayer * distanceToStopInFrontOfEnemy);
            }

            endPosition.y = startPos.y;
            Vector3 targetPos = Vector3.Lerp(startPos, endPosition, x / duration);

            Vector3 delta = targetPos - playerManager.transform.position;
            playerManager.characterController.Move(delta);

        }, duration, duration);
    }

    public override void UpdateState()
    {

        if (playerManager.playerCameraManager.currentLockOnTarget == null)
        {
            dashTween.Kill();
        }

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
