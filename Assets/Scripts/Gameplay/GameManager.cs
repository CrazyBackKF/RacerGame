using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int startTimer;
    [SerializeField] private GameObject playerCar;
    [SerializeField] private GameObject currentRaceTrack;
    private float countdownTime;
    private List<GameObject> cars;

    [SerializeField] private int maxLaps;

    public event EventHandler onRaceStarted;

    public enum State
    {
        Countdown,
        Race
    }

    public State currentState { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
       currentState = State.Countdown;
       Inputs.Instance.changeCurrentInputMap("RaceStartCountdown");
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Countdown:
                countdownTime += Time.deltaTime;
                if (countdownTime >= startTimer)
                {
                    Inputs.Instance.changeCurrentInputMap("MainGameplay");
                    currentState = State.Race;
                    findCars();
                    onRaceStarted?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.Race:
                cars.Sort((a, b) =>
                {
                    CheckpointManager aManager = a.GetComponent<CheckpointManager>();
                    CheckpointManager bManager = b.GetComponent<CheckpointManager>();

                    if (aManager.finished && bManager.finished) return 0;
                    else if (aManager.finished) return -1;
                    else if (bManager.finished) return 1;

                    int aWaypoints = a.GetComponent<WaypointManager>().getCurrentWaypoint();
                    int bWaypoints = b.GetComponent<WaypointManager>().getCurrentWaypoint();

                    return bWaypoints.CompareTo(aWaypoints);
                });
                break;
        }
    }

    private void findCars()
    {
        cars = new List<GameObject>();
        GameObject[] carsArr = GameObject.FindGameObjectsWithTag("car");

        foreach (GameObject car in carsArr)
        {
            cars.Add(car);
        }
    }

    public List<GameObject> getCarsList()
    {
        return cars;
    }

    public int getMaxLaps()
    {
        return maxLaps;
    }

    public GameObject getPlayerCar()
    {
        return playerCar;
    }

    public GameObject getCurrentRaceTrack()
    {
        return currentRaceTrack;
    }

    public Transform getCurrentCheckpoints()
    {
        return currentRaceTrack.transform.Find("Checkpoints");
    }

    public Transform getCurrentWaypoints()
    {
        return currentRaceTrack.transform.Find("Waypoints");
    }

    public Transform getCurrentFinishCameraPositions()
    {
        return currentRaceTrack.transform.Find("FinishCameraPositions");
    }
}
