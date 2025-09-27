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
        Time.timeScale = 1;
        StartCoroutine(loadSceneAsync(index));
    }

    public void loadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private IEnumerator loadSceneAsync(int index)
    {
        int GameSceneIndex = 1;
        AsyncOperation uiLoad = SceneManager.LoadSceneAsync(GameSceneIndex, LoadSceneMode.Single);

        while (!uiLoad.isDone)
        {
            yield return null;
        }

        AsyncOperation sceneLoadingOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        while (!sceneLoadingOperation.isDone)
        {
            yield return null;
        }

        onLevelLoaded?.Invoke(this, EventArgs.Empty);
    }
}
