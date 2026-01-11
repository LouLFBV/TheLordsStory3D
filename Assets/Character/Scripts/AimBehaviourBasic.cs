using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Security.Claims;
using System;

// AimBehaviour hérite de GenericBehaviour. Cette classe correspond au comportement de visée et de déplacement latéral (strafe).
public class AimBehaviourBasic : GenericBehaviour
{
    public GameObject crosshair;                                          // Texture du réticule de visée.
    public float aimTurnSmoothing = 0.15f;                                // Vitesse de rotation du joueur pour correspondre à l’orientation de la caméra lors de la visée.
    public Vector3 aimPivotOffset = new (0.5f, 1.2f, 0f);         // Décalage du pivot de la caméra lorsqu’on vise.
    public Vector3 aimCamOffset = new (0f, 0.4f, -0.7f);         // Décalage de la caméra lorsqu’on vise.

    private int aimBool;                                                  // Variable Animator liée à la visée.
    private bool aim;                                                     // Booléen indiquant si le joueur est en train de viser.
    public bool IsAiming => aim;

    private PlayerStats playerStats;                     // Référence au script PlayerStats.
    private MoveBehaviour moveBehaviour;         // Référence au script MoveBehaviour.
    [SerializeField] private ThirdPersonOrbitCamBasic camScript;
    public event Action<bool> OnAimStateChanged;

    #region PlayerInput
    [SerializeField] private PlayerInput playerInput;
    private bool aimInput;
    private bool shoulderInput;
    #endregion

    #region Méthodes PlayerInput 
    void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.actions["Aim"].Enable();
        playerInput.actions["Aim"].performed += OnAim;
        playerInput.actions["Aim"].canceled += OnAimCanceled;

