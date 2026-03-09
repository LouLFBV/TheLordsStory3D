using UnityEngine;
public class UIState : PlayerState
{
    public UIState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter(); 
        Time.timeScale = 0f;
        UIManagerSystem.instance.OpenPanel(player.RequestedPanelType);

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
        Time.timeScale = 1f;
        UIManagerSystem.instance.CloseAll();
        player.Input.SwitchActionMap("Player");
        UIManagerSystem.instance.ToggleCursor(false);
        InventorySystem.instance.itemActionsSystem.CloseActionPanel();
    }
}

public enum UIPanelType
{
    Inventory,
    Equipment,
    Map,
    Quests,
    PauseMenu,
    Options,
    Commandes,
    Dialogue,
    None
}