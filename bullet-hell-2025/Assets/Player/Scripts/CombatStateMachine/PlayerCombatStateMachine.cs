using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatStateMachine : MonoBehaviour
{
    public PlayerCombatStateFactory factory;

    public PlayerCombatBaseState currentState;

    private void Awake()
    {
        factory = new PlayerCombatStateFactory(this);

        currentState = factory.Idle();
        currentState.EnterState();
    }

    private void Update()
    {
        currentState.UpdateState();
    }

    public void SwitchState(PlayerCombatBaseState newState)
    {
        currentState.ExitState();
        newState.EnterState();
        currentState = newState;
    }
}
