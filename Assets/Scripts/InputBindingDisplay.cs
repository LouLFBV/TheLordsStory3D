using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public static class InputBindingDisplay
{
    public static void UpdateDisplay(
        InputAction action,
        int bindingIndex,
        Image iconField)
    {
        if (action == null || bindingIndex < 0 || bindingIndex >= action.bindings.Count)
            return;

        var binding = action.bindings[bindingIndex];
        string path = binding.effectivePath;

        // -------------------------
        // 1. Pas de binding détecté
        // -------------------------
        if (string.IsNullOrEmpty(path))
        {
            iconField.enabled = false;
            return;
        }

        // -------------------------
        // 2. Tenter de récupérer une icône pour ce binding
        // -------------------------
        Sprite icon = InputIconDatabase.instance.GetIcon(path);

        if (icon != null)
        {
            iconField.sprite = icon;
            iconField.enabled = true;
            return;
        }

        // -------------------------
        // 3. Aucun icône ? on affiche du texte
        // -------------------------
        iconField.enabled = false;
    }
}
