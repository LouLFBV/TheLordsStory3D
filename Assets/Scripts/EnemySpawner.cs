using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Paramètres du spawn")]
    public GameObject enemyPrefab;       // Le prefab de ton ennemi
    public int numberOfEnemies = 3;      // Combien d'ennemis spawnent
    public float spawnRadius = 5f;       // Zone où ils apparaissent autour du spawner

    private List<GameObject> activeEnemies = new List<GameObject>();

    // Quand le joueur entre dans la zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnEnemies();
        }
    }

    // Quand le joueur sort de la zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DestroyEnemies();
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Position aléatoire autour du spawner
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPos.y = transform.position.y; // garder sur le sol

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            activeEnemies.Add(enemy);
        }
    }

    private void DestroyEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
    }
}
