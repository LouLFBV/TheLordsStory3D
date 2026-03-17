using UnityEngine;

public class UIAnimation : MonoBehaviour
{
    
    public void AnimationHealthBar() => PlayerUIManager.Instance.UpdateHealthBar();
}
