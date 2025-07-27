using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] private Transform currentRaceTrack;
    [SerializeField] private float minDistance;
    [SerializeField] private bool isPlayer;

    private List<Transform> waypoints;
    private int currentWaypoint = 0;
    private int currentLap = 0;
    private int maxLaps;
    public bool finished {  get; private set; }

    private bool isFirstFrame = true;

    public event EventHandler<OnWaypointPassedEventArgs> onWaypointPassed;
    public event EventHandler onRaceFinished;

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
        finished = false;
        findWaypoints();
        maxLaps = GameManager.Instance.getMaxLaps();
        onWaypointPassed?.Invoke(this, new OnWaypointPassedEventArgs 
        { 
            currentWaypoint = currentWaypoint, 
            currentTargetForBots = waypoints[currentWaypoint].position,
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

        if ((perpendicularVector.magnitude < minDistance && currentWaypoint != waypoints.Count - 1) || perpendicularVector.magnitude < 1f)
        {
            if (currentWaypoint < waypoints.Count - 1) currentWaypoint++;
            else
            {
                currentWaypoint = 0;
                if (currentLap < maxLaps - 1) currentLap++;
                else raceEnd();
            }

            onWaypointPassed?.Invoke(this, new OnWaypointPassedEventArgs
            {
                currentWaypoint = currentWaypoint,
                currentTargetForBots = waypoints[currentWaypoint].position,
                currentLap = currentLap
            });
        }
    }

    private void raceEnd()
    {
        finished = true;
        onRaceFinished?.Invoke(this, EventArgs.Empty);
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

        return (closestWaypoint + currentLap * waypoints.Count);
    }
}
