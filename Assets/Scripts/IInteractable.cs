using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Appelé quand le joueur se rapproche ou regarde l'objet
    /// </summary>
    /// <param name="isTargeted">true si ciblé, false si plus ciblé</param>
    void SetTargeted(bool isTargeted, Transform player);

    /// <summary>
    /// Appelé quand le joueur appuie sur la touche d'interaction
    /// </summary>
    /// <param name="player">Référence au joueur ou son script d’interaction</param>
    void OnInteract(PlayerInteractor player);
}
