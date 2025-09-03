using System.Collections;
using TMPro;
using UnityEngine;

public class TimerTextManager : MonoBehaviour
{
    public static TimerTextManager Instance {  get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;

    private float time;

    private bool shouldTimerRun;

    private void OnEnable()
    {
        GameManager.Instance.onRaceStarted += startTimer;
        if (shouldTimerRun)
        {
            StartCoroutine(timerCoroutine());
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.onRaceStarted -= startTimer;
    }

    private void startTimer(object sender, System.EventArgs e)
    {
        stopTimer();
        resetTimer();
        shouldTimerRun = true;
        StartCoroutine(timerCoroutine());
    }

    public void stopTimer()
    {
        shouldTimerRun = false;
        time = 0;
        StopAllCoroutines();
    }

    private IEnumerator timerCoroutine()
    {
        while (true)
        {
            timerText.text = FrequentFunctions.timeToText((int)time);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void resetTimer()
    {
        timerText.text = "00.00";
    }

    
}
