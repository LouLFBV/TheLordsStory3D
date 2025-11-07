using UnityEngine;
using System.Collections.Generic;

// Cette classe gère quel comportement du joueur est actif ou prioritaire, 
// et appelle ses fonctions locales.
// Elle contient la configuration de base et les fonctions communes utilisées 
// par tous les comportements du joueur.
public class BasicBehaviour : MonoBehaviour
{
    public Transform playerCamera;                        // Référence à la caméra qui suit le joueur.
    public float turnSmoothing = 0.06f;                   // Vitesse de rotation du joueur pour s’aligner avec la caméra.
    public float sprintFOV = 100f;                        // Champ de vision de la caméra quand le joueur sprinte.
    public string sprintButton = "Sprint";                // Nom de l’entrée du sprint.

    private float h;                                      // Axe horizontal.
    private float v;                                      // Axe vertical.
    private int currentBehaviour;                         // Référence au comportement actuellement actif.
    private int defaultBehaviour;                         // Comportement par défaut du joueur quand aucun autre n’est actif.
    private int behaviourLocked;                          // Référence au comportement temporairement verrouillé (empêche la prise de contrôle).
    private Vector3 lastDirection;                        // Dernière direction dans laquelle le joueur se déplaçait.
    private Animator anim;                                // Référence au composant Animator.
    private ThirdPersonOrbitCamBasic camScript;           // Référence au script de caméra à la 3ᵉ personne.
    private bool sprint;                                  // Indique si le joueur est en mode sprint.
    private bool changedFOV;                              // Indique si le champ de vision a été modifié à cause du sprint.
    private int hFloat;                                   // Paramètre Animator lié à l’axe horizontal.
    private int vFloat;                                   // Paramètre Animator lié à l’axe vertical.
    public List<GenericBehaviour> behaviours;            // Liste de tous les comportements du joueur.
    public List<GenericBehaviour> overridingBehaviours;  // Liste des comportements actuellement prioritaires (override).
    private Rigidbody rBody;                              // Référence au Rigidbody du joueur.
    private int groundedBool;                             // Paramètre Animator indiquant si le joueur est au sol.
    private Vector3 colExtents;                           // Dimensions du collider (utilisées pour le test de contact au sol).
    private bool movementLocked = false;                  // Indique si le mouvement du joueur est temporairement bloqué.

    // Accesseurs rapides :
    public float GetH => h;                               // Retourne l’axe horizontal.
    public float GetV => v;                               // Retourne l’axe vertical.
    public ThirdPersonOrbitCamBasic GetCamScript => camScript;   // Retourne le script de la caméra.
    public Rigidbody GetRigidBody => rBody;               // Retourne le Rigidbody du joueur.
    public Animator GetAnim => anim;                      // Retourne l’Animator du joueur.
    public int GetDefaultBehaviour => defaultBehaviour;   // Retourne le comportement par défaut.

    void Awake()
    {
        // Initialisation des références.
        behaviours = new List<GenericBehaviour>();
        overridingBehaviours = new List<GenericBehaviour>();
        anim = GetComponent<Animator>();
        hFloat = Animator.StringToHash("H");
        vFloat = Animator.StringToHash("V");
        camScript = playerCamera.GetComponent<ThirdPersonOrbitCamBasic>();
        rBody = GetComponent<Rigidbody>();

        // Variables pour la détection du sol.
        groundedBool = Animator.StringToHash("Grounded");
        colExtents = GetComponent<Collider>().bounds.extents;
    }

    void Update()
    {
        // Lecture des axes de déplacement.
        float rawH = Input.GetAxis("Horizontal");
        float rawV = Input.GetAxis("Vertical");

        // Si le mouvement est verrouillé, on force les axes à 0.
        h = movementLocked ? 0f : rawH;
        v = movementLocked ? 0f : rawV;

        // Envoi des axes au contrôleur d’animation.
        anim.SetFloat(hFloat, h, 0.1f, Time.deltaTime);
        anim.SetFloat(vFloat, v, 0.1f, Time.deltaTime);

        // Activation du sprint via l’entrée utilisateur.
        sprint = Input.GetButton(sprintButton);

        // Empêche de sprinter si le joueur n’a plus d’endurance.
        if (PlayerStats.instance != null && PlayerStats.instance.currentEndurance <= 0f)
            sprint = false;

        // Ajuste le champ de vision de la caméra selon le mode sprint.
        if (IsSprinting())
        {
            changedFOV = true;
            camScript.SetFOV(sprintFOV);
        }
        else if (changedFOV)
        {
            camScript.ResetFOV();
            changedFOV = false;
        }

        // Informe l’Animator si le joueur est au sol.
        anim.SetBool(groundedBool, IsGrounded());
    }

