using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public static Inputs Instance { get; private set; }

    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        Instance = this;
    }

    public void changeCurrentInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
    }

    public InputAction accelerate() => playerInput.currentActionMap.FindAction("Accelerate");
    public InputAction deaccelerate() => playerInput.currentActionMap.FindAction("Deaccelerate");
    public InputAction turn() => playerInput.currentActionMap.FindAction("Turn");
    public InputAction drift() => playerInput.currentActionMap.FindAction("Drift");

    public bool isAccelerating() => isPressed("Accelerate");
    public bool isDeaccelerating() => isPressed("Deaccelerate");
    public bool isTurning() => isPressed("Turn");
    public bool isDrifting() => isPressed("Drift");


    private bool isPressed(string name)
    {
        var action = playerInput.currentActionMap.FindAction(name);
        return action != null && Mathf.Abs(action.ReadValue<float>()) > 0.5f;
    }

}
