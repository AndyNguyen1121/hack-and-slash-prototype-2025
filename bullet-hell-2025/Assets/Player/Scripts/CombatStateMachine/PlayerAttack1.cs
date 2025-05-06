using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack1 : PlayerCombatBaseState
{
    public PlayerAttack1(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation("Attack1", true, true, true, false, false, true);
    }

    public override void UpdateState()
    {
        if (!playerManager.isPerformingAction && PlayerInputManager.instance.attackPressed)
        {
            stateMachine.SwitchState(factory.Attack2());
        }
    }

    public override void ExitState()
    {

    }
}
