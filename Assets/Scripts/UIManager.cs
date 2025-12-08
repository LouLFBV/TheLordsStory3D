using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private GameObject[] UIPanels;

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
            DontDestroyOnLoad(gameObject);
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

    public void AddPanel( GameObject panel)
    {
        if (UIPanels == null || UIPanels.Length == 0)
        {
            UIPanels = new GameObject[] { panel };
        }
        else
        {
            var tempList = UIPanels.ToList();
            tempList.Add(panel);
            UIPanels = tempList.ToArray();
        }
    }

    void Update()
    {
        atLeashOnePanelOpened = UIPanels.Any(panel => panel != null && panel.activeSelf);

        if (atLeashOnePanelOpened)
        {
            aimBehaviourBasic.enabled = false;
            playerCamera.horizontalAimingSpeed = 0f;
            playerCamera.verticalAimingSpeed = 0f;
            Cursor.visible = true; // affiche le curseur
            Cursor.lockState = CursorLockMode.None; // déverrouille le curseur

        }
        else
        {
            aimBehaviourBasic.enabled = true;
            playerCamera.horizontalAimingSpeed = defaultHorizontalAimingSpeed;
            playerCamera.verticalAimingSpeed = defaultVerticalAimingSpeed;
            Cursor.visible = false; // cache le curseur
            Cursor.lockState = CursorLockMode.Locked; // verrouille le curseur au centre de l'écran
        }
    }
}
