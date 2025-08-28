using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    private int maxLaps;

    private List<GameObject> currentCheckpointList;
    private int currentCheckpointIndex = 0;
    private int currentLap = 0;

    public bool finished {  get; private set; }

    private bool shouldChangeLap = false;

    public event EventHandler<OnRaceStartedAndOnLapFinishedEventArgs> onRaceStarted;
    public event EventHandler<OnRaceStartedAndOnLapFinishedEventArgs> onLapFinished;
    public event EventHandler onRaceFinished;

    public class OnRaceStartedAndOnLapFinishedEventArgs : EventArgs
    {
        public int lap;
    }

    private void Start()
    {
        GameManager.Instance.onRaceStarted += startRace;
    }

    private void startRace(object sender, EventArgs e)
    {
        finished = false;
        getCheckpoints(GameManager.Instance.getCurrentRacetrackData().getCheckpoints());
        hideCheckpoints(currentCheckpointIndex);
        maxLaps = GameManager.Instance.getMaxLaps();
        onRaceStarted?.Invoke(this, new OnRaceStartedAndOnLapFinishedEventArgs { lap = maxLaps });
        onLapFinished?.Invoke(this, new OnRaceStartedAndOnLapFinishedEventArgs { lap = currentLap + 1 });
    }

    private void getCheckpoints(Transform checkpointsTransform)
    {
        List<GameObject> checkpointList = new List<GameObject>();

        foreach (Transform checkpoint in checkpointsTransform)
        {
            checkpointList.Add(checkpoint.gameObject);
        }

        currentCheckpointList = checkpointList;
    }

    private void hideCheckpoints(int index)
    {
        if (!isPlayer) return;
        
        for (int i = 0; i < currentCheckpointList.Count; i++)
        {
            currentCheckpointList[i].transform.Find("Visuals").gameObject.SetActive(index == i);
        }
    }

    public int getCurrentLap()
    {
        return currentLap;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;

        if (other.gameObject == currentCheckpointList[currentCheckpointIndex])
        {
            if (shouldChangeLap)
            {
                shouldChangeLap = false;
                currentLap++;
                onLapFinished?.Invoke(this, new OnRaceStartedAndOnLapFinishedEventArgs { lap = currentLap + 1 });

                if (currentLap == maxLaps)
                {
                    finished = true;
                    onRaceFinished?.Invoke(this, EventArgs.Empty);
                }
            }

            if (currentCheckpointIndex < currentCheckpointList.Count - 1) currentCheckpointIndex++;
            else
            {
                currentCheckpointIndex = 0;
                if (currentLap < maxLaps) shouldChangeLap = true;
            }

            hideCheckpoints(currentCheckpointIndex);
        }
    }
}
