using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    //[SerializeField] private EnemyData enemyData;
    //private EnemyState currentState;
    //private void Start()
    //{
    //    ChangeState(new IdleState(this, enemyData));
    //}
    //private void Update()
    //{
    //    currentState?.Update();
    //}
    //public void ChangeState(EnemyState newState)
    //{
    //    currentState?.Exit();
    //    currentState = newState;
    //    currentState.Enter();
    //}
}