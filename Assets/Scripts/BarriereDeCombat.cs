using UnityEngine;

public class BarriereDeCombat : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private string triggerName = "Up";

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public void UpBarriere() => _animator.SetTrigger(triggerName);
}
