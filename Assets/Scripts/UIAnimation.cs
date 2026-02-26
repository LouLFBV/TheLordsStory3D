using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    
    public void AnimationHealthBar() => PlayerStats.instance.UpdateHealthBar();
}
