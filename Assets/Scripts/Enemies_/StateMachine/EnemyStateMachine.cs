using System.Collections.Generic;

public class EnemyStateMachine : StateMachine<EnemyState, EnemyStateType>
{
    public EnemyStateMachine(Dictionary<EnemyStateType, EnemyState> allStates) : base(allStates) { }
}
public enum EnemyStateType
{
    Idle,
    Patrol,
    Follow,
    Orbit,
    Attack,
    Hit,
    Stunned,
    Death
}