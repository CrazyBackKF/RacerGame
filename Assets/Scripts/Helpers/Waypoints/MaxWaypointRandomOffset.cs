using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

[DisallowMultipleComponent]
public class MaxWaypointRandomOffset : MonoBehaviour
{
    [SerializeField] private Vector3 minPoint;
    [SerializeField] private Vector3 maxPoint;

    public void calculatePoints(LayerMask mask, float maxDistance)
    {
        Physics.SphereCast(transform.position, 1f, -transform.right, out RaycastHit hitMin, maxDistance, mask);
        Physics.SphereCast(transform.position, 1f, transform.right, out RaycastHit hitMax, maxDistance, mask);

        minPoint = hitMin.point;
        maxPoint = hitMax.point;
    }

    public Vector3 getMinPoint()
    {
        return minPoint;
    }

    public Vector3 getMaxPoint()
    {
        return maxPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);

        if (minPoint == Vector3.zero || maxPoint == Vector3.zero) return;

        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(minPoint, 0.5f);
        Gizmos.DrawSphere(maxPoint, 0.5f);
    }
}
