using UnityEngine;
public class UIState : PlayerState
{
    public UIState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        // 1. On ouvre visuellement l'inventaire
        InventorySystem.instance.OpenInventory();
        Debug.Log("Entered UI State: Inventory opened.");

        // 2. On configure l'input et la caméra (via ton SwitchActionMap)
        player.Input.SwitchActionMap("UI");
        UIManagerSystem.instance.ToggleCursor(true);
    }

    public override void Update()
    {
        // Si on réappuie sur Inventaire ou Cancel pendant qu'on est dans cet état
        if (player.Input.InventoryPressed || player.Input.CancelPressed)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // 1. On ferme visuellement
        InventorySystem.instance.CloseInventory();

        // 2. On rend les contrôles au joueur
        player.Input.SwitchActionMap("Player");
        UIManagerSystem.instance.ToggleCursor(false);
    }
}