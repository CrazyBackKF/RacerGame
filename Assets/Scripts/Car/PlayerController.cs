using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float motorTorque = 400f;
    [SerializeField] private float brakeTorque = 1500f;
    [SerializeField] private float maxSpeed;
    [Header("Turning stats")]
    [SerializeField] private float turnAngle = 60f;
    [SerializeField] private float turnSpeed = 5f;
    [Header("Drift stats")]
    [SerializeField] private float normalStiffness = 1f;
    [SerializeField] private float driftStiffness = 0.5f;
    [SerializeField] private float driftMultiplier = 1.5f;
    [SerializeField] private float driftBrakeTorque = 500f;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<WheelCollider> wheelColliders;
    [SerializeField] private List<WheelCollider> frontWheelColliders;
    [SerializeField] private List<WheelCollider> rearWheelColliders;
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
            setMotorTorque(0f, wheelColliders);
        }

        else if (Inputs.Instance.isAccelerating() && Inputs.Instance.isDeaccelerating())
        {
            if (Mathf.Abs(localRigidbody.z) > 0.1f)
            {
                setBrakeTorque(brakeTorque, wheelColliders);
            }
            else
            {
                setBrakeTorque(0f, wheelColliders);
            }
        }

        else if (Inputs.Instance.isAccelerating())
        {
            if (localRigidbody.z < -0.1f)
            {
                setBrakeTorque(brakeTorque, wheelColliders);
            }
            else
            {
                setMotorTorque(motorTorque, wheelColliders);
            }
        }

        else if (Inputs.Instance.isDeaccelerating())
        {
            if (localRigidbody.z > 0.1f)
            {
                setBrakeTorque(brakeTorque, wheelColliders);
            }
            else
            {
                setMotorTorque(-motorTorque, wheelColliders);
            }
        }

        //skrêcanie
        if (Inputs.Instance.isTurning())
        {
            float rotation = turnAngle * Mathf.Sign(Inputs.Instance.turn().action.ReadValue<float>());

            for (int i = 0; i < frontWheelColliders.Count; i++)
            {
                frontWheelTransforms[i].localRotation = Quaternion.Slerp(frontWheelTransforms[i].localRotation, Quaternion.Euler(0, 0, rotation), turnSpeed * Time.deltaTime);
                if (Inputs.Instance.isDrifting()) frontWheelColliders[i].steerAngle = Mathf.Lerp(frontWheelColliders[i].steerAngle, rotation * driftMultiplier, turnSpeed * Time.deltaTime);
                else frontWheelColliders[i].steerAngle = Mathf.Lerp(frontWheelColliders[i].steerAngle, rotation, turnSpeed * Time.deltaTime);
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
            foreach (WheelCollider wheel in wheelColliders)
            {
                wheel.sidewaysFriction = setWheelFriction(wheel.sidewaysFriction, 0.1f, 0.25f, 0.1f, 0.25f, driftStiffness);
            }

            setBrakeTorque(driftBrakeTorque, rearWheelColliders);
        }
        else
        {
            foreach (WheelCollider wheel in wheelColliders)
            {
                wheel.sidewaysFriction = setWheelFriction(wheel.sidewaysFriction, 0.2f, 1f, 0.5f, 0.75f, normalStiffness);
            }
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void setMotorTorque(float torque, List<WheelCollider> wheelColliders)
    {
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.brakeTorque = 0;
            wheelCollider.motorTorque = torque;
        }
    }

    private void setBrakeTorque(float torque, List<WheelCollider> wheelColliders)
    {
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.motorTorque = 0;
            wheelCollider.brakeTorque = torque;
        }
    }

    private WheelFrictionCurve setWheelFriction(WheelFrictionCurve curve, float extremumSlip, float extremumValue, float asymptoteSlip, float asymptoteValue, float stiffness)
    {
        curve.extremumSlip = extremumSlip;
        curve.extremumValue = extremumValue;
        curve.asymptoteSlip = asymptoteSlip;
        curve.asymptoteValue = asymptoteValue;
        curve.stiffness = stiffness;
        return curve;
    }
}
