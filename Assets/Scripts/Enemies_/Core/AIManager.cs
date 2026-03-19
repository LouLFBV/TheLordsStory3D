using UnityEngine;

public class AIManager : MonoBehaviour
{
    private EnemyController _enemy;
    [SerializeField] private NewEnemySO enemyData; // Ta base de données

    public void Initialize(EnemyController owner)
    {
        _enemy = owner;
    }

    // Système de permission simple
    public bool CanOrbit => enemyData != null && enemyData.canOrbit;

    // Accès aux réglages du SO pour les états
    public float OrbitDistance => enemyData != null ? enemyData.idealOrbitDistance : 4f;
    public float OrbitSpeed => enemyData != null ? enemyData.orbitSpeedMultiplier : 1f;

    public NewEnemySO GetData() => enemyData;
    public bool HasPermission(EnemyStateType stateType)
    {
        if (enemyData == null) return false;

        return stateType switch
        {
            EnemyStateType.Orbit => enemyData.canOrbit,
            // Tu pourras ajouter d'autres cas ici plus tard
            _ => true
        };
    }
}