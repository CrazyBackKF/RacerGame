using System.Collections.Generic;
using UnityEngine;

public class CarData : MonoBehaviour
{
    [SerializeField] private List<Transform> wheelTransforms;
    [SerializeField] private List<WheelCollider> wheelColliders;
    [SerializeField] private Transform steeringWheelTransform;

    [SerializeField] private List<WheelCollider> frontWheelColliders;
    [SerializeField] private List<WheelCollider> rearWheelColliders;

    public List<Transform> getWheelTransforms() => wheelTransforms;
    public List<WheelCollider> getWheelColliders() => wheelColliders;
    public List<WheelCollider> getFrontWheelColliders() => frontWheelColliders;
    public List<WheelCollider> getRearWheelColliders() => rearWheelColliders;
    public Transform getSteeringWheelTransform() => steeringWheelTransform;
}
