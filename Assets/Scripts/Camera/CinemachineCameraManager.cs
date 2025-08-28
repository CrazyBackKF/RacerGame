using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraManager : MonoBehaviour
{
    public static CinemachineCameraManager Instance { get; private set; }

    [SerializeField] private CinemachineCamera finishCamera;
    [SerializeField] private CinemachineCamera normalCamera;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.onGameConfigurated += GameManager_onGameConfigurated;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onRaceFinished -= onRaceFinished;
        }
    }

    private void GameManager_onGameConfigurated(object sender, System.EventArgs e)
    {
        normalCamera.Target.TrackingTarget = GameManager.Instance.getPlayerCar().transform;
        GameManager.Instance.onRaceFinished += onRaceFinished;
    }

    private void onRaceFinished(object sender, System.EventArgs e)
    {
        Transform finishCameraPositions = GameManager.Instance.getCurrentRacetrackData().getFinishCameraPositions();
        Transform finishCameraPosition = finishCameraPositions.GetChild(Random.Range(0, finishCameraPositions.childCount));

        finishCamera.Target.TrackingTarget = finishCameraPosition;

        finishCamera.Priority = 1;
        normalCamera.Priority = 0;
    }

    public CinemachineCamera getMainCinemachineCamera() => normalCamera;
    public CinemachineCamera getFinishCinemachineCamera() => finishCamera;
}
