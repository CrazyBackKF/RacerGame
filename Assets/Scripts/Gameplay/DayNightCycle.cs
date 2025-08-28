using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [System.Serializable]
    public class TimeOfDay
    {
        public TimeOfDays timeOfDay;
        public string name;
        public Material skyboxMaterial;
        public float intensityValue;
    }

    [SerializeField] private TimeOfDays currentTimeOfDay;

    [Header("Times Of Day")]
    [SerializeField] private List<TimeOfDay> timesOfDay;

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

    public enum TimeOfDays
    {
        BrightMorning,
        Morning,
        CloudyMorning,
        Noon,
        Afternoon,
        EarlyDusk,
        Sunset,
        HaloEvening,
        Night,
        Midnight
    }

    private void Start()
    {
        SceneLoader.Instance.onLevelLoaded += SceneLoader_onLevelLoaded;
    }

    private void SceneLoader_onLevelLoaded(object sender, System.EventArgs e)
    {
        changeTimeOfDay(timesOfDay.Find((time) => time.timeOfDay == currentTimeOfDay));
    }

    private void changeTimeOfDay(TimeOfDay timeOfDay)
    {
        RenderSettings.skybox = timeOfDay.skyboxMaterial;
        RenderSettings.ambientIntensity = timeOfDay.intensityValue;
        RenderSettings.reflectionIntensity = timeOfDay.intensityValue;
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
