using UnityEngine;
using System.Collections;
using System.Linq;

public class Door : InteractableBase
{
    [SerializeField] private int niveauDeVerrouillage = 0; // Niveau de verrouillage de la porte
    [SerializeField] private bool IsUnlockable = true;

    private Palette palette; 
    private EquipmentLibrary equipmentLibrary; // Référence à l'item d'équipement si nécessaire

    [Header("Sounds")]

    [SerializeField] private AudioSource openDoorSound;
    [SerializeField] private AudioSource lockedDoorSound;
    [SerializeField] private AudioSource unlockedDoorSound;
    [SerializeField] private AudioSource unlockFailSound;


    [SerializeField]
    private GameObject door; // L'objet à faire pivoter

    [SerializeField]
    private float rotationSpeed = 2f;


    [SerializeField] private Vector3 openRotationEuler = new Vector3(0, 0, 90);

    private Quaternion closedRotation;
    private Quaternion openRotation;
    [HideInInspector] public bool isOpen = false;
    [HideInInspector] public bool isAnimating = false;
    public bool isLocked = false;

    [SerializeField] private ItemData keyItem; // L'item de clé requis pour ouvrir la porte

    private Animator animator;

    private void Start()
    {
        equipmentLibrary = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EquipmentLibrary>();
        palette = Palette.instance;

        if (door != null)
        {
            closedRotation = door.transform.rotation;
            openRotation = closedRotation * Quaternion.Euler(openRotationEuler);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                animator = player.GetComponent<Animator>();
            }
        }
        if (isLocked)
        {
            if (IsUnlockable)
                objectType = InteractableObjectType.Key;
            else
                objectType = InteractableObjectType.Locked;
        }
        else
            objectType = InteractableObjectType.Door;
        interactUI.SetInteractable(this);
    }
    public void OpenAndCloseDoor()
    {
        if (isLocked)
        {
            lockedDoorSound.PlayOneShot(lockedDoorSound.clip);
            return;
        }
        if (!isOpen && !isAnimating)
        {
            StartCoroutine(OpenDoor());
        }
        else if (isOpen && !isAnimating)
        {
            StartCoroutine(CloseDoor());
        }
    }


    private IEnumerator OpenDoor()
    {
        isOpen = true;
        isAnimating = true;
        openDoorSound.PlayOneShot(openDoorSound.clip);
        while (Quaternion.Angle(door.transform.rotation, openRotation) > 0.1f)
        {
            door.transform.rotation = Quaternion.Slerp(
                door.transform.rotation,
                openRotation,
                Time.deltaTime * rotationSpeed
            );
            yield return null;
        }
        door.transform.rotation = openRotation;
        isAnimating = false;
    }

    private IEnumerator CloseDoor()
    {
        isOpen = false;
        isAnimating = true;
        openDoorSound.PlayOneShot(openDoorSound.clip);
        while (Quaternion.Angle(door.transform.rotation, closedRotation) > 0.1f)
        {
            door.transform.rotation = Quaternion.Slerp(
                door.transform.rotation,
                closedRotation,
                Time.deltaTime * rotationSpeed
            );
            yield return null;
        }
        door.transform.rotation = closedRotation;
        isAnimating = false;
    }

    public void TryToOpenWithKey(ItemData key)
    {
        if (!IsUnlockable)
        {
            lockedDoorSound.PlayOneShot(lockedDoorSound.clip);
            return;
        }
        else if (!isLocked)
        {
            OpenAndCloseDoor();
            return;
        }
        // Cas 1 : la bonne clé est utilisée
        else if (key == keyItem)
        {
            ConsommerCle(key);
            DeverrouillerEtOuvrir();
        }
        // Cas 2 : tentative de crochetage avec une clé "improvisée"
        else if (key.attackPoints > 0)
        {
            float chanceDeReussite = Mathf.Clamp01((float)key.attackPoints / (niveauDeVerrouillage + 1));
            float tirage = Random.value;

            Debug.Log($"Chance de réussite : {chanceDeReussite}, tirage : {tirage}");

            if (tirage <= chanceDeReussite)
            {
                DeverrouillerEtOuvrir();
            }
            else
            {
                unlockFailSound.PlayOneShot(unlockFailSound.clip);
                if (PlayerStats.instance.reputationData.reputationPoints > -10)
                    PlayerStats.instance.reputationData.reputationPoints -= 1;
            }
            ConsommerCle(key);
        }
        else
        {
            lockedDoorSound.PlayOneShot(lockedDoorSound.clip);
        }
    }
    private void ConsommerCle(ItemData key)
    {
        if (palette.isEquippedObject1 && palette.equipmentObject1Item == key)
        {
            EquipmentLibraryItem equipmentLibraryItem1 = equipmentLibrary.content.FirstOrDefault(x => x.itemData == key);
            if (equipmentLibraryItem1 != null)
            {
                equipmentLibraryItem1.itemPrefab.SetActive(false);
            }
            palette.RemoveObject(1);
            palette.isEquippedObject1 = false;
        }
        else if (palette.isEquippedObject2 && palette.equipmentObject2Item == key)
        {
            EquipmentLibraryItem equipmentLibraryItem2 = equipmentLibrary.content.FirstOrDefault(x => x.itemData == key);
            if (equipmentLibraryItem2 != null)
            {
                equipmentLibraryItem2.itemPrefab.SetActive(false);
            }
            palette.RemoveObject(2);
            palette.isEquippedObject2 = false;
        }

        animator.SetBool("CarryingConsumable", false);
        palette.UpdateImageSeleted();
    }
    private void DeverrouillerEtOuvrir()
    {
        isLocked = false;
        unlockedDoorSound.PlayOneShot(unlockedDoorSound.clip);
        OpenAndCloseDoor();
        objectType = InteractableObjectType.Door;
        interactUI.SetInteractable(this);
    }

    public override void OnInteract(PlayerInteractor player)
    {
        if (palette.equipmentObject1Item != null && palette.equipmentObject1Item.itemType == ItemType.Key && palette.isEquippedObject1)
            TryToOpenWithKey(palette.equipmentObject1Item);
        else if (palette.equipmentObject2Item != null && palette.equipmentObject2Item.itemType == ItemType.Key && palette.isEquippedObject2)
            TryToOpenWithKey(palette.equipmentObject2Item);
        else
            OpenAndCloseDoor();
    }
}
