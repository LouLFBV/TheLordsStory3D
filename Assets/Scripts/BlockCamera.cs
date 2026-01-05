using UnityEngine;

public class BlockCamera : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered BlockCamera zone.");
        if (!other.CompareTag("Player"))
            return;

        if (other.TryGetComponent<BasicBehaviour>(out var player))
        {
            Debug.Log("Toggling camera lock.");
            if (player.GetCamScript.IsLocked)
                player.GetCamScript.UnlockCamera();
            else
                player.GetCamScript.LockCamera();
        }
    }
}
