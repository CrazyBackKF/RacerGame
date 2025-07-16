using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Full cycle time in minutes")]
    [SerializeField] private float time;

    [Header("Percent values of next times of day")]
    [SerializeField] private List<float> percentValues;

    [Header("Intensity values of next times of day")]
    [SerializeField] private List<float> intensityValues;

    [Header("Shader indexes of next times of day")]
    [SerializeField] private List<int> shaderIndexes;

    [Header("Shaders")]
    [SerializeField] private List<Material> skyboxMaterials;

    private void Start()
    {
        startDayNightCycle();
    }

    private void startDayNightCycle()
    {
        StopAllCoroutines();
        StartCoroutine(dayNightCycle());
    }

    private IEnumerator dayNightCycle()
    {
        int currentIndex = 0;

        while (true)
        {
            int nextIndex = (currentIndex + 1) % percentValues.Count;

            float durationSeconds = time * 60f * (percentValues[nextIndex] - percentValues[currentIndex]) / 100f;
            if (durationSeconds <= 0f) durationSeconds = time * 60f * percentValues[nextIndex] / 100f;

            yield return StartCoroutine(LerpIntensity(
                intensityValues[currentIndex],
                intensityValues[nextIndex],
                durationSeconds));

            RenderSettings.skybox = skyboxMaterials[shaderIndexes[nextIndex]];

            currentIndex = nextIndex;
        }
    }

    private IEnumerator LerpIntensity(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentIntensity = Mathf.Lerp(from, to, t);
            RenderSettings.ambientIntensity = currentIntensity;
            RenderSettings.reflectionIntensity = currentIntensity;
            yield return null;
        }
    }
}
