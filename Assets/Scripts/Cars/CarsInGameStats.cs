using System.Collections;
using UnityEngine;

public class CarsInGameStats : MonoBehaviour
{
    public string playerName;
    private float time;
    public CarsSO carSO;

    private void Awake()
    {
        GameManager.Instance.onRaceStarted += GameManager_onRaceStarted;
        GameManager.Instance.onRaceFinished += GameManager_onRaceFinished;
    }

    private void GameManager_onRaceStarted(object sender, System.EventArgs e)
    {
        time = 0;
        StartCoroutine(countTime());
    }

    private void GameManager_onRaceFinished(object sender, System.EventArgs e)
    {
        StopAllCoroutines();
    }

    private IEnumerator countTime()
    {
        time += Time.deltaTime;
        yield return null;
    }

    public float getTime()
    { 
        return time; 
    }
}
