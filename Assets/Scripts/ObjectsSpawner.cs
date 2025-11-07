using UnityEngine;
using System.Collections;

public class ObjectsSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject aRemplirSiHarvestable;   // Le prefab à respawn
    public float respawnTime = 5f;    // Temps en secondes avant réapparition

    private GameObject currentObject; // L'objet actuellement présent
    private Vector3 spawnPosition;    // Position d'apparition

    private void Start()
    {
        // On mémorise la position (locale au spawner)
        spawnPosition = transform.position;
        if (aRemplirSiHarvestable == null)
            aRemplirSiHarvestable = transform.GetChild(0).gameObject.GetComponent<Item>().itemData.prefab;
    }

    private void SpawnObject()
    {
        currentObject = Instantiate(aRemplirSiHarvestable, spawnPosition, aRemplirSiHarvestable.transform.rotation, transform);
        currentObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    // Appelé par l’objet lorsqu’il est ramassé
    public void OnObjectCollected()
    {
        // Lance la coroutine de respawn
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnObject();
    }
}
