using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem instance;

    [SerializeField]
    private Tooltip tooltip;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;  
    }
    public void Show(string content, string header = "")
    {

        if (tooltip == null)
        {
            return;
        }
        tooltip.SetText(content, header);
        tooltip.gameObject.SetActive(true);
    }

    public void Hide()
    {

        if (tooltip == null)
        {
            return;
        }
        tooltip.gameObject.SetActive(false);
    }
}
