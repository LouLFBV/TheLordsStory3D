using UnityEngine;

public class AttackState : PlayerState
{
    private AttackSO currentAttack;
    private float timer;
    private bool comboBuffered;

    public AttackState(PlayerController player) : base(player) { }

    public void SetAttack(AttackSO attack) => currentAttack = attack;

    public override void Enter()
    {
        timer = 0f;
        comboBuffered = false;

        // On utilise le Hash, c'est beaucoup plus fiable pour Unity
        player.Animator.CrossFadeInFixedTime(currentAttack.AnimationHash, 0.1f, 0, 0f);

        player.Animator.applyRootMotion = true;
        player.ConsumeStamina(currentAttack.staminaCost);
    }
    public override void Update()
    { 
        timer += Time.deltaTime;
        AnimatorStateInfo stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);

        // On ne check la fin que si l'Animator a BIEN commencé l'attaque
        bool isPlayingCorrectAnim = stateInfo.shortNameHash == currentAttack.AnimationHash;

        if (isPlayingCorrectAnim)
        {
            if (stateInfo.normalizedTime >= 0.95f)
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
        // On s'assure que le Root Motion ne reste pas bloqué selon tes besoins
    }
}