using UnityEngine;

public class JumpBehaviour : GenericBehaviour
{
    [Header("Paramètres de saut")]
    public string jumpButton = "Jump";          // Touche de saut par défaut
    public float jumpHeight = 1.5f;             // Hauteur du saut
    public float jumpInertialForce = 10f;       // Force d’inertie horizontale
    public float jumpCooldown = 0.2f;           // Petit délai avant de pouvoir resauter
    public bool jump;                          // Indique si le joueur a déclenché un saut
    private bool isColliding;                   // Vérifie si le joueur touche un obstacle

    private int jumpBool;                       // Paramètre Animator "Jump"
    private int groundedBool;                   // Paramètre Animator "Grounded"

    public bool IsJumping => behaviourManager.GetAnim.GetBool(jumpBool) && !behaviourManager.IsGrounded();


    [Header("References")]
    private AimBehaviourBasic aimBehaviour;

    void Start()
    {
        jumpBool = Animator.StringToHash("Jump");
        groundedBool = Animator.StringToHash("Grounded");
        behaviourManager.GetAnim.SetBool(groundedBool, true);
        aimBehaviour = GetComponent<AimBehaviourBasic>();
    }

    void Update()
    {
        // Détection de la touche de saut
        if (!jump && Input.GetButtonDown(jumpButton) && !aimBehaviour.IsAiming)
        {
            jump = true;

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

            // Si toujours en l'air, applique une force inertielle horizontale
            if (!behaviourManager.IsGrounded() && !isColliding && behaviourManager.GetTempLockStatus())
            {
                rb.AddForce(transform.forward * jumpInertialForce * Time.fixedDeltaTime , ForceMode.Acceleration);
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
