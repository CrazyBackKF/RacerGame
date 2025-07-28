using System.Collections.Generic;
using UnityEngine;

public class CarAnimations : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float steeringWheelMaxAngle = 70;
    [SerializeField] private float steeringWheelRotationSpeed = 2;
    [SerializeField] private float turnSpeed = 5;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<Transform> wheelTransforms;
    [SerializeField] private List<WheelCollider> wheelColliders;
    [SerializeField] private Transform steeringWheelTransform;

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
}
