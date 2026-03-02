using UnityEngine;

public class AttackState : GroundedState
{
    private AttackSO currentAttack;
    private float timer;
    private bool comboBuffered;
    private const int ATTACK_LAYER = 10; // On définit le layer une fois pour toutes
    public AttackState(PlayerController player) : base(player) { }

    public void SetAttack(AttackSO attack) => currentAttack = attack;

    public override void Enter()
    {
        base.Enter();
        // 1. Sécurité : Vérifier si l'arme est nulle
        if (player.PendingWeaponItem == null)
        {
            Debug.LogError("Tentative d'attaque sans arme assignée dans PendingWeaponItem !");
            player.StateMachine.ChangeState(PlayerStateType.Idle);
            return;
        }
        // 1. On récupère l'arme et ses données de combat
        // Supposons que player.EquippedWeapon est l'ItemData actuel
        WeaponCombatData combatData = player.PendingWeaponItem.combatData;

        if (combatData == null)
        {
            Debug.LogError("Cette arme n'a pas de WeaponCombatData !");
            player.StateMachine.ChangeState(PlayerStateType.Idle);
            return;
        }

        // 2. Si c'est le début du combo, on prend la startingAttack
        if (player.CurrentAttack == null)
        {
            player.CurrentAttack = combatData.startingAttack;
        }

        // 3. On joue l'animation
        // On utilise le Hash pour la performance
        player.Animator.Play(player.CurrentAttack.AnimationHash, ATTACK_LAYER, 0f);

        // Initialisation du buffer pour le prochain coup
        timer = 0f;
        comboBuffered = false;
        player.Animator.applyRootMotion = true;
    }

    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;

        // Détection du "Buffer" (clic pendant l'attaque actuelle)
        // On vérifie si on est dans la fenêtre définie dans l'AttackSO
        if (player.Input.AttackPressed) // Remplace par ton système d'input
        {
            if (timer >= player.CurrentAttack.comboWindowStart && timer <= player.CurrentAttack.comboWindowEnd)
            {
                comboBuffered = true;
                Debug.Log("Combo Bufferisé !");
            }
        }

        // Si l'animation est terminée (ou presque)
        if (timer >= player.Animator.GetCurrentAnimatorStateInfo(ATTACK_LAYER).length)
        {
            if (comboBuffered && player.CurrentAttack.nextAttack != null)
            {
                // On passe à l'attaque suivante définie dans le SO
                player.CurrentAttack = player.CurrentAttack.nextAttack;
                player.StateMachine.ChangeState(PlayerStateType.Attack); // On relance l'état
            }
            else
            {
                // Fin du combo, on reset et on rentre en Idle
                player.CurrentAttack = null;
                player.StateMachine.ChangeState(PlayerStateType.Idle);
            }
        }
    }

    public override void Exit()
    {
        // On dit à l'Animator d'arrêter d'afficher le Layer 10
        player.Animator.SetLayerWeight(10, 0f);
    }
}