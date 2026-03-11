using UnityEngine;

public class AttackState : GroundedState
{
    private float timer;
    private bool comboBuffered;
    private const int ATTACK_LAYER = 9; // On définit le layer une fois pour toutes
    public AttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        base.Enter();

        player.Animator.SetLayerWeight(ATTACK_LAYER, 1f);
        // 1. Sécurité : Vérifier si l'arme est nulle
        if (player.PendingWeaponItem == null)
        {
            Debug.LogWarning("Tentative d'attaque sans arme assignée dans PendingWeaponItem !");
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

        // Détection du Buffer
        if (player.Input.AttackPressed)
        {
            if (timer >= player.CurrentAttack.comboWindowStart && timer <= player.CurrentAttack.comboWindowEnd)
            {
                comboBuffered = true;
                Debug.Log("Combo Bufferisé !");
            }
        }

        // ON RÉCUPÈRE L'INFO DE L'ANIMATION ACTUELLE
        var stateInfo = player.Animator.GetCurrentAnimatorStateInfo(ATTACK_LAYER);

        // Vérifier si l'animation est finie
        // On utilise normalizerTime qui va de 0 à 1 (1 = 100% de l'anim)
        if (timer >= stateInfo.length)
        {
            if (comboBuffered && player.CurrentAttack.nextAttack != null)
            {
                // TRANSITION VERS COMBO
                player.CurrentAttack = player.CurrentAttack.nextAttack;

                // On ne change pas d'état (on y est déjà), on reset juste le nécessaire
                ResetForNextCombo();
            }
            else
            {
                // FIN DU COMBO -> RETOUR IDLE
                player.CurrentAttack = null;
                player.StateMachine.ChangeState(PlayerStateType.Idle);
            }
        }
    }

    // Nouvelle petite méthode pour fluidifier le combo
    private void ResetForNextCombo()
    {
        timer = 0f;
        comboBuffered = false;

        // On force l'animation suivante immédiatement
        player.Animator.Play(player.CurrentAttack.AnimationHash, ATTACK_LAYER, 0f);
        Debug.Log("Lancement de l'attaque suivante : " + player.CurrentAttack.name);
    }

    public override void Exit()
    {
        // On dit à l'Animator d'arrêter d'afficher le Layer 10
        player.Animator.SetLayerWeight(ATTACK_LAYER, 0f);
    }
}