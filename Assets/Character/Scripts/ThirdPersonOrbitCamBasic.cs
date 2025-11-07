using UnityEngine;

// Cette classe correspond aux fonctionnalités de la caméra à la troisième personne.
public class ThirdPersonOrbitCamBasic : MonoBehaviour
{
    public Transform player;                                           // Référence du joueur.
    public Vector3 pivotOffset = new Vector3(0.0f, 1.7f, 0.0f);        // Décalage pour recentrer la caméra.
    public Vector3 camOffset = new Vector3(0.0f, 0.0f, -3.0f);       // Décalage pour repositionner la caméra par rapport au joueur.
    public float smooth = 10f;                                         // Vitesse de réactivité de la caméra.
    public float horizontalAimingSpeed = 6f;                           // Vitesse de rotation horizontale.
    public float verticalAimingSpeed = 6f;                             // Vitesse de rotation verticale.
    public float maxVerticalAngle = 30f;                               // Angle vertical maximal autorisé.
    public float minVerticalAngle = -60f;                              // Angle vertical minimal autorisé.
    public string XAxis = "Analog X";                                  // Nom par défaut de l’axe horizontal d’entrée.
    public string YAxis = "Analog Y";                                  // Nom par défaut de l’axe vertical d’entrée.

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
    private bool isCustomOffset;                                       // Indique si un décalage personnalisé est utilisé.

    // Retourne l’angle horizontal actuel.
    public float GetH { get { return angleH; } }

    void Awake()
    {
        // Référence au transform de la caméra.
        cam = transform;

        // Position initiale de la caméra.
        cam.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        cam.rotation = Quaternion.identity;

        // Initialisation des références et valeurs par défaut.
        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = cam.GetComponent<Camera>().fieldOfView;
        angleH = player.eulerAngles.y;

        ResetTargetOffsets();
        ResetFOV();
        ResetMaxVerticalAngle();

        // Vérifie qu’aucun décalage vertical ne soit appliqué directement sur la caméra.
        if (camOffset.y > 0)
            Debug.LogWarning("Le décalage vertical (Y) de la caméra sera ignoré pendant les collisions !\n" +
                             "Il est recommandé d’appliquer tout décalage vertical via le Pivot Offset.");
    }

    void Update()
    {
        // Récupère le mouvement de la souris pour faire tourner la caméra autour du joueur.
        // Souris :
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * horizontalAimingSpeed;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * verticalAimingSpeed;
        // Joystick :
        angleH += Mathf.Clamp(Input.GetAxis(XAxis), -1, 1) * 60 * horizontalAimingSpeed * Time.deltaTime;
        angleV += Mathf.Clamp(Input.GetAxis(YAxis), -1, 1) * 60 * verticalAimingSpeed * Time.deltaTime;

        // Limite le mouvement vertical.
        angleV = Mathf.Clamp(angleV, minVerticalAngle, targetMaxVerticalAngle);

        // Définit l’orientation de la caméra.
        Quaternion camYRotation = Quaternion.Euler(0, angleH, 0);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0);
        cam.rotation = aimRotation;

        // Met à jour le champ de vision.
        cam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(cam.GetComponent<Camera>().fieldOfView, targetFOV, Time.deltaTime);

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
        bool customOffsetCollision = isCustomOffset && noCollisionOffset.sqrMagnitude < targetCamOffset.sqrMagnitude;

        // Repositionne la caméra.
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, customOffsetCollision ? pivotOffset : targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, customOffsetCollision ? Vector3.zero : noCollisionOffset, smooth * Time.deltaTime);

        cam.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;
    }

    // Définit les décalages de caméra à des valeurs personnalisées.
    public void SetTargetOffsets(Vector3 newPivotOffset, Vector3 newCamOffset)
    {
        targetPivotOffset = newPivotOffset;
        targetCamOffset = newCamOffset;
        isCustomOffset = true;
    }

    // Réinitialise les décalages de caméra à leurs valeurs par défaut.
    public void ResetTargetOffsets()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
        isCustomOffset = false;
    }

    // Réinitialise uniquement le décalage vertical de la caméra.
    public void ResetYCamOffset()
    {
        targetCamOffset.y = camOffset.y;
    }

    // Définit un nouveau décalage vertical.
    public void SetYCamOffset(float y)
    {
        targetCamOffset.y = y;
    }

    // Définit un nouveau décalage horizontal.
    public void SetXCamOffset(float x)
    {
        targetCamOffset.x = x;
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
        Vector3 target = player.position + pivotOffset;
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
        Vector3 origin = player.position + pivotOffset;
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
