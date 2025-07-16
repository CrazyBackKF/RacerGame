using System.Collections.Generic;
using UnityEngine;

public class WheelStabilizator : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float stabilizatorPower = 1f;

    [Header("Components")]
    [SerializeField] private List<WheelCollider> wheelColliders;
    [SerializeField] private Rigidbody rb;

    private void Update()
    {
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            if (!wheelCollider.isGrounded)
            {
                rb.AddForceAtPosition(Vector3.down * stabilizatorPower, wheelCollider.transform.position, ForceMode.Force);
            }
        }
    }
}
