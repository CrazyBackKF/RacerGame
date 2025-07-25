using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.Windows;
using UnityEngine.UIElements;

public class AIController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float motorTorque;
    [SerializeField] private float brakeTorque;
    [SerializeField] private float maxTurnAngle;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float distanceToChangeWaypoint;
    [SerializeField] private float randomOffset;
    [SerializeField] private float checkForOtherCarsRadius;
    [SerializeField] private float avoidOtherCarsAngle;
    [SerializeField] private float maxBoxcastDistance;

    [Header("Max speeds")]
    [SerializeField] private float normalMaxSpeed;
    [SerializeField] private float turnMaxSpeed;
    [SerializeField] private float sharpTurnMaxSpeed;
    [SerializeField] private float speedChangeSpeed;
    private float currentMaxSpeed;

    [Header("Layer masks")]
    [SerializeField] private LayerMask carLayerMask;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<WheelCollider> wheelColliders;
    [SerializeField] private List<WheelCollider> frontWheelColliders;
    [SerializeField] private List<WheelCollider> rearWheelColliders;
    [SerializeField] private BoxCollider mainCollider;

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
        currentTarget = Vector3.Lerp(waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMinPoint(), waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMaxPoint(), Random.Range(0.25f, 0.75f));

        while (true)
        {
            if (Vector3.Distance(transform.position, currentTarget) < distanceToChangeWaypoint)
            {
                if (currentWaypoint < waypoints.Count - 1) currentWaypoint++;
                else currentWaypoint = 0;
                Vector3 pointMin = waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMinPoint();
                Vector3 pointMax = waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMaxPoint();
                currentTarget = Vector3.Lerp(pointMin, pointMax, Random.Range(0.25f, 0.75f));
                Debug.Log(currentTarget);
            }

            calculateAngle(out float angle);

            changeStatsBasedOnAngle(angle);

            float oldAngle = angle;

            Collider[] hits = Physics.OverlapSphere(transform.position, checkForOtherCarsRadius, carLayerMask);

            if (hits.Length > 1)
            {
                float sum = 0;
                int count = 0;

                foreach (Collider hit in hits)
                {
                    if (hit.gameObject == gameObject) continue;

                    Vector3 localPos = transform.InverseTransformPoint(hit.transform.position);
                    float weight = 1f / Mathf.Max(localPos.magnitude, 0.01f);

                    sum += localPos.x * weight;
                    count++;
                }

                if (count > 0)
                {
                    angle = -(sum / count) * avoidOtherCarsAngle;
                }
            }


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

            if (Vector3.Dot(transform.up, Vector3.down) > 0.5f || rb.linearVelocity.magnitude < 0.2f)
            {
                if (!isCarReseting)
                {
                    isCarReseting = true;
                    resetCoroutine = StartCoroutine(resetCarIEnumerator());
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
        if (Physics.BoxCast(mainCollider.center, Vector3.Scale(mainCollider.size * 0.5f, transform.lossyScale), transform.forward, out RaycastHit hit ,Quaternion.identity, maxBoxcastDistance, carLayerMask))
        {
            currentMaxSpeed = hit.rigidbody.linearVelocity.magnitude;
        }
        else if (Mathf.Abs(angle) > 40 && !isReversing)
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

    private IEnumerator resetCarIEnumerator()
    {
        yield return new WaitForSeconds(3);
        resetCar();
    }

    private void resetCar()
    {
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

        Gizmos.DrawWireSphere(currentTarget, 1);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, checkForOtherCarsRadius);

        Vector3 halfExtents = Vector3.Scale(mainCollider.size * 0.5f, transform.lossyScale);
        Vector3 startCenter = transform.position + transform.rotation * mainCollider.center;
        Vector3 direction = transform.forward;

        Gizmos.color = Color.green;
        DrawWireBox(startCenter, halfExtents, transform.rotation);

        Vector3 endCenter = startCenter + direction * maxBoxcastDistance;

        Gizmos.color = Color.red;
        DrawWireBox(endCenter, halfExtents, transform.rotation);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startCenter, endCenter);
    }

    void DrawWireBox(Vector3 center, Vector3 halfExtents, Quaternion rotation)
    {
        Matrix4x4 cubeTransform = Matrix4x4.TRS(center, rotation, halfExtents * 2);
        Gizmos.matrix = cubeTransform;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }

}
