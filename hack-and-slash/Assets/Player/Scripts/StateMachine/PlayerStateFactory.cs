
public class PlayerStateFactory 
{
    PlayerStateMachine context;

    // States
    PlayerIdleState idleState;
    PlayerWalkState walkState;
    PlayerRunState runState;
    PlayerJumpState jumpState;
    PlayerGroundedState groundedState;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        context = currentContext;   

        idleState = new PlayerIdleState(context, this);
        walkState = new PlayerWalkState(context, this);
        runState = new PlayerRunState(context, this);
        jumpState = new PlayerJumpState(context, this);
        groundedState = new PlayerGroundedState(context, this);
    }

    public PlayerBaseState Idle()
    {
        return idleState;
    }

    public PlayerBaseState Walk()
    {
        return walkState;
    }

    public PlayerBaseState Run()
    {
        return runState;
    }

    public PlayerBaseState Jump()
    {
        return jumpState;
    }

    public PlayerBaseState Grounded()
    {
        return groundedState;
    }
}
