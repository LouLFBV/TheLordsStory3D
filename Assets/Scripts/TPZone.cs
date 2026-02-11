using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class TPZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ZoneData ID; // ScriptableObject partagé
    [SerializeField] private int zoneIDDestination = 0;
    [SerializeField] private int zoneID = 0;
    [SerializeField] private string zoneNameDestination = "Default Zone";
    private Transform playerTransform;

    [Header("UI/Sounds")]
    [SerializeField] private AudioSource teleportSound;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Button yesButton, noButton;
    public bool isActive = true;

    private void Start()
    {
        if (ID.isTeleporting)
        {
            // On ignore le trigger pendant un court moment
            StartCoroutine(ResetTeleportFlag());
        }
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Si on arrive dans une zone cible
        if (zoneID == ID.zoneIDDestinationArrive && playerTransform != null)
        {
            playerTransform.position = transform.position;
            ID.isTeleporting = true;
        }

        // callback sur changement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;

        // UI setup
        if (canvas != null)
        {
            canvas.SetActive(false);
            yesButton.onClick.RemoveAllListeners(); // Clear previous listeners
            yesButton.onClick.AddListener(ConfirmTeleport);
            noButton.onClick.RemoveAllListeners(); // Clear previous listeners
            noButton.onClick.AddListener(CancelTeleport);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isActive && !ID.isTeleporting)
        {
            if (canvas != null)
                canvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && canvas != null)
        {
            canvas.SetActive(false); // On cache le popup si le joueur sort de la zone
        }
    }

    private void ConfirmTeleport()
    {
        if (teleportSound != null) teleportSound.Play();

        // On sauvegarde la prochaine zone dans le SO
        ID.zoneIDDestinationArrive = zoneIDDestination;


        ID.isTeleporting = true; // Bloque les TP suivants
        SceneManager.LoadScene(zoneNameDestination);
        canvas.SetActive(false); // On cache le popup
    }

    private void CancelTeleport()
    {
        if (canvas != null)
            canvas.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (zoneID == ID.zoneIDDestinationArrive && playerTransform != null)
        {
            playerTransform.position = transform.position;
            ID.isTeleporting = true;
        }

        if (canvas != null)
            canvas.SetActive(false);
    }

    private IEnumerator ResetTeleportFlag()
    {
        yield return new WaitForSeconds(2f);
        ID.isTeleporting = false;
    }
}
