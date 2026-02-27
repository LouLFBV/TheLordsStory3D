using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public static ThirdPersonCameraController Instance { get; private set; }

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Orbit Settings")]
    [SerializeField] private float distance = 3f;
    [SerializeField] private float height = 1.7f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float verticalSpeed = 120f;

    [Header("Vertical Clamp")]
    [SerializeField] private float minVerticalAngle = -40f;
    [SerializeField] private float maxVerticalAngle = 60f;

     public float SprintFOV { get; private set; }
    [SerializeField] private float sprintFOV = 80f;
    private float defaultFOV;                                          // Champ de vision (Field of View) par défaut.
    private float targetFOV;                                           // Champ de vision cible.

    [SerializeField] private float fovLerpSpeed = 8f;
    private Camera _camComponent;

    private PlayerInputHandler input;
    private float yaw;
    private float pitch;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (target == null)
            Debug.LogError("ThirdPersonCameraController: Target not assigned!");

        input = target.GetComponent<PlayerInputHandler>();
        _camComponent = GetComponent<Camera>();
        SprintFOV = sprintFOV;

        defaultFOV = _camComponent.fieldOfView;
        targetFOV = defaultFOV;
        yaw = target.eulerAngles.y;
    }

    private void LateUpdate()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    private void HandleInput()
    {
        Vector2 look = input.mouseLook + input.gamepadLook;

        yaw += look.x * rotationSpeed * Time.deltaTime;
        pitch -= look.y * verticalSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 pivot = target.position + Vector3.up * height;


        // Met à jour le champ de vision.
        _camComponent.fieldOfView = Mathf.Lerp(_camComponent.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);

        // position caméra derrière le pivot
        Vector3 desiredPosition = pivot - rot * Vector3.forward * distance;

        transform.position = desiredPosition;
        transform.LookAt(pivot);
    }
    public Transform GetTransform()
    {
        return transform;
    }
    // Définit un champ de vision personnalisé.
    public void SetFOV(float customFOV)
    {
        this.targetFOV = customFOV;
    }

    // Réinitialise le champ de vision à la valeur par défaut.
    public void ResetFOV()
    {
        this.targetFOV = defaultFOV;
    }
}