using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack2 : PlayerCombatBaseState
{
    public PlayerAttack2(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory) : base(stateMachine, factory) { }
    public override void EnterState()
    {
        playerManager.playerAnimationManager.PlayActionAnimation("Attack2", true, true, true, false, false, true);
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }
}
