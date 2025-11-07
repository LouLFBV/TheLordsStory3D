using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToPreserve;
    private void Awake()
    {
        foreach (var obj in objectsToPreserve)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }
}
