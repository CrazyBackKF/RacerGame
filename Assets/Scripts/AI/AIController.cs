using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.Windows;

public class AIController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float motorTorque;
    [SerializeField] private float brakeTorque;
    [SerializeField] private float maxTurnAngle;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float distanceToChangeWaypoint;
    [SerializeField] private float randomOffset;

    [Header("Max speeds")]
    [SerializeField] private float normalMaxSpeed;
    [SerializeField] private float turnMaxSpeed;
    [SerializeField] private float sharpTurnMaxSpeed;
    [SerializeField] private float speedChangeSpeed;
    private float currentMaxSpeed;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<WheelCollider> wheelColliders;
    [SerializeField] private List<WheelCollider> frontWheelColliders;
    [SerializeField] private List<WheelCollider> rearWheelColliders;

    [SerializeField] private GameObject currentRaceTrack;

    private List<Transform> waypoints;
    private int currentWaypoint = 0;

    private float distanceBetweenFrontWheels;
    private float wheelbase;

    private bool isReversing = false;
    private bool isCarReseting = false; 
    private Coroutine resetCoroutine;

    Vector3 currentTarget;
    private void Start()
    {
        startNewRace();
    }

    private void startNewRace()
    {
        distanceBetweenFrontWheels = Vector3.Distance(frontWheelColliders[0].transform.position, frontWheelColliders[1].transform.position);
        wheelbase = Vector3.Distance(frontWheelColliders[0].transform.position, rearWheelColliders[0].transform.position);
        setNewWaypoints();
        StartCoroutine(race());
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


    private IEnumerator race()
    {
        currentTarget = new Vector3(waypoints[currentWaypoint].position.x + Random.Range(-randomOffset, randomOffset), waypoints[currentWaypoint].position.y, waypoints[currentWaypoint].position.z + Random.Range(-randomOffset, randomOffset));

        while (true)
        {
            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < distanceToChangeWaypoint)
            {
                if (currentWaypoint < waypoints.Count - 1) currentWaypoint++;
                else currentWaypoint = 0;
                currentTarget = new Vector3(waypoints[currentWaypoint].position.x + Random.Range(-randomOffset, randomOffset), waypoints[currentWaypoint].position.y, waypoints[currentWaypoint].position.z + Random.Range(-randomOffset, randomOffset));
            }

            calculateAngle(out float angle);

            changeStatsBasedOnAngle(angle);

            float oldAngle = angle;

            if (Mathf.Abs(angle) > maxTurnAngle && Mathf.Abs(angle) < 90)
            {
                angle = maxTurnAngle * Mathf.Sign(angle);
            }

            if (rb.linearVelocity.magnitude > currentMaxSpeed)
            {
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity.normalized * currentMaxSpeed, speedChangeSpeed * Time.deltaTime);
            }

            if (Mathf.Abs(oldAngle) > 90)
            {
                isReversing = true;
                foreach (WheelCollider collider in wheelColliders)
                {
                    WheelFrictionCurve curve = collider.sidewaysFriction;
                    collider.sidewaysFriction = setWheelFriction(curve, curve.extremumSlip, curve.extremumValue, curve.asymptoteSlip, curve.asymptoteValue, 1);
                }
                setMotorTorque(-motorTorque);
            }
            else
            {
                setMotorTorque(motorTorque);
            }
            ackermanGeometry(angle);

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

            yield return null;
        }
    }

    private void setMotorTorque(float torque)
    {
        foreach (WheelCollider collider in wheelColliders)
        {
            collider.brakeTorque = 0;
            collider.motorTorque = torque;
        }
    }

    private void setBrakeTorque(float torque)
    {
        foreach (WheelCollider collider in wheelColliders)
        {
            collider.motorTorque = 0;
            collider.brakeTorque = torque;
        }
    }

    private void calculateAngle(out float angle)
    {
        Vector3 localTarget = transform.InverseTransformPoint(currentTarget);
        angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
    }

    private void changeStatsBasedOnAngle(float angle)
    {
        if (isReversing)
        {
            Vector3 localTarget = transform.InverseTransformPoint(waypoints[currentWaypoint].position);

            if (localTarget.z > 0 && Mathf.Abs(angle) < 30f)
            {
                foreach (WheelCollider collider in wheelColliders)
                {
                    WheelFrictionCurve curve = collider.sidewaysFriction;
                    collider.sidewaysFriction = setWheelFriction(curve, curve.extremumSlip, curve.extremumValue, curve.asymptoteSlip, curve.asymptoteValue, 10);
                }

                isReversing = false;
            }
        }

        float oldAngle = (frontWheelColliders[0].steerAngle + frontWheelColliders[1].steerAngle) / 2;
        if (Mathf.Abs(angle - oldAngle) < 10)
        {
            angle = oldAngle;
        }

        if (Mathf.Abs(angle) > 40 && !isReversing)
        {
            currentMaxSpeed = sharpTurnMaxSpeed;
        }
        else if (Mathf.Abs(angle) > 30 && !isReversing)
        {
            currentMaxSpeed = turnMaxSpeed;
        }
        else
        {
            currentMaxSpeed = normalMaxSpeed;
        }
    }

    private void ackermanGeometry (float baseAngle)
    {
        float turnRadius = wheelbase / Mathf.Tan(Mathf.Abs(baseAngle) * Mathf.Deg2Rad);
        float trackHalf = distanceBetweenFrontWheels / 2f;

        float innerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius - trackHalf)) * Mathf.Sign(baseAngle);
        float outerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelbase / (turnRadius + trackHalf)) * Mathf.Sign(baseAngle);

        if (baseAngle > 0) // skrêt w prawo
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
        setMotorTorque(0f);
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        transform.position = waypoints[Mathf.Clamp(currentWaypoint - 1, 0, waypoints.Count - 1)].position;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        isCarReseting = false;
    }


    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(waypoints[currentWaypoint].position, 1);
    }

}
