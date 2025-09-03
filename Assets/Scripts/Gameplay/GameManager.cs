using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    private const int botsNumber = 3;

    public static GameManager Instance { get; private set; }

    [Header("CarList")]
    [SerializeField] private List<CarsSO> carSOList;
    private int currentCarIndex;
    [Space]
    [Space]
    [Header("MapList")]
    [SerializeField] private List<MapsSO> mapSOList;
    private int currentMapIndex;
    [Space]
    [Space]

    [SerializeField] private int startTimer;
    private RacetrackData racetrackData;
    private GameObject playerCar;
    [SerializeField] private bool shouldPlay;
    private float countdownTime;
    private List<GameObject> cars;
    private CheckpointManager playerCheckpointManager;

    [SerializeField] private int maxLaps;

    public event EventHandler onRaceStarted;
    public event EventHandler onRaceFinished;
    public event EventHandler onGameConfigurated;

    [SerializeField] private GameObject playerCarPrefab;
    [SerializeField] private GameObject botPrefab;

    public enum State
    {
        MainMenu,
        Countdown,
        Race,
        Finish
    }

    public State currentState { get; private set; }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        currentState = State.MainMenu;
        if (shouldPlay)
        {
            currentState = State.Countdown;
        }
        Inputs.Instance.changeCurrentInputMap("RaceStartCountdown");

        SelectCarUIScript.Instance.onCarSelected += SelectCarUIScript_onCarSelected;
        SceneLoader.Instance.onLevelLoaded += SceneLoader_onLevelLoaded;
    }

    private void SceneLoader_onLevelLoaded(object sender, EventArgs e)
    {
        racetrackData = FindFirstObjectByType<RacetrackData>();

        playerCar = Instantiate(playerCarPrefab, racetrackData.getPlaces().GetChild(0).position, racetrackData.getPlaces().GetChild(0).rotation);
        GameObject carModel = Instantiate(carSOList[currentCarIndex].model, playerCar.transform);
        playerCar.GetComponent<CarsInGameStats>().carSO = carSOList[currentCarIndex];

        playerCheckpointManager = playerCar.GetComponent<CheckpointManager>();

        for (int i = 1; i <= botsNumber; i++)
        {
            Instantiate(botPrefab, racetrackData.getPlaces().GetChild(i).position, racetrackData.getPlaces().GetChild(i).rotation);
        }

        onGameConfigurated?.Invoke(this, EventArgs.Empty);

        currentState = State.Countdown;
    }

    private void SelectCarUIScript_onCarSelected(object sender, SelectCarUIScript.OnCarSelectedEventArgs e)
    {
        currentCarIndex = e.carIndex;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.MainMenu:
                break;

            case State.Countdown:
                countdownTime += Time.deltaTime;
                if (countdownTime >= startTimer)
                {
                    playerCar.GetComponent<CheckpointManager>().onRaceFinished += PlayerCar_onRaceFinished;
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

    private void PlayerCar_onRaceFinished(object sender, EventArgs e)
    {
        currentState = State.Finish;

        onRaceFinished?.Invoke(this, EventArgs.Empty);

        playerCar.GetComponent<CheckpointManager>().onRaceFinished -= PlayerCar_onRaceFinished;
    }

    private void findCars()
    {
        cars = new List<GameObject>();
        GameObject[] carsArr = GameObject.FindGameObjectsWithTag("car");

        string[] botNames = { "bot1", "bot2", "bot3" };

        for (int i = 0; i < carsArr.Length; i++)
        {
            CarsInGameStats carInGameStats = carsArr[i].GetComponent<CarsInGameStats>();
            carInGameStats.playerName = botNames[i % botNames.Length];

            cars.Add(carsArr[i]);
        }
    }

    public bool isGamePlaying()
    {
        return currentState == State.Race || currentState == State.Countdown;
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

    public RacetrackData getCurrentRacetrackData()
    {
        return racetrackData;
    }
    public List<CarsSO> getCarsSO()
    {
        return carSOList;
    }

    public List<MapsSO> getMapsSO()
    {
        return mapSOList;
    }

    public bool isPlayerRacing()
    {
        return !playerCheckpointManager.finished;
    }
}
