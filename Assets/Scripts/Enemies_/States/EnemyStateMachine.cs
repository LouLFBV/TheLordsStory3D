using System.Collections.Generic;

public class EnemyStateMachine : BaseStateMachine<EnemyState, EnemyStateType>
{
    public EnemyStateMachine(Dictionary<EnemyStateType, EnemyState> allStates) : base(allStates) { }
}
public enum EnemyStateType
{
    Idle,
    Patrol,
    Follow,
    Attack,
    Hit,
    Stunned,
    Death
}