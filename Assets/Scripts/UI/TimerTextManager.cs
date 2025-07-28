using System.Collections;
using TMPro;
using UnityEngine;

public class TimerTextManager : MonoBehaviour
{
    public static TimerTextManager Instance {  get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;

    private float time;

    private void Start()
    {
        resetTimer();
        GameManager.Instance.onRaceStarted += startTimer;
    }

    private void startTimer(object sender, System.EventArgs e)
    {
        stopTimer();
        resetTimer();
        StartCoroutine(timerCoroutine());
    }

    public void stopTimer()
    {
        time = 0;
        StopAllCoroutines();
    }

    private IEnumerator timerCoroutine()
    {
        while (true)
        {
            timerText.text = timeToText((int)time);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private void resetTimer()
    {
        timerText.text = "00.00";
    }

    private string timeToText(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;

        string minutesString = minutes.ToString();
        string secondsString = seconds.ToString();

        if (minutes < 10) minutesString = "0" + minutesString;
        if (seconds < 10) secondsString = "0" + secondsString;

        return (minutesString + "." + secondsString);
    }
}
