using TMPro;
using UnityEngine;

public class Livre :InteractableBase
{
    [TextArea]
    [SerializeField] private string textePage1, textePage2;
    [SerializeField] private TextMeshProUGUI texteMPPage1, texteMPPage2;

    [SerializeField] private GameObject canvas;
    public bool isOpen = false;
    [SerializeField] private AudioSource openSound;

    private PlayerController _player;

    void Start()
    {
        if (texteMPPage1 != null)
            texteMPPage1.text = textePage1;
        if (texteMPPage2 != null)
            texteMPPage2.text = textePage2;
    }


    public void OuvrirFermerLivre()
    {
        if (_player == null)
            _player = PlayerController.Instance;
        if (!isOpen)
        {
            if (openSound != null)
            {
                openSound.Play();
            }
            canvas.SetActive(true);
            _player.StateMachine.ChangeState(PlayerStateType.UI);
            isOpen = true;
        }
        else
        {
            _player.StateMachine.ChangeState(PlayerStateType.Idle);
            canvas.SetActive(false);
            isOpen = false;
        }
    }

    public override void OnInteract(PlayerInteractor player)
    {
        OuvrirFermerLivre();
    }
}
