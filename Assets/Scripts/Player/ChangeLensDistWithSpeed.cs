using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChangeLensDistWithSpeed : MonoBehaviour
{
    private Volume globalVolume;
    [SerializeField] private float changeSpeed;

    private Rigidbody rb;
    private LensDistortion distortion;

    private float maxLensDistortion = 0.6f;
    private float maxSpeed = 40;
    private float minSpeed = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        globalVolume = GameManager.Instance.getGlobalVolume();

        if (rb == null) Debug.Log("Rigidbody jest null w skrypcie " + typeof(ChangeLensDistWithSpeed));

        if (globalVolume.profile.TryGet<LensDistortion>(out LensDistortion distortion))
        {
            this.distortion = distortion;
        }
        else Debug.Log("LensDistortion jest null w skrypcie " + typeof(ChangeLensDistWithSpeed));
    }

    private void Update()
    {
        if (!GameManager.Instance.isPlayerRacing())
        {
            distortion.intensity.value = 0;
        }

        else if (rb.linearVelocity.sqrMagnitude > minSpeed * minSpeed)
        {
            float distortionValue = Mathf.Clamp(rb.linearVelocity.magnitude * maxLensDistortion / maxSpeed, 0, maxLensDistortion);
            distortion.intensity.value = Mathf.Lerp(distortion.intensity.value, -distortionValue, Time.deltaTime * changeSpeed);
        }

        else
        {
            distortion.intensity.value = Mathf.Lerp(distortion.intensity.value, 0, Time.deltaTime * changeSpeed);
        }
    }
}
