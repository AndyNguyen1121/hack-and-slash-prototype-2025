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
        if (!playerManager.isPerformingAction && PlayerInputManager.instance.attackPressed)
        {
            stateMachine.SwitchState(factory.Attack1());
        }
    }

    public override void ExitState()
    {
        
    }
}
