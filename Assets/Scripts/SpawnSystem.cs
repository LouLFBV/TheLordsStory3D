using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public sealed class SpawnSystem
{
    private readonly Dictionary<string, SpawnPoint> _spawnPoints = new();
    private string _pendingSpawnID;

    public SpawnSystem()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Register(SpawnPoint spawnPoint)
    {
        if (!_spawnPoints.ContainsKey(spawnPoint.SpawnID))
            _spawnPoints.Add(spawnPoint.SpawnID, spawnPoint);
        else
            Debug.LogWarning($"Duplicate SpawnID detected: {spawnPoint.SpawnID}");
    }

    public void Unregister(SpawnPoint spawnPoint)
    {
        if (_spawnPoints.ContainsKey(spawnPoint.SpawnID))
            _spawnPoints.Remove(spawnPoint.SpawnID);
    }

    public void SetSpawn(string spawnID)
    {
        _pendingSpawnID = spawnID;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(_pendingSpawnID))
            return;

        CoroutineRunner.Instance.StartCoroutine(SpawnNextFrame());
    }


    private IEnumerator SpawnNextFrame()
    {
        yield return null; // Attend 1 frame

        if (!_spawnPoints.TryGetValue(_pendingSpawnID, out SpawnPoint target))
        {
            Debug.LogWarning($"SpawnPoint '{_pendingSpawnID}' not found.");
            yield break;
        }

        PlayerStats player = PlayerStats.instance;

        if (player == null)
        {
            Debug.LogError("PlayerStats instance is null.");
            yield break;
        }
        player.transform.SetPositionAndRotation(
            target.transform.position,
            target.transform.rotation
        );
        _pendingSpawnID = null;
        _spawnPoints.Clear(); // optionnel mais propre
    }
}