    // Appelée à chaque FixedUpdate : exécute la logique du comportement actif ou prioritaire.
    void FixedUpdate()
    {
        bool isAnyBehaviourActive = false;

        // Exécute les comportements overrides (ex : Aim)
        foreach (GenericBehaviour behaviour in overridingBehaviours)
            behaviour.LocalFixedUpdate();

        // ➕ Force certains comportements à toujours s’exécuter
        foreach (GenericBehaviour behaviour in behaviours)
        {
            if (behaviour.isActiveAndEnabled && (behaviour is JumpBehaviour || behaviour is MoveBehaviour))
            {
                behaviour.LocalFixedUpdate();
            }
        }

        // Si aucun comportement actif, maintient le joueur au sol.
        if (!isAnyBehaviourActive && overridingBehaviours.Count == 0)
        {
            rBody.useGravity = true;
            Repositioning();
        }
    }


    // Appelée à chaque LateUpdate : permet les ajustements visuels ou post-rotation caméra.
    private void LateUpdate()
    {
        if (behaviourLocked > 0 || overridingBehaviours.Count == 0)
        {
            foreach (GenericBehaviour behaviour in behaviours)
            {
                if (behaviour.isActiveAndEnabled && currentBehaviour == behaviour.GetBehaviourCode())
                    behaviour.LocalLateUpdate();
            }
        }
        else
        {
            foreach (GenericBehaviour behaviour in overridingBehaviours)
                behaviour.LocalLateUpdate();
        }
    }

    // Enregistre un nouveau comportement dans la liste.
    public void SubscribeBehaviour(GenericBehaviour behaviour)
    {
        behaviours.Add(behaviour);
    }

    // Définit le comportement par défaut du joueur.
    public void RegisterDefaultBehaviour(int behaviourCode)
    {
        defaultBehaviour = behaviourCode;
        currentBehaviour = behaviourCode;
    }

    // Active un comportement personnalisé.
    public void RegisterBehaviour(int behaviourCode)
    {
        if (currentBehaviour == defaultBehaviour)
            currentBehaviour = behaviourCode;
    }

    // Désactive un comportement personnalisé pour revenir au comportement par défaut.
    public void UnregisterBehaviour(int behaviourCode)
    {
        if (currentBehaviour == behaviourCode)
            currentBehaviour = defaultBehaviour;
    }

    // Active un comportement prioritaire qui prend le dessus sur les autres (ex : visée).
    public bool OverrideWithBehaviour(GenericBehaviour behaviour)
    {
        if (!overridingBehaviours.Contains(behaviour))
        {
            // Si aucun comportement n’est encore prioritaire.
            if (overridingBehaviours.Count == 0)
            {
                // Informe le comportement actuel qu’il est sur le point d’être supplanté.
                foreach (GenericBehaviour overriddenBehaviour in behaviours)
                {
                    if (overriddenBehaviour.isActiveAndEnabled && currentBehaviour == overriddenBehaviour.GetBehaviourCode())
                    {
                        overriddenBehaviour.OnOverride();
                        break;
                    }
                }
            }

            overridingBehaviours.Add(behaviour);
            return true;
        }
        return false;
    }

    // Retire un comportement prioritaire pour redonner le contrôle au comportement précédent.
    public bool RevokeOverridingBehaviour(GenericBehaviour behaviour)
    {
        if (overridingBehaviours.Contains(behaviour))
        {
            overridingBehaviours.Remove(behaviour);
            return true;
        }
        return false;
    }

    // Vérifie si un comportement (ou n’importe lequel) est actuellement prioritaire.
    public bool IsOverriding(GenericBehaviour behaviour = null)
    {
        if (behaviour == null)
            return overridingBehaviours.Count > 0;
        return overridingBehaviours.Contains(behaviour);
    }

