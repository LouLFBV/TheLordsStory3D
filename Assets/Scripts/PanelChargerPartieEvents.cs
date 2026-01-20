using UnityEngine;

public class PanelChargerPartieEvents : MonoBehaviour
{
    public void OnOpenAnimationFinished()
    {
        Menu.Instance.OnOpenAnimationFinished();
    }
}
