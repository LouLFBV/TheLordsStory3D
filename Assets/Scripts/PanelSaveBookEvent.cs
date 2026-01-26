using UnityEngine;

public class PanelSaveBookEvent : MonoBehaviour
{
    [SerializeField] private SaveBook saveBook;
    
    public void DesactivePanel()
    {
        saveBook.ClosePanel();
    }
}
