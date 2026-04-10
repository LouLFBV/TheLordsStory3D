using UnityEngine;

public class PlayerFallState : PlayerAirborneState
{
    private float fallTimer;
    private float threshold = 0.15f;
    private bool animTriggered;

    // --- REGLAGES ---
    private float fallMultiplier = 1.5f; // Plus ce chiffre est haut, plus il tombe lourdement

    public PlayerFallState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        fallTimer = 0;
        animTriggered = false;
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

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // On applique une force supplémentaire vers le bas (Gravité custom)
        // Physics.gravity.y est généralement -9.81
        // (fallMultiplier - 1) car la gravité normale s'applique déjà de son côté
        Vector3 extraFallForce = Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;

        player.Rigidbody.linearVelocity += extraFallForce;
    }

    public override void Exit()
    {
        base.Exit();
        player.Animator.SetBool("IsFalling", false);
    }
}