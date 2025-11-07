using UnityEngine;

[CreateAssetMenu(fileName = "New EnemySO")]
public class EnemySO : ScriptableObject
{
    public EnemyType enemyType;
    public int pvMax;
}

public enum EnemyType
{
    None,
    Loup,
    Squelette,
    Ours,
    Zombie,
    Boss
}