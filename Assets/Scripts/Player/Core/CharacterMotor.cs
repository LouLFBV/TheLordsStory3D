using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Transform cameraTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        cameraTransform = ThirdPersonCameraController.Instance.GetTransform();
    }
    public void RotateTowardsInput(Vector2 input)
    {
        if (input == Vector2.zero)
            return;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = cameraTransform.right;
        right.y = 0;

        Vector3 direction = forward * input.y + right * input.x;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }

    public bool IsGrounded()
    {
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        Vector3 start = col.bounds.center;
        float radius = col.radius * 0.9f;
        float rayLength = (col.height / 2f) - radius + 0.2f;

        bool grounded = Physics.SphereCast(start, radius, Vector3.down, out _, rayLength, ~0, QueryTriggerInteraction.Ignore);

        Debug.DrawRay(start, Vector3.down * rayLength, grounded ? Color.green : Color.red);
        return grounded;
    }
}