using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinishUIScript : MonoBehaviour
{
    [SerializeField] private RectTransform finishMoneyUI;
    [SerializeField] private TextMeshProUGUI moneyTextMesh;
    [SerializeField] private CanvasGroup blackForeground;
    [SerializeField] private GameObject leaderboards;
    [SerializeField] private GameObject leaderboardsEntry;
    [SerializeField] private Transform placesTransform;
    [SerializeField] private float timeScaleDropSpeed;

    private void Start()
    {
        Player_onRaceFinished(this, System.EventArgs.Empty);

        GameManager.Instance.onGameConfigurated += GameManager_onGameConfigurated;
    }

    private void GameManager_onGameConfigurated(object sender, System.EventArgs e)
    {
        GameManager.Instance.onRaceFinished += Player_onRaceFinished;
    }

    private void Player_onRaceFinished(object sender, System.EventArgs e)
    {
        LTSeq seq = LeanTween.sequence();

        seq.append(LeanTween.value(1f, 0f, 5f)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float value) => Time.timeScale = value)
            .setOnComplete(() => finishMoneyUI.gameObject.SetActive(true)));

        seq.append(LeanTween.moveY(finishMoneyUI, 0f, 1f).setIgnoreTimeScale(true).setEaseInQuad());

        float money = 10000;

        seq.append(LeanTween.value(0, money, 0.0003f * money)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float value) => moneyTextMesh.text = "+$ " + ((int)value).ToString()));

        seq.append(2f);

        seq.append(LeanTween.alphaCanvas(blackForeground, 1, 0.5f)
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                finishMoneyUI.gameObject.SetActive(false);
                leaderboards.SetActive(true);

                fillLeaderboards();
            }));

        seq.append(2f);

        seq.append(LeanTween.alphaCanvas(blackForeground, 0, 0.5f).setIgnoreTimeScale(true));
    }

    private void fillLeaderboards()
    {
        List<GameObject> cars = GameManager.Instance.getCarsList();

        for (int i = 0; i < cars.Count; i++)
        {
            CarsInGameStats carInGameStats = cars[i].GetComponent<CarsInGameStats>();

            GameObject currentEntry = Instantiate(leaderboardsEntry, placesTransform);
            string place = (i + 1).ToString() + ".";
            string playerName = carInGameStats.playerName;
            string model = carInGameStats.carSO.carName;
            string time = FrequentFunctions.timeToText((int)carInGameStats.getTime());
            currentEntry.GetComponent<LeaderboardsEntryScript>().setTextMeshes(place, playerName, model, time);
        }
    }
}
