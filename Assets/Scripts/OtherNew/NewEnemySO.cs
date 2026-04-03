
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData")]
public class NewEnemySO : ScriptableObject
{
    public EnemyType enemyType;

    [Header("Permissions")]
    public bool canOrbit = true;
    public bool canFlank = false;
    public bool isAggressive = true; // Fonce directement ou non

    [Header("Orbit Settings")]
    public float idealOrbitDistance = 4f;
    public float orbitSpeedMultiplier = 1f;

    [Header("Vision")]
    public float visionRange = 10f;
    public float visionAngle = 60f;
}


public enum EnemyType
{
    None,
    Loup,
    Squelette,
    Ours,
    Gobelin,
    Boss,
    Ogre,
    ChevalierFantome,
    Araignee,
    Mimic
}