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
        if (aRemplirSiHarvestable == null)
            aRemplirSiHarvestable = transform.GetChild(0).gameObject.GetComponent<Item>().itemData.prefab;
        spawnPosition = transform.GetChild(0).position;
    }

    private void SpawnObject()
    {
        currentObject = Instantiate(aRemplirSiHarvestable, spawnPosition, aRemplirSiHarvestable.transform.rotation, transform);
        currentObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void OnObjectCollected()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnObject();
    }
}
