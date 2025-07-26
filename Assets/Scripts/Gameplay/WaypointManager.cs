using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] private Transform currentRaceTrack;
    [SerializeField] private float minDistance;

    private List<Transform> waypoints;
    private int currentWaypoint = 0;
    private int currentLap = 0;

    private bool isFirstFrame = true;

    public event EventHandler<OnWaypointPassedEventArgs> onWaypointPassed;

    public class OnWaypointPassedEventArgs : EventArgs
    {
        public int currentWaypoint;
        public Vector3 currentTargetForBots;
        public int currentLap;
    }

    private void findWaypoints()
    {
        waypoints = new List<Transform>();
        Transform waypointsList = currentRaceTrack.Find("Waypoints");

        foreach (Transform t in waypointsList)
        {
            waypoints.Add(t);
        }
    }

    private void startRace()
    {
        findWaypoints();
        onWaypointPassed?.Invoke(this, new OnWaypointPassedEventArgs 
        { 
            currentWaypoint = currentWaypoint, 
            currentTargetForBots = Vector3.Lerp(waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMinPoint(), waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMaxPoint(), UnityEngine.Random.Range(0.25f, 0.75f)),
            currentLap = currentLap
        });
    }

    private void Update()
    {
        if (isFirstFrame)
        {
            isFirstFrame = false;
            startRace();
        }

        if (currentWaypoint >= waypoints.Count) return;

        Vector3 minPoint = waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMinPoint();
        Vector3 maxPoint = waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMaxPoint();

        Vector3 AB = maxPoint - minPoint;
        Vector3 AP = transform.position - minPoint;

        float t = Vector3.Dot(AP, AB) / Vector3.Dot(AB, AB);

        t = Mathf.Clamp01(t);

        Vector3 Q = minPoint + t * AB;

        Vector3 perpendicularVector = transform.position - Q;

        if (perpendicularVector.magnitude < minDistance)
        {
            if (currentWaypoint < waypoints.Count - 1) currentWaypoint++;
            else
            {
                currentWaypoint = 0;
                currentLap++;
            }

            onWaypointPassed?.Invoke(this, new OnWaypointPassedEventArgs
            {
                currentWaypoint = currentWaypoint,
                currentTargetForBots = Vector3.Lerp(waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMinPoint(), waypoints[currentWaypoint].GetComponent<MaxWaypointRandomOffset>().getMaxPoint(), UnityEngine.Random.Range(0.25f, 0.75f)),
                currentLap = currentLap
            });
        }
    }

    public int getCurrentWaypoint()
    {
        int closestWaypoint = 0;
        float minDistance = Vector3.Distance(transform.position, waypoints[0].transform.position);

        for (int i = 0; i < waypoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].transform.position);
            if (distance < minDistance)
            {
                closestWaypoint = i;
                minDistance = distance;
            }
        }

        return (closestWaypoint + currentLap * 1000);
    }
}
