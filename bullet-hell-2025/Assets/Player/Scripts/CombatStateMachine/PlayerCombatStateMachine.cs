using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class PlayerCombatStateMachine : MonoBehaviour
{
#region CombatStates
    public CombatScriptableObj idleInfo;
    public CombatScriptableObj attack1Info;
    public CombatScriptableObj attack2Info;
    public CombatScriptableObj attack3Info;
    public CombatScriptableObj attack4Info;

    [Space(10)]
    public CombatScriptableObj jumpAttack1;
    public CombatScriptableObj jumpAttack2;
    public CombatScriptableObj jumpAttack3;
    public CombatScriptableObj jumpAttack4;
    public CombatScriptableObj jumpAttack5;
    public CombatScriptableObj jumpAttack6;


    #endregion


    public PlayerCombatStateFactory factory;
    public PlayerCombatBaseState currentState;

    [HideInInspector]
    public PlayerManager playerManager;

    private void Start()
    {
        playerManager = PlayerManager.instance;
        factory = new PlayerCombatStateFactory(this);

        currentState = factory.stateList[PlayerCombatState.Idle].combatState;
        currentState.EnterState();
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    public void SwitchState(PlayerCombatState switchState)
    {
        currentState.ExitState();
        currentState = factory.stateList[switchState].combatState;
        currentState.EnterState();
    }

    public void GravitateTowardsPosition(Vector3 position, float distanceToStop, float minimumDistance, float maximumDistance, float duration)
    {
        if ((Vector3.Distance(position, transform.position) < minimumDistance) || (Vector3.Distance(position, transform.position) > maximumDistance))
            return;

        playerManager.characterController.enabled = false;

        Vector3 dirFromPlayerToTarget = (position - transform.position).normalized;
        Vector3 adjustedTargetPosition = position + (-dirFromPlayerToTarget * distanceToStop);

        DOTween.Sequence()
            .Append(transform.DOMove(adjustedTargetPosition, duration))
            .OnComplete(() => playerManager.characterController.enabled = true);


    }

    public bool ValidateCombatStateCriteria(CombatScriptableObj combatStateObject)
    {
        bool isValid = true;

        if ((!playerManager.isGrounded && combatStateObject.grounded) ||
            (playerManager.isGrounded && !combatStateObject.grounded) ||
            (playerManager.isPerformingAction && !combatStateObject.interruptPerformingAction) ||
            (!playerManager.playerCameraManager.isLockedOn && combatStateObject.onlyLockedOn))
        {
            isValid = false;
        }

        if (isValid)
        {
            foreach (var input in combatStateObject.requiredInputs)
            {
                if (!PlayerInputManager.instance.currentInputs.Contains(input))
                {
                    isValid = false;
                    break;
                }
            }
        }
        return isValid;
    }

    // grounded
    // directional and action input
    // isPerformingAction
    // state to transition to
    // performed if locked on only
}
