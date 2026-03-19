
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class NewEnemySO : ScriptableObject
{
    [Header("Permissions")]
    public bool canOrbit = true;
    public bool canFlank = false;
    public bool isAggressive = true; // Fonce directement ou non

    [Header("Orbit Settings")]
    public float idealOrbitDistance = 4f;
    public float orbitSpeedMultiplier = 1f;
}