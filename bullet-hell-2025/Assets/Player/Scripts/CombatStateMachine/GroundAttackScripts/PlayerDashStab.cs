using AYellowpaper.SerializedCollections.KeysGenerators;
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

        //playerManager.playerAnimationManager.ChangeRootMotionMultiplier(1.5f, 1, 1.5f);

        // Setup Variables
        float elapsed = 0;
        float duration = 0.5f;
        float distance = 2f;
        float maxDistance = playerManager.playerCameraManager.lockOnRadius;
        playerManager.characterController.enabled = false;
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

        dashTween = DOTween.To(() => 0f, x =>
        {
            elapsed = x;
            Vector3 updatedStartPos = playerManager.transform.position;

            // Dynamically update end position if target is in range
            if (inRange)
            {
                endPosition = playerManager.playerCameraManager.currentLockOnTarget.position;
                endPosition = endPosition + (dirToPlayer * distance);
            }

            endPosition.y = updatedStartPos.y;
            playerManager.transform.position = Vector3.Lerp(updatedStartPos, endPosition, x / duration);

        }, duration, duration).OnComplete(() => playerManager.characterController.enabled = true);
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
