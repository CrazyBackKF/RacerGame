using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointManager : MonoBehaviour
{
    [SerializeField] private float minDistance;
    [SerializeField] private bool isPlayer;

    [SerializeField] private CheckpointManager checkpointManager;

    private List<Transform> waypoints;
    private int currentWaypoint = 0;
    private int maxLaps = 0;

    private bool isRaceStarted = false;

    public event EventHandler<OnWaypointPassedEventArgs> onWaypointPassed;

    public class OnWaypointPassedEventArgs : EventArgs
    {
        public int currentWaypoint;
        public Vector3 currentTargetForBots;
        public int currentLap;
    }

    private void Start()
    {
        GameManager.Instance.onRaceStarted += startRace;
    }

    private void startRace(object sender, EventArgs e)
    {
        isRaceStarted = true;
        findWaypoints(GameManager.Instance.getCurrentWaypoints());
        maxLaps = GameManager.Instance.getMaxLaps();
        onWaypointPassed?.Invoke(this, new OnWaypointPassedEventArgs
        {
            currentWaypoint = currentWaypoint,
            currentTargetForBots = waypoints[currentWaypoint].position,
            currentLap = checkpointManager.getCurrentLap()
        });
    }

    private void findWaypoints(Transform currentWaypointsTransform)
    {
        waypoints = new List<Transform>();

        foreach (Transform t in currentWaypointsTransform)
        {
            waypoints.Add(t);
        }
    }

    private void Update()
    {
        if (!isRaceStarted || currentWaypoint >= waypoints.Count) return;

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
            }

            onWaypointPassed?.Invoke(this, new OnWaypointPassedEventArgs
            {
                currentWaypoint = currentWaypoint,
                currentTargetForBots = waypoints[currentWaypoint].position,
                currentLap = checkpointManager.getCurrentLap()
            });
        }

        if (checkpointManager.finished) isRaceStarted = false;
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

        return (closestWaypoint + checkpointManager.getCurrentLap() * (waypoints.Count + 1));
    }
}
