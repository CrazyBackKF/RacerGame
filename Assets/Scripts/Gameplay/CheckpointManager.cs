using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [SerializeField] private Transform currentRacetrackCheckpointsTransform;
    [SerializeField] private int maxLaps;

    private List<GameObject> currentCheckpointList;
    private int currentCheckpointIndex = 0;
    private int currentLaps = 0;

    private bool shouldChangeLap = false;

    public event EventHandler<OnRaceStartedAndOnLapFinishedEventArgs> onRaceStarted;
    public event EventHandler<OnRaceStartedAndOnLapFinishedEventArgs> onLapFinished;

    public class OnRaceStartedAndOnLapFinishedEventArgs : EventArgs
    {
        public int lap;
    }

    private void Awake()
    {
        Instance = this;
    }


    public void startGame()
    {
        getCheckpoints(currentRacetrackCheckpointsTransform);
        hideCheckpoints(currentCheckpointIndex);
        onRaceStarted?.Invoke(this, new OnRaceStartedAndOnLapFinishedEventArgs { lap = maxLaps });
        onLapFinished?.Invoke(this, new OnRaceStartedAndOnLapFinishedEventArgs { lap = currentLaps + 1 });
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
        for (int i = 0; i < currentCheckpointList.Count; i++)
        {
            if (index == i) currentCheckpointList[i].SetActive(true);
            else currentCheckpointList[i].SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == currentCheckpointList[currentCheckpointIndex])
        {
            if (shouldChangeLap)
            {
                shouldChangeLap = false;
                currentLaps++;
                onLapFinished?.Invoke(this, new OnRaceStartedAndOnLapFinishedEventArgs { lap = currentLaps + 1 });

                if (currentLaps == maxLaps)
                {
                    endRace();
                }
            }

            if (currentCheckpointIndex < currentCheckpointList.Count - 1) currentCheckpointIndex++;
            else
            {
                currentCheckpointIndex = 0;
                if (currentLaps < maxLaps) shouldChangeLap = true;
            }
            Debug.Log(currentCheckpointIndex);
            hideCheckpoints(currentCheckpointIndex);
        }
    }

    private void endRace()
    {
        Debug.Log("Wygrales, skrypt CheckpointManager");
    }
}
