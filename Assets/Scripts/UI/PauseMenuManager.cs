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

        VisualElement container = ui.rootVisualElement.Q<VisualElement>("MainContainer") as VisualElement;
        int waitingTime = 0;
        if (isPaused)
        {
            container.RemoveFromClassList("animatePauseOff");
        }
        else
        {
            container.AddToClassList("animatePauseOff");
            waitingTime = 500;
        }

        container.schedule.Execute(() =>
        {
            foreach (GameObject go in gameObjectsToDisable)
            {
                go.SetActive(!isPaused);
            }

            Time.timeScale = isPaused ? 0 : 1;
        }).StartingIn(waitingTime);

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
