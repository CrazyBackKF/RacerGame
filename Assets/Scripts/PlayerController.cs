using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float motorTorque = 400f;
    [SerializeField] private float brakeTorque = 1500f;
    [SerializeField] private float turnAngle = 60f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float normalStiffness = 1f;
    [SerializeField] private float driftStiffness = 0.5f;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<WheelCollider> wheelColliders;
    [SerializeField] private List<WheelCollider> frontWheelColliders;
    [SerializeField] private List<Transform> frontWheelTransforms;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Vector3 localRigidbody = transform.InverseTransformDirection(rb.linearVelocity); 
        // jazda
        if (!Inputs.Instance.isAccelerating() && !Inputs.Instance.isDeaccelerating())
        {
            addMotorTorque(0f);
        }

        else if (Inputs.Instance.isAccelerating() && Inputs.Instance.isDeaccelerating())
        {
            if (Mathf.Abs(localRigidbody.z) > 0.1f)
            {
                addBrakeTorque(brakeTorque);
            }
            else
            {
                addBrakeTorque(0f);
            }
        }

        else if (Inputs.Instance.isAccelerating())
        {
            if (localRigidbody.z < -0.1f)
            {
                addBrakeTorque(brakeTorque);
            }
            else
            {
                addMotorTorque(motorTorque);
            }
        }

        else if (Inputs.Instance.isDeaccelerating())
        {
            if (localRigidbody.z > 0.1f)
            {
                addBrakeTorque(brakeTorque);
            }
            else
            {
                addMotorTorque(-motorTorque);
            }
        }

        //skrêcanie
        if (Inputs.Instance.isTurning())
        {
            float rotation = turnAngle * Mathf.Sign(Inputs.Instance.turn().action.ReadValue<float>());

            for (int i = 0; i < frontWheelColliders.Count; i++)
            {
                frontWheelTransforms[i].localRotation = Quaternion.Slerp(frontWheelTransforms[i].localRotation, Quaternion.Euler(0, 0, rotation), turnSpeed * Time.deltaTime);
                frontWheelColliders[i].steerAngle = Mathf.Lerp(frontWheelColliders[i].steerAngle, rotation, turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            for (int i = 0; i < frontWheelColliders.Count; i++)
            {
                frontWheelTransforms[i].localRotation = Quaternion.Slerp(frontWheelTransforms[i].localRotation, Quaternion.identity, turnSpeed * Time.deltaTime);
                frontWheelColliders[i].steerAngle = Mathf.Lerp(frontWheelColliders[i].steerAngle, 0, turnSpeed * Time.deltaTime);
            }
        }

        //drift
        if (Inputs.Instance.isDrifting())
        {
            foreach (WheelCollider wheel in  wheelColliders)
            {
                WheelFrictionCurve curve = wheel.forwardFriction;
                curve.stiffness = driftStiffness;
                wheel.forwardFriction = curve;
            }
        }
        else
        {
            foreach (WheelCollider wheel in wheelColliders)
            {
                WheelFrictionCurve curve = wheel.forwardFriction;
                curve.stiffness = normalStiffness;
                wheel.forwardFriction = curve;
            }
        }

    }

    private void addMotorTorque(float torque)
    {
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.brakeTorque = 0;
            wheelCollider.motorTorque = torque;
        }
    }

    private void addBrakeTorque(float torque)
    {
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.motorTorque = 0;
            wheelCollider.brakeTorque = torque;
        }
    }
}
