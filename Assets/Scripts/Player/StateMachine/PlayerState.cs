public abstract class PlayerState : State
{
    protected PlayerController player;
    protected PlayerState(PlayerController player)
    {
        this.player = player;
    }
}