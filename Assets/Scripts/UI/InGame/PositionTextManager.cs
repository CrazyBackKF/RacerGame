using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PositionTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentPosition;
    [SerializeField] private TextMeshProUGUI allCarsNumber;

    private GameObject playerCar;

    private void Start()
    {
        GameManager.Instance.onGameConfigurated += GameManager_onGameConfigurated;
    }

    private void GameManager_onGameConfigurated(object sender, System.EventArgs e)
    {
        playerCar = GameManager.Instance.getPlayerCar();
    }

    private void Update()
    {
        if (playerCar == null) return;

        List<GameObject> cars = GameManager.Instance.getCarsList();
        if (cars == null || cars.Count == 0) return;
        int playerPosition = cars.FindIndex(car => car == playerCar) + 1;

        currentPosition.text = playerPosition.ToString();
        allCarsNumber.text = cars.Count.ToString();
    }
}
