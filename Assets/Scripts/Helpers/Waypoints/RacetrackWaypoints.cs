using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RacetrackWaypoints : MonoBehaviour
{
    [SerializeField] private Color trackColor;
    [SerializeField] private float radius;
    [SerializeField] private float upDistance;
    [SerializeField] private LayerMask mapMask;

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

            Gizmos.DrawSphere(transform.GetChild(i).position, radius);
        }
    }
}
