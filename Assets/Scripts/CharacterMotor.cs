//using UnityEngine;

//public class CharacterMotor : MonoBehaviour
//{
//    [SerializeField] private Rigidbody rb;
//    [SerializeField] private float moveSpeed = 5f;
//    [SerializeField] private float rotationSpeed = 10f;

//    [SerializeField] private Transform cameraTransform;

//    private CapsuleCollider capsule;
//    private float originalHeight;
//    private Vector3 originalCenter;

//    private void Awake()
//    {
//        capsule = GetComponent<CapsuleCollider>();
//        originalHeight = capsule.height;
//        originalCenter = capsule.center;
//    }

//    public void Move(Vector2 input)
//    {
//        Vector3 forward = cameraTransform.forward;
//        forward.y = 0;
//        forward.Normalize();

//        Vector3 right = cameraTransform.right;
//        right.y = 0;

//        Vector3 direction = forward * input.y + right * input.x;


//        if (direction.sqrMagnitude > 0.01f)
//        {
//            direction.Normalize();

//            Quaternion targetRotation = Quaternion.LookRotation(direction);
//            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

//            rb.linearVelocity = direction * moveSpeed + Vector3.up * rb.linearVelocity.y;
//        }
//        else
//        {
//            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
//        }
//    }

//    public void EnableRollCollider(bool enabled)
//    {
//        if (enabled)
//        {
//            capsule.height = originalHeight * 0.5f;
//            capsule.center = originalCenter + Vector3.down * 0.4f;
//        }
//        else
//        {
//            capsule.height = originalHeight;
//            capsule.center = originalCenter;
//        }
//    }
//}

using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    private Transform _cameraTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _cameraTransform = ThirdPersonCameraController.Instance.GetTransform();
    }
    public void RotateTowardsInput(Vector2 input)
    {
        if (input == Vector2.zero)
            return;

        Vector3 forward = _cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = _cameraTransform.right;
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
}