using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;


    [SerializeField]
    private ThirdPersonOrbitCamBasic playerCamera;

    [SerializeField] private AimBehaviourBasic aimBehaviourBasic;
    [SerializeField] private MoveBehaviour moveBehaviour;
    [SerializeField] private JumpBehaviour jumpBehaviour;

    private float defaultHorizontalAimingSpeed;
    private float defaultVerticalAimingSpeed;

    [HideInInspector]
    public bool atLeashOnePanelOpened;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        defaultHorizontalAimingSpeed = playerCamera.horizontalAimingSpeed;
        defaultVerticalAimingSpeed = playerCamera.verticalAimingSpeed;
    }


    public void HandlePanelOpened()
    {
        // Désactiver le contrôle du joueur
        aimBehaviourBasic.enabled = false;
        moveBehaviour.StopPlayer();
        jumpBehaviour.canJump = false;

        // Arrêter la caméra
        playerCamera.horizontalAimingSpeed = 0f;
        playerCamera.verticalAimingSpeed = 0f;

        // Afficher le curseur
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HandlePanelClosed()
    {
        // Réactiver le contrôle du joueur
        aimBehaviourBasic.enabled = true;
        moveBehaviour.StartPlayer();
        jumpBehaviour.canJump = true;

        // Rétablir la caméra
        playerCamera.horizontalAimingSpeed = defaultHorizontalAimingSpeed;
        playerCamera.verticalAimingSpeed = defaultVerticalAimingSpeed;

        // Masquer le curseur
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
