using UnityEngine;

[CreateAssetMenu(fileName = "CarsSO", menuName = "Scriptable Objects/CarsSO")]
public class CarsSO : ScriptableObject
{
    [Header("Main Information")]
    public string carName;
    public GameObject model;

    [Space]
    [Header("Stats")]
    public float maxSpeed;
    public float acceleration;
    public float braking;
    public float mass;
}
