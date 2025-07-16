using UnityEngine;

public class ShadowsNearPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float enableDistance = 10f;

    private Light[] allLights;

    void Start()
    {
        allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light light in allLights)
        {
            light.shadows = LightShadows.None;
        }
    }

    void Update()
    {
        foreach (Light light in allLights)
        {
            if (Vector3.Distance(light.transform.position, player.position) <= enableDistance)
                light.shadows = LightShadows.Soft;
            else
                light.shadows = LightShadows.None;
        }
    }
}
