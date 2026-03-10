using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private PlayerController player;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private float interactRadius = 0.5f; // Largeur du SphereCast
    [SerializeField] private LayerMask interactableMask;

    [HideInInspector] public IInteractable currentTarget; 
    
    [SerializeField] private Transform playerTransform;


    //#region Player Input
    //[SerializeField] private PlayerInput playerInput;
    //private bool isInteracting;

    //private void OnEnable()
    //{
    //    playerInput.actions["Interact"].Enable();
    //    playerInput.actions["Interact"].performed += OnInteractPerformed;
    //    playerInput.actions["Interact"].canceled += OnInteractCanceled;
    //}

    //private void OnDisable()
    //{
    //    playerInput.actions["Interact"].Disable();
    //    playerInput.actions["Interact"].performed -= OnInteractPerformed;
    //    playerInput.actions["Interact"].canceled -= OnInteractCanceled;
    //}

    //private void OnInteractPerformed(InputAction.CallbackContext context) => isInteracting = true;
    //private void OnInteractCanceled(InputAction.CallbackContext context) => isInteracting = false;
    //#endregion

    private void Update()
    {
        DetectInteractable();

        if (player.Input.InteractPressed || player.Input.DialogueNextPressed)
        {
            Debug.Log("Attempting to interact with: " + (currentTarget != null ? currentTarget.ToString() : "nothing"));
            currentTarget?.OnInteract(this);
            player.Input.UseInteractInput();
            player.Input.UseDialogueNextInput();
        }
    }

    private void DetectInteractable()
    {
        if (Physics.SphereCast(transform.position, interactRadius, transform.forward, out RaycastHit hit, interactRange, interactableMask, QueryTriggerInteraction.Collide))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != currentTarget)
            {
                currentTarget?.SetTargeted(false, player.transform);
                currentTarget = interactable;
                currentTarget?.SetTargeted(true, player.transform);
            }
            Debug.Log($"Interactable detected: {hit.collider.name}");
        }
        else
        {
            currentTarget?.SetTargeted(false, player.transform);
            currentTarget = null;
        }
    }

    private void OnDrawGizmos()
    {
        // Visualisation du SphereCast
        Gizmos.color = Color.green;
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * interactRange;

        Gizmos.DrawLine(start, end); // Ligne de vue
        Gizmos.DrawWireSphere(start + transform.forward * interactRange * 0.5f, interactRadius); // Sphere au milieu approximatif
    }
}
