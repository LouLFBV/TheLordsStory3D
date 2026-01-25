using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpBehaviour : GenericBehaviour
{
    [Header("Paramètres de saut")]
    public float jumpHeight = 1.5f;             // Hauteur du saut
    public float jumpInertialForce = 10f;       // Force d’inertie horizontale
    public bool jump;                          // Indique si le joueur a déclenché un saut
    private bool isColliding;                   // Vérifie si le joueur touche un obstacle
    public bool canJump = true;              // Indique si le joueur peut sauter
    private int jumpBool;                       // Paramètre Animator "Jump"
    private int groundedBool;                   // Paramètre Animator "Grounded"

    [Header("Gravity tuning")]
    public float fallGravityMultiplier = 2.5f;

    public bool IsJumping => behaviourManager.GetAnim.GetBool(jumpBool) && !behaviourManager.IsGrounded();

    [SerializeField] private PlayerInput playerInput;
    private bool isJumpInput;



    [Header("References")]
    private AimBehaviourBasic aimBehaviour;
    private AttackBehaviour attackBehaviour;

    void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
        if (aimBehaviour == null)
            aimBehaviour = GetComponent<AimBehaviourBasic>();
        if (attackBehaviour == null)
            attackBehaviour = GetComponent<AttackBehaviour>();
    }

    void Start()
    {
        jumpBool = Animator.StringToHash("Jump");
        groundedBool = Animator.StringToHash("Grounded");
        behaviourManager.GetAnim.SetBool(groundedBool, true);
    }

    void OnEnable()
    {
        playerInput.actions["Jump"].performed += OnJumpPerformed;
        playerInput.actions["Jump"].canceled += OnJumpCanceled;
    }

    void OnDisable()
    {
        playerInput.actions["Jump"].performed -= OnJumpPerformed;
        playerInput.actions["Jump"].canceled -= OnJumpCanceled;
    }
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        isJumpInput = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumpInput = false;
    }


    void Update()
    {
        // Détection de la touche de saut
        if (!jump && isJumpInput && !aimBehaviour.IsAiming && canJump && !attackBehaviour.isAttacking)
        {
            jump = true;
            isJumpInput = false;
        }
    }

    public void FixedUpdate()
    {
        JumpManagement();
    }


    // --- GESTION DU SAUT PRINCIPAL ---
    void JumpManagement()
    {
        Rigidbody rb = behaviourManager.GetRigidBody;

        // --- Début du saut ---
        if (jump && behaviourManager.IsGrounded() && !behaviourManager.GetAnim.GetBool(jumpBool))
        {

            // Verrou temporaire du comportement (empêche d'autres inputs)
            behaviourManager.LockTempBehaviour(this.behaviourCode);

            // Animation
            behaviourManager.GetAnim.SetBool(jumpBool, true);
            behaviourManager.GetAnim.SetBool(groundedBool, false);

            // Supprime la vélocité verticale pour éviter les "super sauts"
            RemoveVerticalVelocity(rb);

            // Calcule la vitesse verticale du saut
            float velocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpHeight);
            rb.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);

            // Désactive temporairement la friction pour traverser les obstacles
            CapsuleCollider col = GetComponent<CapsuleCollider>();
            if (col)
            {
                col.material.dynamicFriction = 0f;
                col.material.staticFriction = 0f;
            }

            jump = false; // Réinitialise l’état
        }
        // --- En plein saut ---
        else if (behaviourManager.GetAnim.GetBool(jumpBool) )
        {
            // --- Gravité renforcée à la descente ---
            if (!behaviourManager.IsGrounded() && rb.linearVelocity.y < 0)
            {
                rb.AddForce(
                    Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1f),
                    ForceMode.Acceleration
                );
            }

            // Si toujours en l'air, applique une force inertielle horizontale
            if (!behaviourManager.IsGrounded() && !isColliding && behaviourManager.GetTempLockStatus())
            {
                rb.AddForce(transform.forward * jumpInertialForce , ForceMode.Acceleration);
            }

            // --- Atterrissage ---
            if (rb.linearVelocity.y < 0 && behaviourManager.IsGrounded())
            {

                behaviourManager.GetAnim.SetBool(groundedBool, true);
                behaviourManager.GetAnim.SetBool(jumpBool, false);
                behaviourManager.UnlockTempBehaviour(this.behaviourCode);

                // Restaure la friction par défaut
                CapsuleCollider col = GetComponent<CapsuleCollider>();
                if (col)
                {
                    col.material.dynamicFriction = 0.6f;
                    col.material.staticFriction = 0.6f;
                }
            }
            jump = false;
        }
        else if (jump)
        {
            Debug.Log("[JumpBehaviour] Input reçu mais conditions de saut non remplies " +
                      $"\n- IsGrounded: {behaviourManager.IsGrounded()}" +
                      $"\n- Anim JumpBool: {behaviourManager.GetAnim.GetBool(jumpBool)}");
        }
    }

    // --- Supprime la vélocité verticale ---
    private void RemoveVerticalVelocity(Rigidbody rb)
    {
        Vector3 vel = rb.linearVelocity;
        vel.y = 0;
        rb.linearVelocity = vel;
        Debug.Log("[JumpBehaviour] Vélocité verticale remise à zéro");
    }

    // --- Détection de collisions ---
    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }
}
