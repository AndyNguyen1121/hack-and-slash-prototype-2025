using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : PlayerCombatBaseState
{
    public PlayerIdle(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
        if (playerManager.playerCombatManager.canCounterAttack && PlayerInputManager.instance.attackPressed)
        {
            stateMachine.SwitchState(PlayerCombatState.CounterAttack);
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
        playerManager.playerCombatManager.elapsedFrames = 0;
        playerManager.playerCombatManager.parryWindowActive = false;
    }
}
