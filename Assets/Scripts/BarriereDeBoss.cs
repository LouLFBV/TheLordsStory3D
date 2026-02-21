using UnityEngine;

public class BarriereDeBoss : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        MonterBarriere();
    }

    public void MonterBarriere()
    {
        _animator.SetTrigger("Monter");
    }
}
