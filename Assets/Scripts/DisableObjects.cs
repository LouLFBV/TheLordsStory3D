using UnityEngine;

public class DisableObjects : MonoBehaviour
{
    [SerializeField] GameObject objectToDisable;
    [SerializeField] bool disableOnStart = true;

    private void OnEnable()
    {
        if (objectToDisable != null && disableOnStart)
        {
            objectToDisable.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (objectToDisable != null && !disableOnStart)
        {
            objectToDisable.SetActive(false);
        }
    }
}
