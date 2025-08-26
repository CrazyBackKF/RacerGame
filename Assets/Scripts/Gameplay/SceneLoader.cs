using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public event EventHandler onLevelLoaded;

    private void Awake()
    {
        Instance = this;
    }


    public void loadLevel(int index)
    {
        StartCoroutine(loadSceneAsync(index));
    }

    private IEnumerator loadSceneAsync(int index)
    {
        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);

        while (!sceneLoadingOperation.isDone)
        {
            yield return null;
        }

        int UISceneIndex = 1;
        AsyncOperation uiLoad = SceneManager.LoadSceneAsync(UISceneIndex, LoadSceneMode.Additive);

        while (!uiLoad.isDone)
        {
            yield return null;
        }

        onLevelLoaded?.Invoke(this, EventArgs.Empty);
    }
}
