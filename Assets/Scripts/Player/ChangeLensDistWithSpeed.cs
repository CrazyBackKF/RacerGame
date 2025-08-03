using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChangeLensDistWithSpeed : MonoBehaviour
{
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float changeSpeed;

    private Rigidbody rb;
    private LensDistortion distortion;

    private float maxLensDistortion = 0.5f;
    private float maxSpeed = 30;
    private float minSpeed = 20;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.Log("Rigidbody jest null w skrypcie " + typeof(ChangeLensDistWithSpeed));

        if (globalVolume.profile.TryGet<LensDistortion>(out LensDistortion distortion))
        {
            this.distortion = distortion;
        }
        else Debug.Log("LensDistortion jest null w skrypcie " + typeof(ChangeLensDistWithSpeed));
    }

    private void Update()
    {
        if (rb.linearVelocity.sqrMagnitude > minSpeed * minSpeed)
        {
            float distortionValue = Mathf.Clamp(rb.linearVelocity.magnitude * maxLensDistortion / maxSpeed, 0, maxLensDistortion);
            distortion.intensity.value = Mathf.Lerp(distortion.intensity.value, -distortionValue, Time.deltaTime * changeSpeed);
        }
    }
}
