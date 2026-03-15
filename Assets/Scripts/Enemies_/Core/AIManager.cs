using UnityEngine;

public class AIManager : MonoBehaviour
{
    private EnemyController enemyController;
    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }
    private void Start()
    {
        // Initialize the first state, e.g., IdleState
        // enemyController.ChangeState(new IdleState(enemyController, enemyData));
    }
}