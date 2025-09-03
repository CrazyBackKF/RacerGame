using TMPro;
using UnityEngine;

public class LeaderboardsEntryScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI place;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI model;
    [SerializeField] private TextMeshProUGUI time;

    public void setTextMeshes(string place, string playerName, string model, string time)
    {
        this.place.text = place;
        this.playerName.text = playerName;
        this.model.text = model;
        this.time.text = time;
    }
}
