using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GlobalClickableHandles
{
    static GlobalClickableHandles()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        Handles.color = Color.magenta;

        foreach (var comp in Object.FindObjectsByType<MaxWaypointRandomOffset>(FindObjectsSortMode.None))
        {
            if (Handles.Button(comp.transform.position, Quaternion.identity, 0.8f, 0.8f, Handles.SphereHandleCap))
            {
                Selection.activeGameObject = comp.gameObject;
            }
        }
    }
}
