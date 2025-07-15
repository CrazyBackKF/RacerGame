using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public static Inputs Instance { get; private set; }

    [SerializeField] private InputActionReference accelerateInput;
    [SerializeField] private InputActionReference deaccelerateInput;
    [SerializeField] private InputActionReference turnInput;
    [SerializeField] private InputActionReference driftInput;

    private void Awake()
    {
        Instance = this;
    }

    public InputActionReference accelerate() {  return accelerateInput; }
    public InputActionReference deaccelerate() {  return deaccelerateInput; }
    public InputActionReference turn() {  return turnInput; }
    public InputActionReference drift() {  return driftInput; }

    public bool isAccelerating() { return accelerateInput.action.ReadValue<float>() > 0.5f; }
    public bool isDeaccelerating() { return deaccelerateInput.action.ReadValue<float>() > 0.5f; }
    public bool isTurning() { return Mathf.Abs(turnInput.action.ReadValue<float>()) > 0.5f; }
    public bool isDrifting() { return driftInput.action.ReadValue<float>() > 0.5f; }
}
