using UnityEngine;

public class FrequentFunctions
{
    public static string timeToText(int time)
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