    // Vérifie si le comportement actif correspond à celui passé en paramètre.
    public bool IsCurrentBehaviour(int behaviourCode)
    {
        return this.currentBehaviour == behaviourCode;
    }

    // Vérifie si un comportement est temporairement verrouillé (transition).
    public bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0)
    {
        return (behaviourLocked != 0 && behaviourLocked != behaviourCodeIgnoreSelf);
    }

    // Verrouille temporairement un comportement (ex : saut, visée, roulade...).
    public void LockTempBehaviour(int behaviourCode)
    {
        if (behaviourLocked == 0)
            behaviourLocked = behaviourCode;
    }

    // Déverrouille le comportement temporairement bloqué.
    public void UnlockTempBehaviour(int behaviourCode)
    {
        if (behaviourLocked == behaviourCode)
            behaviourLocked = 0;
    }

    // Fonctions utilitaires communes à tous les comportements :

    // Vérifie si le joueur est en train de sprinter.
    public virtual bool IsSprinting()
    {
        return sprint && IsMoving() && CanSprint();
    }

    // Vérifie si le joueur peut sprinter (tous les comportements doivent le permettre).
    public bool CanSprint()
    {
        if (PlayerStats.instance.currentEndurance <= 0 || Palette.instance.IfPlayerHasWeaponEquipped())
            return false;

        foreach (GenericBehaviour behaviour in behaviours)
            if (!behaviour.AllowSprint()) return false;

        foreach (GenericBehaviour behaviour in overridingBehaviours)
            if (!behaviour.AllowSprint()) return false;

        return true;
    }

    // Vérifie si le joueur se déplace horizontalement.
    public bool IsHorizontalMoving() => h != 0;

    // Vérifie si le joueur se déplace (horizontalement ou verticalement).
    public bool IsMoving() => (h != 0) || (v != 0);

    // Retourne la dernière direction de déplacement du joueur.
    public Vector3 GetLastDirection() => lastDirection;

    // Met à jour la dernière direction du joueur.
    public void SetLastDirection(Vector3 direction) => lastDirection = direction;

    // Replace le joueur en position debout, orienté vers sa dernière direction.
    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(rBody.rotation, targetRotation, turnSmoothing);
            rBody.MoveRotation(newRotation);
        }
    }

    // Vérifie si le joueur est au sol via un SphereCast.
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



    // Verrouille ou déverrouille le mouvement du joueur.
    public void SetMovementLock(bool locked)
    {
        movementLocked = locked;

        // Stoppe immédiatement le mouvement horizontal.
        if (locked)
        {
            var rb = GetRigidBody;
            var v = rb.linearVelocity;
            v.x = 0f; v.z = 0f;
            rb.linearVelocity = v;

            // Fige les paramètres d’animation.
            anim.SetFloat("H", 0f);
            anim.SetFloat("V", 0f);
        }
    }
}

// Classe de base pour tous les comportements du joueur.
// Toute logique spécifique (ex: visée, attaque, esquive...) doit hériter de celle-ci.
// Fournit des références communes et des fonctions virtuelles à redéfinir.
public abstract class GenericBehaviour : MonoBehaviour
{
    public int speedFloat;                      // Paramètre "Speed" dans l’Animator.
    public BasicBehaviour behaviourManager;     // Référence au gestionnaire principal des comportements.
    public int behaviourCode;                // Identifiant unique du comportement.
    protected bool canSprint;                   // Indique si le comportement autorise le sprint.

    void Awake()
    {
        behaviourManager = GetComponent<BasicBehaviour>();
        speedFloat = Animator.StringToHash("Speed");
        canSprint = true;

        // Détermine un identifiant unique pour chaque type de comportement.
        behaviourCode = this.GetType().GetHashCode();
    }

    // Méthodes virtuelles appelées localement (à redéfinir dans les classes filles).
    public virtual void LocalFixedUpdate() { }
    public virtual void LocalLateUpdate() { }
    public virtual void OnOverride() { }

    // Retourne l’identifiant du comportement.
    public int GetBehaviourCode() => behaviourCode;

    // Vérifie si le sprint est autorisé pour ce comportement.
    public bool AllowSprint() => canSprint;
}
