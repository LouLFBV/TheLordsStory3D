using UnityEngine;

public class RollState : GroundedState
{
    private float rollDuration = 0.8f; // À ajuster selon la longueur de ton animation
    private float timer;

    public RollState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter: Roll");
        // 1. Consommation de Stamina (déjà vérifiée dans GroundedState avant de switch)
        player.Stamina.Spend(20f);

        // 2. Déclenchement Animation
        player.Animator.SetTrigger(AnimatorHashes.rollTrigger);
        player.Animator.applyRootMotion = true;

        // 3. Réduction du Collider (Ton ancienne logique)
        player.Motor.StartRollCollider();

        timer = 0;

        // 4. Rotation initiale : On oriente le joueur vers sa direction d'input
        // pour qu'il roule là où il veut aller, pas juste devant lui.
        RotateRollDirection();
    }

    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;

        // Sortie automatique de l'état après X temps 
        // (ou via un Animation Event "OnRollEnd")
        if (timer >= rollDuration)
        {
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // On remet le collider à sa taille normale
        player.Motor.EndRollCollider();
    }

    private void RotateRollDirection()
    {
        Vector2 input = player.Input.MoveInput;
        if (input.sqrMagnitude > 0.1f)
        {
            // On récupère la direction par rapport à la caméra
            Vector3 moveDir = player.Motor.GetDirectionFromInput(input);
            player.transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }
}