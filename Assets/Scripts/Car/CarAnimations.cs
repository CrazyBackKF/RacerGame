using System.Collections.Generic;
using UnityEngine;

public class CarAnimations : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private List<Transform> wheelTransforms;
    [SerializeField] private List<WheelCollider> wheelColliders;

    private void Update()
    {
        float speed = rb.linearVelocity.magnitude * Time.deltaTime * Mathf.Rad2Deg;

        for (int i = 0; i < wheelTransforms.Count; i++)
        {
            Quaternion rotation = Quaternion.Euler(speed / wheelColliders[i].radius, 0, 0);
            wheelTransforms[i].localRotation *= rotation;
        }
    }
}
