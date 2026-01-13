using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Data")]
    public ItemData itemData;
    public int amount = 1;

    [Header("Visual Floating")]
    public bool enableFloating = false;
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotationSpeed = 60f;

    private Vector3 startPosition;
    private float timeOffset;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 100f);

        if (rb != null)
        {
            rb.isKinematic = enableFloating;
            rb.useGravity = !enableFloating;
        }
    }

    private void Update()
    {
        if (!enableFloating)
            return;

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        float newY = startPosition.y + Mathf.Sin((Time.time + timeOffset) * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
