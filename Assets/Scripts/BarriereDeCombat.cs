using UnityEngine;

public class BarriereDeCombat : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void UpBarriere() => animator.SetTrigger("Up");
}
