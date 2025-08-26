using System.Collections.Generic;
using UnityEngine;

public class WheelStabilizator : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float stabilizatorPower = 1f;

    [Header("Components")]
    private List<WheelCollider> wheelColliders;
    [SerializeField] private Rigidbody rb;

    private void Start()
    {
        CarData carData = transform.GetChild(0).GetComponent<CarData>();
        wheelColliders = carData.getWheelColliders();

        foreach (WheelCollider collider in wheelColliders)
        {
            collider.enabled = true;
        }
    }

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

    public void setWheelColliders(List<WheelCollider> wheelColliders)
    {
        this.wheelColliders = wheelColliders;
    }
}
