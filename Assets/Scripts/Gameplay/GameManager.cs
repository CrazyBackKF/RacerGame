using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int startTimer;
    private float countdownTime;

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
                }
                break;
        }
    }
}
