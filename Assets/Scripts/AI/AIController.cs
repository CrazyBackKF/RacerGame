using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Runtime.CompilerServices;

public class AIController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float motorTorque;
    [SerializeField] private float brakeTorque;
    [SerializeField] private float maxTurnAngle;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float distanceToChangeWaypoint;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<WheelCollider> wheelColliders;
    [SerializeField] private List<WheelCollider> frontWheelColliders;
    [SerializeField] private List<WheelCollider> rearWheelColliders;

    [SerializeField] private GameObject currentRaceTrack;

    private List<Transform> waypoints;
    private int currentWaypoint = 0;

    private void Start()
    {
        startNewRace();
    }

    private void startNewRace()
    {
        currentWaypoint = 0;
        setNewWaypoints();
        StartCoroutine(race());
    }
    
    private void setNewWaypoints()
    {
        waypoints = new List<Transform>();
        Transform waypointsTransform = currentRaceTrack.transform.Find("Waypoints");

        foreach (Transform t in waypointsTransform)
        {
            waypoints.Add(t);
        }
    }


    private IEnumerator race()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < distanceToChangeWaypoint)
            {
                currentWaypoint++;
            }

            Vector3 waypointPos = waypoints[currentWaypoint].position;
            Vector3 localTarget = transform.InverseTransformPoint(waypointPos);
            float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

            if (Mathf.Abs(angle) > maxTurnAngle)
            {
                angle = maxTurnAngle * Mathf.Sign(angle);
            }

            foreach (WheelCollider collider in frontWheelColliders)
            {
                collider.steerAngle = Mathf.Lerp(collider.steerAngle, angle, turnSpeed * Time.deltaTime);
            }

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }

            setMotorTorque(motorTorque);

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



    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(waypoints[currentWaypoint].position, 1);
    }

}
