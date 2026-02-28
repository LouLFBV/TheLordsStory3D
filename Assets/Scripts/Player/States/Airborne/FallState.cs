using UnityEngine;

public class FallState : AirborneState
{
    private float fallTimer;
    private float threshold = 0.15f; // Le temps d'attente "Juice"
    private bool animTriggered;

    public FallState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        fallTimer = 0;
        animTriggered = false;
        // On ne trigger rien ici !
    }

    public override void Update()
    {
        base.Update();

        if (!animTriggered)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer > threshold)
            {
                player.Animator.SetBool("IsFalling", true);
                animTriggered = true;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.Animator.SetBool("IsFalling", false);
    }
}