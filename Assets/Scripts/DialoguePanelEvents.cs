using UnityEngine;

public class DialoguePanelEvents : MonoBehaviour
{
    public void CloseDialoguePanel()
    {
        DialogueManager.instance.CloseDialoguePanel();
    }

    public void ClosePlayerPanel()
    {
        DialogueManager.instance.ClosePlayerPanel();
    }
}
