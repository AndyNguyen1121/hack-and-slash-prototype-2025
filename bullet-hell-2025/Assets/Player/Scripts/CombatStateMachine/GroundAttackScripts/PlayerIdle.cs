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
        foreach (CombatScriptableObj criteria in factory.stateList[PlayerCombatState.Idle].stateInformation.nextStates)
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
