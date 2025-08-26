using System.Collections.Generic;
using UnityEngine;

public class CarAnimations : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float steeringWheelMaxAngle = 70;
    [SerializeField] private float steeringWheelRotationSpeed = 2;
    [SerializeField] private float turnSpeed = 5;

    private Rigidbody rb;
    private List<Transform> wheelTransforms;
    private List<WheelCollider> wheelColliders;
    private Transform steeringWheelTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        CarData carData = transform.GetChild(0).GetComponent<CarData>();
        setVariables(carData.getWheelTransforms(), carData.getWheelColliders(), carData.getSteeringWheelTransform());
    }

    private void Update()
    {
        float speed = rb.linearVelocity.magnitude * Time.deltaTime * Mathf.Rad2Deg;

        for (int i = 0; i < wheelTransforms.Count; i++)
        {
            Quaternion rotation = Quaternion.Euler(speed / wheelColliders[i].radius, 0, 0);
            wheelTransforms[i].localRotation *= rotation;
        }

        Quaternion steeringWheelRotation = Quaternion.Euler(0, steeringWheelMaxAngle * Inputs.Instance.turn().ReadValue<float>(), 0);
        steeringWheelTransform.localRotation = Quaternion.Slerp(steeringWheelTransform.localRotation, steeringWheelRotation, steeringWheelRotationSpeed * Time.deltaTime);

        foreach (WheelCollider collider in wheelColliders)
        {
            collider.transform.localRotation = Quaternion.Slerp(collider.transform.localRotation, Quaternion.Euler(0, 0, collider.steerAngle), turnSpeed * Time.deltaTime);
        }
    }

    private void setVariables(List<Transform> wheelTransforms, List<WheelCollider> wheelColliders, Transform steeringWheelTransform)
    {
        this.wheelTransforms = wheelTransforms;
        this.wheelColliders = wheelColliders;
        this.steeringWheelTransform = steeringWheelTransform;
    }
}
