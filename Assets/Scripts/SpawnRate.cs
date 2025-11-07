using UnityEngine;

public class SpawnRate : MonoBehaviour
{
    [Header("Spawn Rate Settings (0 -> 100)")]
    [SerializeField] private int spawnRate; 
    void Start()
    {
        int randomValue = Random.Range(0, 100);
        if (randomValue > spawnRate)
        {
            GameObject parent = transform.parent.gameObject;
            Destroy(parent);
        }
    }
}
