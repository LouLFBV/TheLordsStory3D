using UnityEngine;
using System.Collections;

public class Campfire : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;
    [SerializeField] private float healInterval = 1f;

    private Coroutine healRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerStats>(out var player))
        {
            healRoutine = StartCoroutine(HealOverTime(player));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (healRoutine != null)
        {
            StopCoroutine(healRoutine);
            healRoutine = null;
        }
    }

    private IEnumerator HealOverTime(PlayerStats player)
    {
        while (player != null && !player.isDead)
        {
            player.ConsumeItem(healAmount);
            yield return new WaitForSeconds(healInterval);
        }
    }
}
