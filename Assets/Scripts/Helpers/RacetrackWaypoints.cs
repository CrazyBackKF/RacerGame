using System.Collections.Generic;
using UnityEngine;

public class RacetrackWaypoints : MonoBehaviour
{
    [SerializeField] private Color trackColor;
    [SerializeField] private float radius;
    [SerializeField] private float upDistance;

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
