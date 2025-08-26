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
    [SerializeField] private CinemachineCamera mainCinemachineCamera;
    [SerializeField] private CinemachineCamera finishCinemachineCamera;

    public Transform getRaceTrack() => raceTrack;
    public Transform getPlaces() => places;
    public Transform getWaypoints() => waypoints;
    public Transform getCheckpoints() => checkpoints;
    public Transform getFinishCameraPositions() => finishCameraPositions;
    public Volume getGlobalVolume() => globalVolume;
    public CinemachineCamera getMainCinemachineCamera() => mainCinemachineCamera;
    public CinemachineCamera getFinishCinemachineCamera() => finishCinemachineCamera;
}
