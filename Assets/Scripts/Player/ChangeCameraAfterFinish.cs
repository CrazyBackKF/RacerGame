using Unity.Cinemachine;
using UnityEngine;

public class ChangeCameraAfterFinish : MonoBehaviour
{
    [SerializeField] private CinemachineCamera finishCamera;
    [SerializeField] private CinemachineCamera normalCamera;

    private void OnEnable()
    {
        GetComponent<CheckpointManager>().onRaceFinished += onRaceFinished;
    }
    private void OnDisable()
    {
        GetComponent<CheckpointManager>().onRaceFinished -= onRaceFinished;
    }

    private void onRaceFinished(object sender, System.EventArgs e)
    {
        Transform finishCameraPositions = GameManager.Instance.getCurrentFinishCameraPositions();
        Transform finishCameraPosition = finishCameraPositions.GetChild(Random.Range(0, finishCameraPositions.childCount));

        CameraTarget cameraTarget = new();
        cameraTarget.TrackingTarget = finishCameraPosition;
        cameraTarget.CustomLookAtTarget = false;

        finishCamera.Target = cameraTarget;

        finishCamera.Priority = 1;
        normalCamera.Priority = 0;
    }
}
