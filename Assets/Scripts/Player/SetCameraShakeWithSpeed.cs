using Unity.Cinemachine;
using UnityEngine;

public class SetCameraShakeWithSpeed : MonoBehaviour
{
    [SerializeField] private Vector2 amplitudeEdges;
    [SerializeField] private Vector2 frequencyEdges;
    [SerializeField] private Vector2 speedEdges;

    private CinemachineBasicMultiChannelPerlin perlin;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null) Debug.Log("Rigidbody jest null w " + typeof(SetCameraShakeWithSpeed));

        perlin = CinemachineCameraManager.Instance.getMainCinemachineCamera().GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        float normalizedSpeed = Mathf.InverseLerp(speedEdges.x, speedEdges.y, speed);

        CameraShake.Instance.setCameraShake(Mathf.Lerp(amplitudeEdges.x, amplitudeEdges.y, normalizedSpeed), Mathf.Lerp(frequencyEdges.x, frequencyEdges.y, normalizedSpeed), perlin);

    }
}
