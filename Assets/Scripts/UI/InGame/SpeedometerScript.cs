using UnityEngine;

public class SpeedometerScript : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private RectTransform speedometerPointer;
    private Rigidbody rb;

    [Header("Stats")]
    [SerializeField] private float speed;

    private float maxVelocity = 180;
    private float maxRotation = 256;

    private void Start()
    {
        GameManager.Instance.onGameConfigurated += GameManager_onGameConfigurated;
    }

    private void GameManager_onGameConfigurated(object sender, System.EventArgs e)
    {
        rb = GameManager.Instance.getPlayerCar().GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb == null) return;

        float velocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude * 3.6f;

        float angle = -maxRotation * velocity / maxVelocity;
        speedometerPointer.rotation = Quaternion.Slerp(speedometerPointer.rotation, Quaternion.Euler(0, 0, angle), speed * Time.deltaTime);
    }
}
