using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PositionTextManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentPosition;
    [SerializeField] private TextMeshProUGUI allCarsNumber;

    [SerializeField] private GameObject playerCar;

    private void Update()
    {
        List<GameObject> cars = GameManager.Instance.getCarsList();
        if (cars == null || cars.Count == 0) return;
        int playerPosition = cars.FindIndex(car => car == playerCar) + 1;

        currentPosition.text = playerPosition.ToString();
        allCarsNumber.text = cars.Count.ToString();
    }
}
