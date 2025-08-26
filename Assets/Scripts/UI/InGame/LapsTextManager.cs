using TMPro;
using UnityEngine;

public class LapsTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentLap;
    [SerializeField] private TextMeshProUGUI allLaps;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        GameManager.Instance.onGameConfigurated += GameManager_onGameConfigurated;
    }

    private void GameManager_onGameConfigurated(object sender, System.EventArgs e)
    {
        Show();
        CheckpointManager playerManager = GameManager.Instance.getPlayerCar().GetComponent<CheckpointManager>();

        playerManager.onRaceStarted += (object sender, CheckpointManager.OnRaceStartedAndOnLapFinishedEventArgs e) => { allLaps.text = e.lap.ToString(); };
        playerManager.onLapFinished += (object sender, CheckpointManager.OnRaceStartedAndOnLapFinishedEventArgs e) => { currentLap.text = e.lap.ToString(); };
    }

    private void Show()
    {
        canvas.enabled = true;
    }

    private void Hide()
    {
        canvas.enabled = false;
    }
}
