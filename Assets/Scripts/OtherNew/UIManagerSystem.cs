using UnityEngine;

public class UIManagerSystem : MonoBehaviour
{
    public static UIManagerSystem instance;

    // On garde les valeurs en mémoire pour les rétablir plus tard
    private float _defaultRotationSpeed;
    private float _defaultVerticalSpeed;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // On récupère les vitesses de base via l'Instance de la caméra
        if (ThirdPersonCameraController.Instance != null)
        {
            _defaultRotationSpeed = ThirdPersonCameraController.Instance.RotationSpeed;
            _defaultVerticalSpeed = ThirdPersonCameraController.Instance.VerticalSpeed;
        }
    }

    public void ToggleCursor(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;

        // Si l'UI est visible, vitesse = 0. Sinon, on remet les vitesses par défaut.
        if (ThirdPersonCameraController.Instance != null)
        {
            ThirdPersonCameraController.Instance.RotationSpeed = isVisible ? 0f : _defaultRotationSpeed;
            ThirdPersonCameraController.Instance.VerticalSpeed = isVisible ? 0f : _defaultVerticalSpeed;
        }
    }
}