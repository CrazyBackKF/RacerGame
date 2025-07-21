using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public static Inputs Instance { get; private set; }

    [SerializeField] private PlayerInput playerInput;

    private InputActionMap mainGameplay;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainGameplay = playerInput.actions.FindActionMap("MainGameplay", true);
    }

    public void changeCurrentInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
    }

    public InputAction accelerate() => mainGameplay.FindAction("Accelerate");
    public InputAction deaccelerate() => mainGameplay.FindAction("Deaccelerate");
    public InputAction turn() => mainGameplay.FindAction("Turn");
    public InputAction drift() => mainGameplay.FindAction("Drift");

    public bool isAccelerating() => isPressed("Accelerate", mainGameplay);
    public bool isDeaccelerating() => isPressed("Deaccelerate", mainGameplay);
    public bool isTurning() => isPressed("Turn", mainGameplay);
    public bool isDrifting() => isPressed("Drift", mainGameplay);


    private bool isPressed(string name, InputActionMap map)
    {
        return (playerInput.currentActionMap == map && Mathf.Abs(map.FindAction(name).ReadValue<float>()) > 0.5f);
    }

}
