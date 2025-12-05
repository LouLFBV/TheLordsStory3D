using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider : MonoBehaviour
{
    public static InputProvider Instance{get; private set;}
    public PlayerInput UIInput { get; private set; }
    [SerializeField] private PlayerInput playerInput;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        UIInput = playerInput;
    }
}