        playerInput.actions["Shoulder"].Enable();
        playerInput.actions["Shoulder"].performed += OnShoulder;
        playerInput.actions["Shoulder"].canceled += OnShoulderCanceled;
    }


    private void OnDisable()
    {
        playerInput.actions["Aim"].Disable();
        playerInput.actions["Aim"].performed -= OnAim;
        playerInput.actions["Aim"].canceled -= OnAimCanceled;

        playerInput.actions["Shoulder"].Disable();
        playerInput.actions["Shoulder"].performed -= OnShoulder;
        playerInput.actions["Shoulder"].canceled -= OnShoulderCanceled;
    }

    private void OnShoulderCanceled(InputAction.CallbackContext context)
    {
        shoulderInput = false;
    }

    private void OnShoulder(InputAction.CallbackContext context)
    {
        shoulderInput = true;
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        aimInput = false;
        OnAimStateChanged?.Invoke(aimInput);
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        aimInput = true;
        OnAimStateChanged?.Invoke(aimInput);
    }
    #endregion

    // Start est toujours appelé après toutes les fonctions Awake.
    void Start()
    {
        // Configuration des références.
        aimBool = Animator.StringToHash("Aim");
        playerStats = GetComponent<PlayerStats>();
        moveBehaviour = GetComponent<MoveBehaviour>();
    }

    // Update est utilisé pour définir des comportements quel que soit le comportement actif.
    void Update()
    {
        // Activer la visée tant que le bouton est tenu
        if (aimInput && !aim && !playerStats.isDead && behaviourManager.IsGrounded())
        {
            StartCoroutine(ToggleAimOn());
        }
        // Désactiver la visée quand on relâche le bouton
        else if (!aimInput && aim)
        {
            StartCoroutine(ToggleAimOff());
        }

        // 🔸 Forcer le comportement "strafe" même sans viser, si une arme est équipée.
        if (Palette.instance.IfPlayerHasWeaponEquipped() && !aim && !behaviourManager.IsOverriding(this))
        {
            // On ne vise pas mais on veut activer la gestion de mouvement par cette classe.
            behaviourManager.OverrideWithBehaviour(this);
        }
        else if (!Palette.instance.IfPlayerHasWeaponEquipped() && !aim && behaviourManager.IsOverriding(this))
        {
            // Si on n’a plus d’arme et qu’on n’est pas en visée → on rend le contrôle au MoveBehaviour.
            behaviourManager.RevokeOverridingBehaviour(this);
        }

        // Pas de sprint pendant la visée.
        canSprint = !aim;

        // Change la position de la caméra (épaule gauche/droite).
        if (shoulderInput)
        {
            shoulderInput = false;

            if (aim)
            {
                aimCamOffset.x = -aimCamOffset.x;
                aimPivotOffset.x = -aimPivotOffset.x;
            }
        }


        // Définit le booléen de visée dans l’Animator Controller.
        behaviourManager.GetAnim.SetBool(aimBool, aim);

        UpdateCrosshairVisibility();
    }


    // Coroutine pour activer le mode visée avec un léger délai.
    private IEnumerator ToggleAimOn()
    {
        if (moveBehaviour.changedFOV)
        {
            camScript.ResetFOV();
            moveBehaviour.changedFOV = false;
        }
            yield return new WaitForSeconds(0.05f);
        // La visée n’est pas possible si un autre comportement verrouille temporairement le contrôle (ex : attaque, roulade).
        if (behaviourManager.GetTempLockStatus(this.behaviourCode))
            yield break;

        // Active le mode visée.
        else
        {
            aim = true;
            int signal = 1;
            aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
            aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourManager.GetAnim.SetFloat("Speed", 0);
            // Cet état remplace le comportement actif actuel.
            behaviourManager.OverrideWithBehaviour(this);
        }
    }

    // Coroutine pour désactiver le mode visée avec un léger délai.
    private IEnumerator ToggleAimOff()
    {
        aim = false;
        yield return new WaitForSeconds(0.3f);
        behaviourManager.GetCamScript.ResetTargetOffsets();
        behaviourManager.GetCamScript.ResetMaxVerticalAngle();
        yield return new WaitForSeconds(0.05f);
        behaviourManager.RevokeOverridingBehaviour(this);
    }

    // LocalFixedUpdate remplace la fonction virtuelle de la classe de base.
    public override void LocalFixedUpdate()
    {
        // Maintient les décalages caméra lorsqu’on vise.
        if (aim)
            behaviourManager.GetCamScript.SetTargetOffsets(aimPivotOffset, aimCamOffset);
    }

    // LocalLateUpdate : le gestionnaire est appelé ici pour appliquer la rotation du joueur après la rotation de la caméra, afin d’éviter les scintillements.
    public override void LocalLateUpdate()
    {
        if (aim /*|| Palette.instance.IfPlayerHasWeaponEquipped()*/)
        {
            AimManagement(); // Gère la rotation pendant la visée.
        }
    }

    // Gère les paramètres de visée lorsque le mode visée est actif.
    void AimManagement()
    {
        // Gère l’orientation du joueur pendant la visée.
        Rotating();
    }

    // Fait pivoter le joueur pour correspondre à l’orientation correcte selon la caméra.
    void Rotating()
    {
        Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
        // Le joueur se déplace sur le sol, la composante Y de la direction de la caméra n’est pas pertinente.
        forward.y = 0.0f;
        forward = forward.normalized;

        // Fait toujours tourner le joueur selon la rotation horizontale de la caméra en mode visée.
        Quaternion targetRotation = Quaternion.Euler(0, behaviourManager.GetCamScript.GetH, 0);

        float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

        // Fait pivoter le joueur pour qu’il fasse face à la caméra.
        behaviourManager.SetLastDirection(forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);
    }

    // Affiche ou masque le réticule de visée.
    private void UpdateCrosshairVisibility()
    {
        if (crosshair == null) return;

        float mag = behaviourManager.GetCamScript.GetCurrentPivotMagnitude(aimPivotOffset);

        // Affiche le réticule uniquement si on vise ET que la caméra est bien alignée.
        bool shouldShow = aim && mag < 0.05f;

        if (crosshair.activeSelf != shouldShow)
            crosshair.SetActive(shouldShow);
    }
}
