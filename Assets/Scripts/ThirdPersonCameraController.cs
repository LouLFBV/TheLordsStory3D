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

        // position caméra derrière le pivot
        Vector3 desiredPosition = pivot - rot * Vector3.forward * distance;

        transform.position = desiredPosition;
        transform.LookAt(pivot);
    }
    public Transform GetTransform()
    {
        return transform;
    }
}