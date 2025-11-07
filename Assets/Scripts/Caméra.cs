using UnityEngine;

/// <summary>
/// Caméra 3ᵉ personne “orbite + suivi”
/// – Pivot automatique autour du joueur (rotation souris)
/// – Zoom molette (distance min/max)
/// – Collision avec le décor (évite de traverser les murs)
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{

    public static ThirdPersonCamera instance; // Singleton (optionnel, pour accès rapide)

    [Header("Cible à suivre")]
    public Transform target;                // généralement le transform du Player
    public Vector3 pivotOffset = new Vector3(0f, 1.6f, 0f); // hauteur des yeux

    [Header("Rotation")]
    public float mouseSensitivity = 120f;   // vitesse de rotation
    public float minPitch = -35f;           // angle vertical min
    public float maxPitch = 60f;           // angle vertical max

    [Header("Zoom")]
    public float distance = 4f;             // distance de départ
    public float minDistance = 2f;
    public float maxDistance = 6f;
    public float zoomSpeed = 2f;            // vitesse de zoom molette

    [Header("Collision (caméra)")]
    public LayerMask collisionMask;         // couches à éviter (Walls, Default, etc.)
    public float sphereRadius = 0.25f;      // rayon pour le Cast

    /*--------------- privé ---------------*/
    float yaw = 0f;   // rotation horizontale (°)
    float pitch = 0f;   // rotation verticale   (°)

    public bool canMove = true; // Permet de désactiver le mouvement de la caméra

    private void Awake()
    {
        // Singleton (optionnel, pour accès rapide)
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (target == null) Debug.LogError("ThirdPersonCamera ▶ Aucun target assigné !");
        // Initialise la rotation à l’angle de départ
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        // Bloque le curseur (optionnel)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null || !canMove) return;

        /*--- 1. Lecture inputs rotation / zoom ---*/
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);

        /*--- 2. Calcul position désirée ---*/
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + pivotOffset;
        Vector3 desired = pivot - rotation * Vector3.forward * distance;

        /*--- 3. Collision anticlip ---*/
        if (Physics.SphereCast(pivot, sphereRadius, desired - pivot,
                               out RaycastHit hit, distance, collisionMask))
        {
            desired = pivot - rotation * Vector3.forward * (hit.distance - 0.05f);
        }

        /*--- 4. Applique position + rotation ---*/
        transform.SetPositionAndRotation(desired, rotation);
    }
}
