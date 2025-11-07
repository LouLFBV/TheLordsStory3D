using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SlotAmelioration : MonoBehaviour
{
    [Header("Left")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI prixText;
    public TextMeshProUGUI niveauActuel;
    public Image icone;
    public Button acceptButton;

    [Header("Right")]
    public TextMeshProUGUI statsActuels;
    public TextMeshProUGUI statsAmeliores;
    public TextMeshProUGUI niveauAmeliore;
    public Button resetButton;
}
