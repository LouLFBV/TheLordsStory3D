using UnityEngine;

public class DestroyAllDontDestroyOnLoad : MonoBehaviour
{
    private void Start()
    {
        DestroyAllPersistentObjects();
    }

    private void DestroyAllPersistentObjects()
    {
        // Trouver tous les objets dans la scène actuelle
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            // Vérifier si l'objet appartient à la scène de "DontDestroyOnLoad"
            if (obj.scene.name == null || obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
            }
        }
    }
}
