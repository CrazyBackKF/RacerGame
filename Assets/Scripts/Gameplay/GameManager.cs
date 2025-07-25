using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int startTimer;
    private float countdownTime;
    private List<GameObject> cars;

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
                    CheckpointManager.Instance.startGame();
                    TimerTextManager.Instance.startTimer();
                    findCars();
                }
                break;
            case State.Race:
                //cars.Sort((a, b) =>
                //{
                //    int aWaypoints = a.GetComponent<CheckpointManager>().getCurrentWaypoints();
                //    int bWaypoints = b.GetComponent<CheckpointManager>().getCurrentWaypoints();

                //    return bWaypoints.CompareTo(aWaypoints);
                //});
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
}
