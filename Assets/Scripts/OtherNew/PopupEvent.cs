using System;

public static class PopupEvent
{
    public static Action<string> OnPopupRequested;

    public static void Raise(string description)
    {
        OnPopupRequested?.Invoke(description);
    }
}