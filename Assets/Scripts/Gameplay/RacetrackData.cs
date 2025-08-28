using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

public class RacetrackData : MonoBehaviour
{
    [SerializeField] private Transform raceTrack;
    [SerializeField] private Transform places;
    [SerializeField] private Transform waypoints;
    [SerializeField] private Transform checkpoints;
    [SerializeField] private Transform finishCameraPositions;
    [SerializeField] private Volume globalVolume;

    public Transform getRaceTrack() => raceTrack;
    public Transform getPlaces() => places;
    public Transform getWaypoints() => waypoints;
    public Transform getCheckpoints() => checkpoints;
    public Transform getFinishCameraPositions() => finishCameraPositions;
    public Volume getGlobalVolume() => globalVolume;
}
