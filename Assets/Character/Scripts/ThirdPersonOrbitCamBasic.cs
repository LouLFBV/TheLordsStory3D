using System;
using System.Collections;
using UnityEngine;

// Cette classe correspond aux fonctionnalités de la caméra à la troisième personne.
public class ThirdPersonOrbitCamBasic : MonoBehaviour
{
    public static ThirdPersonOrbitCamBasic Instance;
    public Transform player;                                           // Référence du joueur.
    public float smooth = 10f;                                         // Vitesse de réactivité de la caméra.
    public float horizontalAimingSpeed = 6f;                           // Vitesse de rotation horizontale.
    public float verticalAimingSpeed = 6f;                             // Vitesse de rotation verticale.
    public float maxVerticalAngle = 30f;                               // Angle vertical maximal autorisé.
    public float minVerticalAngle = -60f;                              // Angle vertical minimal autorisé.

    private float angleH = 0;                                          // Angle horizontal de la caméra en fonction du mouvement de la souris.
    private float angleV = 0;                                          // Angle vertical de la caméra en fonction du mouvement de la souris.
    private Transform cam;                                             // Référence du transform de la caméra.
    private Vector3 smoothPivotOffset;                                 // Décalage actuel du pivot (interpolation).
    private Vector3 smoothCamOffset;                                   // Décalage actuel de la caméra (interpolation).
    private Vector3 targetPivotOffset;                                 // Décalage cible du pivot pour interpolation.
    private Vector3 targetCamOffset;                                   // Décalage cible de la caméra pour interpolation.
    private float defaultFOV;                                          // Champ de vision (Field of View) par défaut.
    private float targetFOV;                                           // Champ de vision cible.
    private float targetMaxVerticalAngle;                              // Angle vertical maximum personnalisé.
    private Camera camComponent;
    private Coroutine shakeRoutine;
    private Vector3 shakeOffset;


    [Header("Camera Lock")]
    [SerializeField] private bool isLocked = false;
    public bool IsLocked => isLocked;

    // Retourne l’angle horizontal actuel.
    public float GetH => angleH;

    [SerializeField] private MoveBehaviour playerMoveBehaviour;
    [SerializeField] private AimBehaviourBasic playerAimBehaviour;
    [SerializeField] private BowBehaviour bowBehaviour;

    [SerializeField] private float fovLerpSpeed = 8f;


    [Header("Base Offsets")]
    [SerializeField] private Vector3 basePivotOffset = new Vector3(0f, 1.7f, 0f);
    [SerializeField] private Vector3 baseCamOffset = new Vector3(0f, 0f, -3f);

    [Header("Camera Collision")]
    [SerializeField] private float minCameraDistance = 0.6f;
    [SerializeField] private float collisionLerpSpeed = 12f;

    [Header("Aim Offsets")]
    [SerializeField] private Vector3 aimCamOffset = new Vector3(0.5f, 0f, -1.2f);
    private Vector3 currentAimOffset;

    [Header("Crouch Offsets")]
    [SerializeField] private float crouchYOffset = -0.3f;
    private float currentCrouchYOffset;

    [Header("Bow Charge Zoom")]
    [SerializeField] private float bowZoomDistance = 0.6f;
    [SerializeField] private AnimationCurve bowZoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float bowChargeFOV = 55f;
    private float bowCharge01;
    private bool isBowCharging;


    #region PlayerControls

    [Header("Sensibilités")]
    public float mouseSensitivity = 1f;
    public float gamepadSensitivity = 3f;   // généralement plus élevée

    private PlayerControls controls; 
    private Vector2 mouseLook;
    private Vector2 gamepadLook;

    #endregion


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        isLocked = false;
        controls = new PlayerControls();
        // Souris
        controls.Player.LookMouse.performed += ctx => mouseLook = ctx.ReadValue<Vector2>();
        controls.Player.LookMouse.canceled += _ => mouseLook = Vector2.zero;

        // Gamepad
        controls.Player.LookGamepad.performed += ctx => gamepadLook = ctx.ReadValue<Vector2>();
        controls.Player.LookGamepad.canceled += _ => gamepadLook = Vector2.zero;


