using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private int comboIndex;
    private bool canQueueNextAttack;
    public void PerformAttack()
    {
        animator.applyRootMotion = true;
        animator.SetInteger("ComboIndex", comboIndex);
        animator.SetTrigger(AnimatorHashes.Attack);

        canQueueNextAttack = false;
    }

    public void EnableComboWindow()
    {
        canQueueNextAttack = true;
    }

    public void TryQueueNextAttack()
    {
        if (!canQueueNextAttack)
            return;

        comboIndex++;
        PerformAttack();
    }

    public void ResetCombo()
    {
        comboIndex = 0;
        animator.applyRootMotion = false;
    }

    public void EndAttack()
    {
        animator.applyRootMotion = false;
    }
}