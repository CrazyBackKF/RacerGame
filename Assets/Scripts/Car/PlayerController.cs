using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float motorTorque = 400f;
    [SerializeField] private float brakeTorque = 1500f;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float distanceToChangeWaypoint;
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

    [SerializeField] private GameObject currentRaceTrack;

    private List<Transform> waypoints;
    private int currentWaypoint; 
    private bool isCarReseting = false;
    private Coroutine resetCoroutine;

    private float distanceBetweenFrontWheels;
    private float wheelbase;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        distanceBetweenFrontWheels = Vector3.Distance(frontWheelColliders[0].transform.position, frontWheelColliders[1].transform.position);
        wheelbase = Vector3.Distance(frontWheelColliders[0].transform.position, rearWheelColliders[0].transform.position);
        setNewWaypoints();
    }

    private void setNewWaypoints()
    {
        currentWaypoint = 0;
        waypoints = new List<Transform>();
        Transform waypointsTransform = currentRaceTrack.transform.Find("Waypoints");

        foreach (Transform t in waypointsTransform)
        {
            waypoints.Add(t);
        }
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < distanceToChangeWaypoint)
        {
            if (currentWaypoint < waypoints.Count - 1) currentWaypoint++;
            else currentWaypoint = 0;
        }

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
            if (Inputs.Instance.isTurning())
            {
                float input = Inputs.Instance.turn().ReadValue<float>();
                float baseAngle = turnAngle * Mathf.Sign(input);

                if (Inputs.Instance.isDrifting())
                {
                    baseAngle *= driftMultiplier;
                }

                float turnRadius = wheelbase / Mathf.Tan(Mathf.Abs(baseAngle) * Mathf.Deg2Rad);
                float trackHalf = distanceBetweenFrontWheels / 2f;

                float innerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - trackHalf)) * Mathf.Sign(input);
                float outerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + trackHalf)) * Mathf.Sign(input);

                if (input > 0) // skrêt w prawo
                {
                    frontWheelColliders[0].steerAngle = Mathf.Lerp(frontWheelColliders[0].steerAngle, innerAngle, turnSpeed * Time.deltaTime); // lewe
                    frontWheelColliders[1].steerAngle = Mathf.Lerp(frontWheelColliders[1].steerAngle, outerAngle, turnSpeed * Time.deltaTime); // prawe
                }
                else // skrêt w lewo
                {
                    frontWheelColliders[1].steerAngle = Mathf.Lerp(frontWheelColliders[1].steerAngle, innerAngle, turnSpeed * Time.deltaTime); // prawe
                    frontWheelColliders[0].steerAngle = Mathf.Lerp(frontWheelColliders[0].steerAngle, outerAngle, turnSpeed * Time.deltaTime); // lewe
                }
            }

        }
        else
        {
            for (int i = 0; i < frontWheelColliders.Count; i++)
            {
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

        if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            if (!isCarReseting)
            {
                isCarReseting = true;
                resetCoroutine = StartCoroutine(resetCar());
            }
        }
        else if (isCarReseting)
        {
            isCarReseting = false;
            StopCoroutine(resetCoroutine);
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

    private IEnumerator resetCar()
    {
        yield return new WaitForSeconds(3);
        setMotorTorque(0f, wheelColliders);
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        transform.position = waypoints[Mathf.Clamp(currentWaypoint - 1, 0, waypoints.Count - 1)].position;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        isCarReseting = false;
    }
}
