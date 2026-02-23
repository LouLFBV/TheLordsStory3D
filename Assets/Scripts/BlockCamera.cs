using System.Collections;
using UnityEngine;

public class BlockCamera : MonoBehaviour
{
    [Header("Camera Default Settings")]
    [SerializeField] private Vector2 defaultRotation = new Vector2(0f, 0f);
    // X = yaw (horizontal), Y = pitch (vertical)

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (other.TryGetComponent<BasicBehaviour>(out var player))
        {
            var cam = player.GetCamScript;
            if (cam.IsLocked)
            {
                cam.UnlockCamera();
            }
            else
            {
                cam.SetRotation(defaultRotation.x, defaultRotation.y);
                StartCoroutine(LockAfterDelay(player, 0.1f));
            }
        }
    }

    private IEnumerator LockAfterDelay(BasicBehaviour player, float delay)
    {
        yield return new WaitForSeconds(delay);
        player.GetCamScript.LockCamera();
    }
}