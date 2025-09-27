using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void setCameraShake(float amplitude, float frequency, CinemachineBasicMultiChannelPerlin perlin)
    {
        perlin.AmplitudeGain = amplitude;
        perlin.FrequencyGain = frequency;
    }

    public void setCameraShakeForSeconds(float amplitude, float frequency, float seconds, CinemachineBasicMultiChannelPerlin perlin)
    {
        StartCoroutine(setCameraShakeIEnumerator(amplitude, frequency, seconds, perlin));
    }

    private IEnumerator setCameraShakeIEnumerator(float amplitude, float frequency, float seconds, CinemachineBasicMultiChannelPerlin perlin)
    {
        perlin.AmplitudeGain = amplitude;
        perlin.FrequencyGain = frequency;

        yield return new WaitForSeconds(seconds);

        perlin.AmplitudeGain = 0;
        perlin.FrequencyGain = 0;
    }
}
