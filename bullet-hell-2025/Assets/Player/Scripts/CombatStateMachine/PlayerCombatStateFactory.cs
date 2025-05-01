using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatStateFactory : MonoBehaviour
{
    public PlayerCombatStateMachine stateMachine;

    //States
    private PlayerIdle idle;
    private PlayerAttack1 attack1;

    public PlayerCombatStateFactory(PlayerCombatStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

        attack1 = new PlayerAttack1(stateMachine, this);
        idle = new PlayerIdle(stateMachine, this);
    }

    public PlayerCombatBaseState Idle()
    {
        return idle;
    }
    public PlayerCombatBaseState Attack1()
    {
        return attack1;
    }
}
