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
    public CombatScriptableObj uppercutInfo;
    public CombatScriptableObj dashStabInfo;
    public CombatScriptableObj guardInfo;

    [Space(10)]
    public CombatScriptableObj jumpAttack1Info;
    public CombatScriptableObj jumpAttack2Info;
    public CombatScriptableObj jumpAttack3Info;
    public CombatScriptableObj jumpAttack4Info;
    public CombatScriptableObj jumpAttack5Info;
    public CombatScriptableObj jumpAttack6Info;
    public CombatScriptableObj helmBreakInfo;


    #endregion


    public PlayerCombatStateFactory factory;
    public PlayerCombatBaseState currentState;
    public CombatScriptableObj currentStateObj;

    [HideInInspector]
    public PlayerManager playerManager;

    // Damage Values
    public float damage;
    public float knockUpForce;
    public float knockBackForce;    

    private void Start()
    {
        playerManager = PlayerManager.instance;
        factory = new PlayerCombatStateFactory(this);

        currentState = factory.stateList[PlayerCombatState.Idle].combatState;
        currentStateObj = factory.stateList[PlayerCombatState.Idle].stateObj;
        playerManager.playerCombatManager.SetDamageValues(factory.stateList[PlayerCombatState.Idle].stateObj);
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
        currentStateObj = factory.stateList[switchState].stateObj;
        playerManager.playerCombatManager.SetDamageValues(factory.stateList[switchState].stateObj);
        currentState.EnterState();
    }

    public void SetDamageValues(CombatScriptableObj stateInfo)
    {
        damage = stateInfo.damageInfo.damage;
        knockUpForce = stateInfo.damageInfo.knockUpForce;
        knockBackForce = stateInfo.damageInfo.knockBackForce;
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


        // check inputs
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

            if (combatStateObject.requireDirectionalInput)
            {
                if (PlayerInputManager.instance.localInputDirection != combatStateObject.directionalInput)
                {
                    isValid = false;
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