        // Référence au transform de la caméra.
        cam = transform;
        camComponent = cam.GetComponent<Camera>();


        // Position initiale de la caméra.
        cam.position = player.position + Quaternion.identity * basePivotOffset + Quaternion.identity * baseCamOffset;
        cam.rotation = Quaternion.identity;


        smoothPivotOffset = basePivotOffset;
        smoothCamOffset = baseCamOffset;

        targetPivotOffset = basePivotOffset;
        targetCamOffset = baseCamOffset;


        defaultFOV = camComponent.fieldOfView;
        angleH = player.eulerAngles.y;

        ResetFOV();
        ResetMaxVerticalAngle();

        playerMoveBehaviour = player.GetComponent<MoveBehaviour>();
        bowBehaviour = player.GetComponent<BowBehaviour>();
        playerAimBehaviour = player.GetComponent<AimBehaviourBasic>();

        // Vérifie qu’aucun décalage vertical ne soit appliqué directement sur la caméra.
        if (baseCamOffset.y > 0)
            Debug.LogWarning("Le décalage vertical (Y) de la caméra sera ignoré pendant les collisions !\n" +
                             "Il est recommandé d’appliquer tout décalage vertical via le Pivot Offset.");
    }

    void Update()
    {
        if (isLocked) return;
        // Récupère le mouvement de la souris pour faire tourner la caméra autour du joueur.
        Vector2 mouse = mouseLook * mouseSensitivity;
        Vector2 pad = gamepadSensitivity * Time.deltaTime * gamepadLook;

        Vector2 finalLook = mouse + pad;

        angleH += finalLook.x * horizontalAimingSpeed;
        angleV += finalLook.y * verticalAimingSpeed;



        // Limite le mouvement vertical.
        angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);

        // Définit l’orientation de la caméra.
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
        cam.rotation = aimRotation;

        // Met à jour le champ de vision.
        camComponent.fieldOfView = Mathf.Lerp(camComponent.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);


        targetPivotOffset = basePivotOffset;



        // Offsets composés dynamiquement (base + aim + crouch)
        targetCamOffset =
            baseCamOffset +
            currentAimOffset +
            Vector3.up * currentCrouchYOffset;



        // Ajoute le zoom de l’arc si nécessaire.
        float zoomT = bowZoomCurve.Evaluate(bowCharge01);
        Vector3 bowZoomOffset = Vector3.forward * (zoomT * bowZoomDistance);



        targetCamOffset += bowZoomOffset;

        // Vérifie les collisions entre la caméra et l’environnement.
        Vector3 baseTempPosition = player.position + camYRotation * targetPivotOffset;
        Vector3 noCollisionOffset = targetCamOffset;
        while (noCollisionOffset.magnitude >= 0.2f)
        {
            if (DoubleViewingPosCheck(baseTempPosition + aimRotation * noCollisionOffset))
                break;
            noCollisionOffset -= noCollisionOffset.normalized * 0.2f;
        }
        if (noCollisionOffset.magnitude < 0.2f)
            noCollisionOffset = Vector3.zero;



        // Si un décalage personnalisé est en collision, passe temporairement en “vue à la première personne”.
        float desiredDistance = noCollisionOffset.magnitude;



        // Clamp pour éviter la first-person trop rapide
        desiredDistance = Mathf.Max(desiredDistance, minCameraDistance);



        // Direction caméra → joueur conservée
        Vector3 desiredCamOffset =
            noCollisionOffset.normalized * desiredDistance;

        smoothPivotOffset = Vector3.Lerp(
            smoothPivotOffset,
            targetPivotOffset,
            smooth * Time.deltaTime
        );

        smoothCamOffset = Vector3.Lerp(
            smoothCamOffset,
            desiredCamOffset,
            collisionLerpSpeed * Time.deltaTime
        );


        cam.position =
        player.position +
        camYRotation * smoothPivotOffset +
        aimRotation * (smoothCamOffset + shakeOffset);

    }

    public void LockCamera()
    {
        Debug.Log("Camera Locked");
        isLocked = true;
    }

    public void UnlockCamera()
    {
        Debug.Log("Camera Unlocked");
        isLocked = false;
    }

    void OnEnable()
    {
        controls.Enable();

        if (playerMoveBehaviour != null)
            playerMoveBehaviour.OnCrouchChanged += HandleCrouch;

        if (playerAimBehaviour != null)
            playerAimBehaviour.OnAimStateChanged += OnAimChanged;

        if (bowBehaviour != null)
        {
            bowBehaviour.OnBowChargeProgress += OnBowChargeProgress;
            bowBehaviour.OnBowChargeStateChanged += OnBowChargeStateChanged;
        }


        CameraEvents.OnCameraShake += PlayCameraShake;
    }

    void OnDisable()
    {
        controls.Disable();

        if (playerMoveBehaviour != null)
            playerMoveBehaviour.OnCrouchChanged -= HandleCrouch;

        if (playerAimBehaviour != null)
            playerAimBehaviour.OnAimStateChanged -= OnAimChanged;

        if (bowBehaviour != null)
        {
            bowBehaviour.OnBowChargeProgress -= OnBowChargeProgress;
            bowBehaviour.OnBowChargeStateChanged -= OnBowChargeStateChanged;
        }


        CameraEvents.OnCameraShake -= PlayCameraShake;
    }

    private void OnBowChargeProgress(float value)
    {
        if (!isBowCharging) return;

        bowCharge01 = value;
        targetFOV = Mathf.Lerp(defaultFOV, bowChargeFOV, bowCharge01);
    }


    private void OnBowChargeStateChanged(bool charging)
    {
        isBowCharging = charging;

        if (!charging)
        {
            bowCharge01 = 0f;
            ResetFOV();
        }
    }



    private void OnAimChanged(bool aiming)
    {
        currentAimOffset = aiming ? aimCamOffset : Vector3.zero;
        ResetFOV();
    }



    private void HandleCrouch(bool crouched)
    {
        currentCrouchYOffset = crouched ? crouchYOffset : 0f;
    }


    private void PlayCameraShake(float intensity, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(CameraShake(intensity, duration));
    }

    private IEnumerator CameraShake(float intensity, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            shakeOffset = UnityEngine.Random.insideUnitSphere * intensity;
            shakeOffset = Vector3.ClampMagnitude(shakeOffset, intensity);


            yield return null;
        }
        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }

    // Définit un champ de vision personnalisé.
    public void SetFOV(float customFOV)
    {
        this.targetFOV = customFOV;
    }

    // Réinitialise le champ de vision à la valeur par défaut.
    public void ResetFOV()
    {
        this.targetFOV = defaultFOV;
    }

    // Définit l’angle vertical maximal de la caméra.
    public void SetMaxVerticalAngle(float angle)
    {
        this.targetMaxVerticalAngle = angle;
    }

    // Réinitialise l’angle vertical maximal à la valeur par défaut.
    public void ResetMaxVerticalAngle()
    {
        this.targetMaxVerticalAngle = maxVerticalAngle;
    }

    // Vérifie deux fois les collisions (certains objets concaves échappent à une seule détection).
    bool DoubleViewingPosCheck(Vector3 checkPos)
    {
        return ViewingPosCheck(checkPos) && ReverseViewingPosCheck(checkPos);
    }

    // Vérifie la collision de la caméra vers le joueur.
    bool ViewingPosCheck(Vector3 checkPos)
    {
        Vector3 target = player.position + basePivotOffset;
        Vector3 direction = target - checkPos;

        if (Physics.SphereCast(checkPos, 0.2f, direction, out RaycastHit hit, direction.magnitude))
        {
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false; // Collision détectée avec un obstacle.
            }
        }
        return true; // Pas de collision ou collision avec le joueur.
    }

    // Vérifie la collision du joueur vers la caméra.
    bool ReverseViewingPosCheck(Vector3 checkPos)
    {
        Vector3 origin = player.position + basePivotOffset;
        Vector3 direction = checkPos - origin;

        if (Physics.SphereCast(origin, 0.2f, direction, out RaycastHit hit, direction.magnitude))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    // Retourne la distance actuelle du pivot de la caméra.
    public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
    {
        return Mathf.Abs((finalPivotOffset - smoothPivotOffset).magnitude);
    }
}

public static class CameraEvents
{
    public static Action<float, float> OnCameraShake;
    // amplitude, duration
}
