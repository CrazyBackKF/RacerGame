using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuGameObject;
    [SerializeField] private List<GameObject> gameObjectsToDisable;

    [SerializeField] private UIDocument ui;
    private bool isGamePaused = false;

    private Button resumeButton;

    private void Start()
    {
        Inputs.Instance.pause().performed += (UnityEngine.InputSystem.InputAction.CallbackContext obj) => pauseOrUnpaseGame(!isGamePaused);
        
        
    }

    private void pauseOrUnpaseGame(bool isPaused)
    {
        isGamePaused = isPaused;
        UnityEngine.Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        pauseMenuGameObject.SetActive(isPaused);

        foreach (GameObject go in gameObjectsToDisable)
        {
            go.SetActive(!isPaused);
        }

        Time.timeScale = isPaused ? 0 : 1;
        subscribeEvents(isPaused);
    }

    private void subscribeEvents(bool shouldSubscribe)
    {
        if (shouldSubscribe)
        {
            resumeButton = ui.rootVisualElement.Q<Button>("Resume") as Button;
            resumeButton.RegisterCallback<ClickEvent>((ClickEvent evt) => pauseOrUnpaseGame(false));
        }
        else
        {
            resumeButton.UnregisterCallback<ClickEvent>((ClickEvent evt) => pauseOrUnpaseGame(false));
        }
    }
}
