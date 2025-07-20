using TMPro;
using UnityEngine;

public class TimerTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        timerText.text = timeToText((int)(Time.time - GameManager.Instance.startRaceTime));
    }

    public string timeToText(int time)
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
