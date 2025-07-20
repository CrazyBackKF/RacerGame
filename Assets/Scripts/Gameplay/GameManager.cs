using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int startTimer;
    private float countdownTime;
    public float startRaceTime { get; private set; }

    public enum State
    {
        Countdown,
        Race
    }

    private State currentState;

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
                    startRaceTime = Time.time;
                    CheckpointManager.Instance.startGame();
                }
                break;
        }
    }
}
