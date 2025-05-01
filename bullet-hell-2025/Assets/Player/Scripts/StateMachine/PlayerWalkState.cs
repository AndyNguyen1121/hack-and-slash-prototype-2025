using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine context, PlayerStateFactory playerStateFactory) : base(context, playerStateFactory) { }
    public override void OnStateEnter() { }

    public override void OnStateUpdate()
    {
        CheckSwitchStates();
    }

    public override void OnStateExit() { }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates() { }
}
