using UnityEngine;

public class PaletteInputHandler : MonoBehaviour
{
    [SerializeField] private PaletteSlotManager slotManager;
    [SerializeField] private PaletteEquipmentManager equipmentManager;

    public void HandleInput(PlayerController player)
    {
        if (player.StateMachine.CurrentState is not GroundedState ||
            player.StateMachine.CurrentState is EquipState)
            return;

        if (player.Input.Weapon1Pressed)
            equipmentManager.ToggleWeapon(0, player);

        else if (player.Input.Weapon2Pressed)
            equipmentManager.ToggleWeapon(1, player);

        else if (player.Input.Object1Pressed)
            equipmentManager.ToggleObject(0, player);

        else if (player.Input.Object2Pressed)
            equipmentManager.ToggleObject(1, player);
    }
}