using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private InteractBehaviour InteractBehaviour;
    public void ReEnablePlayerMouvementFromInteractBehaviour() => InteractBehaviour.ReEnablePlayerMouvement();

    public void EnableTwoHandFromInteractBehaviour() => InteractBehaviour.EnableTwoHand();
    public void DisableTwoHandFromInteractBehaviour() => InteractBehaviour.DisableTwoHand();
    public void AddItemToInventoryFromInteractBehaviour() => InteractBehaviour.AddItemToInventory();
    public void PlayHarvestingSoundEffectFromInteractBehaviour() => InteractBehaviour.PlayHarvestingSoundEffect();
     
    public void BreakHarvestableFromInteractBehaviour() => StartCoroutine(InteractBehaviour.BreakHarvestable());
}
