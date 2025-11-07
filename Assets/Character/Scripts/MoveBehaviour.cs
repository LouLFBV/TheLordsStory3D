using UnityEngine;

// MoveBehaviour hérite de GenericBehaviour. Ce script gère le déplacement du joueur sans Root Motion.
public class MoveBehaviour : GenericBehaviour
{
    [Header("Vitesses de déplacement")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 4.0f;
    public float sprintSpeed = 7.0f;

    public float acceleration = 10f;     // Accélération pour rendre le déplacement fluide.
    public float deceleration = 15f;     // Décélération quand le joueur relâche les touches.

    [Header("Réglages divers")]
    public float speedDampTime = 0.1f;   // Damping de l'animation.
    public bool canMove = true;          // Si le joueur peut se déplacer.

    [HideInInspector] public float speed;       // Vitesse réelle actuelle.
    [HideInInspector] public float speedSeeker; // Cible de vitesse.

    private AttackBehaviour attackBehaviour;
    private AimBehaviourBasic aimBehaviour;
    private JumpBehaviour jumpBehaviour;
    private Rigidbody rb;

    void Start()
    {
        behaviourManager.SubscribeBehaviour(this);
        behaviourManager.RegisterDefaultBehaviour(this.behaviourCode);

        rb = behaviourManager.GetRigidBody;
        speedSeeker = runSpeed;

        attackBehaviour = GetComponent<AttackBehaviour>();
        aimBehaviour = GetComponent<AimBehaviourBasic>();
        jumpBehaviour = GetComponent<JumpBehaviour>();
    }

    void Update()
    {
        HandleRootMotion();
    }

    public override void LocalFixedUpdate()
    {
        HandleMovement(behaviourManager.GetH, behaviourManager.GetV);
    }

    private void HandleRootMotion()
    {
        // Sécurisation pour éviter division par zéro
        float normalizedSpeed = runSpeed > 0f ? speed / runSpeed : 0f;

        bool useRootMotion = !behaviourManager.GetAnim.GetBool("Jump") && (normalizedSpeed < 0.7f || aimBehaviour.IsAiming || BowBehaviour.instance.chargeBow);
        behaviourManager.GetAnim.applyRootMotion = useRootMotion;
    }

    private void HandleMovement(float horizontal, float vertical)
    {
        if (!canMove || aimBehaviour.IsAiming) return;

        // Calcul de la direction et magnitude de l'input
        Vector2 input = new Vector2(horizontal, vertical);
        float inputMagnitude = Mathf.Clamp01(input.magnitude);

        // Détermination de la vitesse cible
        float targetSpeed = inputMagnitude * speedSeeker;

        // Sprint
        bool isSprinting = behaviourManager.IsSprinting() && PlayerStats.instance != null && PlayerStats.instance.currentEndurance > 0f;
        if (isSprinting)
        {
            targetSpeed = sprintSpeed;
            PlayerStats.instance.UpdateEndurance(-PlayerStats.instance.coutDuSprint * Time.deltaTime);
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

        // Animation
        float animSpeed = speed / sprintSpeed;
        if (animSpeed < 0.05f) animSpeed = 0f;
        behaviourManager.GetAnim.SetFloat(speedFloat, animSpeed, speedDampTime, Time.deltaTime);
    }

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
        canMove = false;
        attackBehaviour.canAttack = false;
        speed = 0;
        rb.linearVelocity = Vector3.zero;
        behaviourManager.GetAnim.SetFloat(speedFloat, 0f);
    }

    public void StartPlayer()
    {
        canMove = true;
        attackBehaviour.canAttack = true;
    }
}
