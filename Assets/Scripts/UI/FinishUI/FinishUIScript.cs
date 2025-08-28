using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinishUIScript : MonoBehaviour
{
    [SerializeField] private RectTransform finishUiElements;
    [SerializeField] private TextMeshProUGUI moneyTextMesh;
    [SerializeField] private float timeScaleDropSpeed;

    private void Start()
    {
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
            .setOnComplete(() => finishUiElements.gameObject.SetActive(true)));
        seq.append(LeanTween.moveY(finishUiElements, 0f, 1f).setIgnoreTimeScale(true).setEaseInQuad());

        float money = 10000;

        seq.append(LeanTween.value(0, money, 0.0003f * money)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float value) => moneyTextMesh.text = "+$ " + ((int)value).ToString()));
    }
}
