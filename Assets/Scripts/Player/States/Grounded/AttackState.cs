using UnityEngine;

public class AttackState : PlayerGroundedState
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
        timer = 0f;
        comboBuffered = false;

        // On utilise le Hash, c'est beaucoup plus fiable pour Unity
        // Play force l'animation instantanément sans transition
        player.Animator.Play(currentAttack.animationName, ATTACK_LAYER, 0f);

        player.Animator.applyRootMotion = true;
        //if (player.Stamina.CanSpend(currentAttack.staminaCost))
        //     player.Stamina.Spend(currentAttack.staminaCost);
        player.Animator.SetLayerWeight(10, 1f);
    }
    public override void Update()
    { 
        timer += Time.deltaTime;
        // --- AJOUTE CECI : DÉTECTION DU COMBO ---
        // Si le joueur clique et qu'on a dépassé la fenêtre de début de combo
        if (player.Input.AttackPressed && timer >= currentAttack.comboWindowStart)
        {
            comboBuffered = true;
            Debug.Log("Combo enregistré !"); // Petit log pour confirmer
        }
        // ----------------------------------------

        AnimatorStateInfo stateInfo = player.Animator.GetCurrentAnimatorStateInfo(ATTACK_LAYER);

        // On ne check la fin que si l'Animator a BIEN commencé l'attaque
        bool isPlayingCorrectAnim = stateInfo.shortNameHash == currentAttack.AnimationHash;

        if (isPlayingCorrectAnim)
        {
            // Au lieu de 0.95f (95% de l'anim), essaie 0.7f ou 0.8f
            if (stateInfo.normalizedTime >= 0.85f)
            {
                if (comboBuffered && currentAttack.nextAttack != null)
                {
                    SetAttack(currentAttack.nextAttack);
                    Enter(); // On relance la suivante
                }
                else
                {
                    Debug.Log("Fin de l'attaque, retour à Idle");
                    player.StateMachine.ChangeState(PlayerStateType.Idle);
                }
            }
        }
        else if (timer > 0.5f) // Sécurité : si après 0.5s on n'est toujours pas dans l'anim
        {
            Debug.LogError("L'animation n'a jamais démarré, sortie de secours.");
            player.StateMachine.ChangeState(PlayerStateType.Idle);
        }
    }

    public override void Exit()
    {
        // On dit à l'Animator d'arrêter d'afficher le Layer 10
        player.Animator.SetLayerWeight(10, 0f);
    }
}