using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

// MoveBehaviour hérite de GenericBehaviour. Ce script gère le déplacement du joueur sans Root Motion.
public class MoveBehaviour : GenericBehaviour
{
    #region Champs/Paramètres
    [Header("Vitesses de déplacement")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 4.0f;
    public float sprintSpeed = 7.0f;

    public float acceleration = 10f;     // Accélération pour rendre le déplacement fluide.
    public float deceleration = 15f;     // Décélération quand le joueur relâche les touches.

    public float H => moveInput.x;
    public float V => moveInput.y;
    private int hFloat;                                   // Paramètre Animator lié à l’axe horizontal.
    private int vFloat;                                   // Paramètre Animator lié à l’axe vertical.

    [Header("Réglages divers")]
    public float speedDampTime = 0.1f;   // Damping de l'animation.
    public bool canMove = true;          // Si le joueur peut se déplacer.
    public float sprintFOV = 100f;                        // Champ de vision de la caméra quand le joueur sprinte.
    [SerializeField] private ThirdPersonOrbitCamBasic camScript; // Référence au script de la caméra.
    [HideInInspector] public bool changedFOV;                              // Indique si le champ de vision a été modifié à cause du sprint.
    public bool isSprinting;

    [HideInInspector] public float speed;       // Vitesse réelle actuelle.
    [HideInInspector] public float speedSeeker; // Cible de vitesse.

    private AttackBehaviour attackBehaviour;
    private AimBehaviourBasic aimBehaviour;
    private JumpBehaviour jumpBehaviour;
    private Rigidbody rb;


    //Nouveau système Input
    [SerializeField] private PlayerInput playerInput;
    private Vector2 moveInput;
    private bool sprintInput;

    #endregion

    private void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
    }
    #region PlayerInput Méthodes
    private void OnEnable()
    {
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMoveCanceled;

        playerInput.actions["Sprint"].performed += OnSprint;
        playerInput.actions["Sprint"].canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMoveCanceled;

        playerInput.actions["Sprint"].performed -= OnSprint;
        playerInput.actions["Sprint"].canceled -= OnSprintCanceled;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnSprint(InputAction.CallbackContext ctx)
    {
        sprintInput = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        sprintInput = false;
    }
    #endregion

    void Start()
    {
        behaviourManager.SubscribeBehaviour(this);
        behaviourManager.RegisterDefaultBehaviour(this.behaviourCode);

        rb = behaviourManager.GetRigidBody;
        speedSeeker = runSpeed;

        hFloat = Animator.StringToHash("H");
        vFloat = Animator.StringToHash("V");

        attackBehaviour = GetComponent<AttackBehaviour>();
        aimBehaviour = GetComponent<AimBehaviourBasic>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
    }

    void Update()
    {
        // --- GESTION HORIZONTALE / VERTICALE POUR L'ANIMATOR ---

        float h = !canMove ? 0f : moveInput.x;
        float v = !canMove ? 0f : moveInput.y;

        behaviourManager.GetAnim.SetFloat(hFloat, h, 0.1f, Time.deltaTime);
        behaviourManager.GetAnim.SetFloat(vFloat, v, 0.1f, Time.deltaTime);

        // --- SPRINT ---
        if (PlayerStats.instance != null && PlayerStats.instance.currentEndurance <= 0f)
            isSprinting = false;


        // --- ROOT MOTION ---
        HandleRootMotion();
    }

    public override void LocalFixedUpdate()
    {
        HandleMovement(moveInput.x, moveInput.y);
    }


    private void HandleRootMotion()
    {
        // Sécurisation pour éviter division par zéro
        float normalizedSpeed = runSpeed > 0f ? speed / runSpeed : 0f;
        bool bowCharging = BowBehaviour.instance != null && BowBehaviour.instance.chargeBow;

        bool useRootMotion =
            !behaviourManager.GetAnim.GetBool("Jump") &&
            (normalizedSpeed < 0.7f || aimBehaviour.IsAiming || bowCharging);

        behaviourManager.GetAnim.applyRootMotion = useRootMotion;
    }
    private void HandleMovement(float horizontal, float vertical)
    {
        if (!canMove || aimBehaviour.IsAiming)
        { 
            isSprinting = false;
            return;
        }

        // Calcul de la direction et magnitude de l'input
        Vector2 input = new Vector2(horizontal, vertical);
        float inputMagnitude = Mathf.Clamp01(input.magnitude);

        // Détermination de la vitesse cible
        float targetSpeed = inputMagnitude * speedSeeker;

        // 🟢 Gestion du sprint avec le nouveau Input System
        isSprinting = CanSprint();

        if (isSprinting && IsMoving())
        {
            targetSpeed = sprintSpeed;
            changedFOV = true;
            camScript.SetFOV(sprintFOV);
            PlayerStats.instance.UpdateEndurance(-PlayerStats.instance.coutDuSprint * Time.deltaTime);
        }
        else if (changedFOV)
        {
            camScript.ResetFOV();
            changedFOV = false;
            // On remet la vitesse par défaut (course ou marche)
            targetSpeed = inputMagnitude * runSpeed;
        }

        // Accélération / décélération
        speed = Mathf.MoveTowards(speed, targetSpeed, (targetSpeed > speed ? acceleration : deceleration) * Time.fixedDeltaTime);

        // Rotation
        Vector3 moveDir = GetMoveDirection(horizontal, vertical);

        // Déplacement physique
        if (behaviourManager.IsGrounded() && !jumpBehaviour.jump)
        {
            Vector3 velocity = moveDir * speed;
            velocity.y = rb.linearVelocity.y; // conserve la gravité
            rb.linearVelocity = velocity;
        }

        // Si pas d’input, stopper le joueur
        if (inputMagnitude < 0.1f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            speed = 0f;
        }

        if (behaviourManager.GetAnim == null)
        {
            Debug.LogWarning("Animator manquant sur le BasicBehaviour !");
            return;
        }

        // Animation
        float animSpeed = speed / sprintSpeed;
        if (animSpeed < 0.05f) animSpeed = 0f;
        behaviourManager.GetAnim.SetFloat("Speed", animSpeed, speedDampTime, Time.deltaTime);
    }

    private bool CanSprint()
    {
        return canMove
            && moveInput.magnitude > 0.1f
            && sprintInput
            && PlayerStats.instance.currentEndurance > 0f
            && !attackBehaviour.CanAttack(); // ou !attackBehaviour.isAttacking ?
    }

    // Vérifie si le joueur se déplace horizontalement.
    public bool IsHorizontalMoving() => moveInput.x != 0;

    // Vérifie si le joueur se déplace (horizontalement ou verticalement).
    public bool IsMoving() => (moveInput.x != 0) || (moveInput.y != 0);

    private Vector3 GetMoveDirection(float horizontal, float vertical)
    {
        if (aimBehaviour != null && aimBehaviour.IsAiming)
            return Vector3.zero;

        Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward.Normalize();

        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 moveDir = forward * vertical + right * horizontal;

        if (moveDir.sqrMagnitude > 0f)
        {
            moveDir.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, targetRotation, behaviourManager.turnSmoothing);
            rb.MoveRotation(smoothRotation);
            behaviourManager.SetLastDirection(moveDir);
        }

        return moveDir;
    }

    // Gestion friction pour collisions
    private void OnCollisionStay(Collision collision)
    {
        if (behaviourManager.IsCurrentBehaviour(this.GetBehaviourCode()) && collision.GetContact(0).normal.y <= 0.1f)
        {
            var col = GetComponent<CapsuleCollider>();
            col.material.dynamicFriction = 0f;
            col.material.staticFriction = 0f;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var col = GetComponent<CapsuleCollider>();
        col.material.dynamicFriction = 0.6f;
        col.material.staticFriction = 0.6f;
    }

    public void StopPlayer()
    {
        Debug.Log("Stopping player movement.");
        canMove = false;
        attackBehaviour.canAttack = false;

        speed = 0;
        rb.linearVelocity = Vector3.zero;

        behaviourManager.GetAnim.SetFloat("Speed", 0f);
    }


    public void StartPlayer()
    {
        Debug.Log("Starting player movement.");
        canMove = true;
        attackBehaviour.canAttack = true;
    }
}
