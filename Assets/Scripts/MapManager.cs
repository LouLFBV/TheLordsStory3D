using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [SerializeField] private Transform iconeMapTransform;
    [SerializeField] private ListeMorceauxDeMap[] allListMap = new ListeMorceauxDeMap[5];
    [SerializeField] private GameObject iconeIfNoMap;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // SECURITE SI DOUBLON
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Nouvelle scène chargée : " + scene.name);
        SetMapActive(scene.name);
    }

    private void SetMapActive(string sceneName)
    {
        bool found = false;
        bool hasActiveMorceaux = false;

        foreach (var map in allListMap)
        {
            bool isCurrent = (map.nomDeScene == sceneName);

            if (isCurrent)
            {
                found = true;
                if (map.morceauxDeMap != null && map.morceauxDeMap.Count > 0)
                {
                    hasActiveMorceaux = true;
                }
            }
            foreach (var morceau in map.morceauxDeMap)
            {
                if (morceau != null)
                    morceau.SetActive(isCurrent);
            }
        }
        if (iconeIfNoMap != null)
        {
            bool shouldShow = !found || !hasActiveMorceaux;
            iconeIfNoMap.SetActive(shouldShow);
        }
    }


    public void AddIconeMap(GameObject iconeMapPrefab)
    {
        if (iconeMapTransform == null || iconeMapPrefab == null) return;

        GameObject newIcon = Instantiate(
            iconeMapPrefab,
            iconeMapTransform.position,
            Quaternion.identity,
            iconeMapTransform
        );

        string currentSceneName = SceneManager.GetActiveScene().name;

        foreach (var map in allListMap)
        {
            if (map.nomDeScene == currentSceneName)
            {
                map.morceauxDeMap.Add(newIcon);
                iconeIfNoMap.SetActive(false);
                return; 
            }
        }

        Debug.LogWarning("Aucune map associée à cette scène");
    }
}

[System.Serializable]
public class ListeMorceauxDeMap
{
    public string nomDeScene;
    public List<GameObject> morceauxDeMap;
}
