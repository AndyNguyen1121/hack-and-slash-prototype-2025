using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCombatBaseState
{
    protected PlayerCombatStateMachine stateMachine;
    protected PlayerCombatStateFactory factory;

    public PlayerCombatBaseState(PlayerCombatStateMachine stateMachine, PlayerCombatStateFactory factory)
    {
        this.stateMachine = stateMachine;  
        this.factory = factory;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}
