using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RacetrackWaypoints : MonoBehaviour
{
    [SerializeField] private Color trackColor;
    [SerializeField] private float upDistance;
    [SerializeField] private LayerMask mapMask;
    [SerializeField] private float minDistanceToDouble;

    [ContextMenu("Calibrate Waypoints")]
    private void calibrateWaypoints()
    {
        RenameWaypoints();
        changeYCoordinate();
        rotateToNextWaypoint();
        addWaypointOffsets();
    }

    [ContextMenu("Rename Waypoints")]
     private void RenameWaypoints()
     {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.name = (i + 1).ToString();
        }
     }

    [ContextMenu("Change y coordinate")]
    private void changeYCoordinate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (Physics.Raycast(transform.GetChild(i).position, Vector3.down, out RaycastHit hit, 100f))
            {
                transform.GetChild(i).position = new Vector3(transform.GetChild(i).position.x, hit.point.y + upDistance, transform.GetChild(i).position.z);
            }
        }
    }

    [ContextMenu("Rotate to next waypoint")]
    private void rotateToNextWaypoint()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Vector3 rotation;
            if (i == transform.childCount - 1)
            {
                rotation = (transform.GetChild(0).transform.position - transform.GetChild(i).transform.position).normalized;
            }
            else
            {
                rotation = (transform.GetChild(i + 1).transform.position - transform.GetChild(i).transform.position).normalized;
            }
            transform.GetChild(i).rotation = Quaternion.LookRotation(rotation);
        }
    }

    [ContextMenu("Add WaypointOffsets")]
    private void addWaypointOffsets()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            
            if (!child.TryGetComponent<MaxWaypointRandomOffset>(out MaxWaypointRandomOffset comp))
            {
                comp = child.AddComponent<MaxWaypointRandomOffset>();
            }

            comp.calculatePoints(mapMask, 100f);
        }
    }

    [ContextMenu("Double the waypoints")]
    private void doubleTheWaypoints()
    {
        int originalCount = transform.childCount;
        Transform[] waypoints = new Transform[originalCount];

        for (int i = 0; i < originalCount; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }

        for (int i = 0; i < originalCount; i++)
        {
            Vector3 firstWaypoint = waypoints[i].position;
            Vector3 secondWaypoint = waypoints[(i + 1) % originalCount].position;

            if (Vector3.Distance(firstWaypoint, secondWaypoint) < minDistanceToDouble) continue; 

            GameObject newObject = new GameObject("new");
            newObject.transform.parent = transform;
            newObject.transform.position = (firstWaypoint + secondWaypoint) / 2;

            int index = waypoints[i].GetSiblingIndex();
            newObject.transform.SetSiblingIndex(index + 1);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = trackColor;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (i > 0)
            {
                Gizmos.DrawLine(transform.GetChild(i - 1).position, transform.GetChild(i).position);
            }
            else
            {
                Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(i).position);
            }
        }
    }
}
