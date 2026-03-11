using UnityEngine;

public static class AnimatorHashes
{
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int rollTrigger = Animator.StringToHash("ForwardRoll");
    public static readonly int Hit = Animator.StringToHash("Hit");
    public static readonly int ComboIndex = Animator.StringToHash("ComboIndex");

    public static readonly int hHash = Animator.StringToHash("H");
    public static readonly int vHash = Animator.StringToHash("V");
    public static readonly int speedHash = Animator.StringToHash("Speed");
    public static readonly int chargeBool = Animator.StringToHash("ChargeBow");
}