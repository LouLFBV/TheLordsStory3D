using UnityEngine;

public class EquipmentForgeronUI : MonoBehaviour
{
    [SerializeField] private ForgeronUI forgeronUI;
    public EquipmentType equipmentType;

    public void OnClick()
    {
        Debug.Log("Bouton cliqué pour le type d'équipement : " + equipmentType);

        forgeronUI.UpdateForgeronUI(equipmentType);
    }
}
