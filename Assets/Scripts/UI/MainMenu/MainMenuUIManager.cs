using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenuUI;

    private void Awake()
    {
        VisualElement root = mainMenuUI.rootVisualElement;

        root.Q<Button>("PlayButton").RegisterCallback<ClickEvent>(playMenu);
        root.Q<Button>("SelectCarButton").RegisterCallback<ClickEvent>(selectCar);
        root.Q<Button>("ProfileButton").RegisterCallback<ClickEvent>(viewProfile);
        root.Q<Button>("ShopButton").RegisterCallback<ClickEvent>(viewShop);
        root.Q<Button>("ExitButton").RegisterCallback<ClickEvent>(quitGame);
    }

    private void playMenu(ClickEvent evt)
    {
        
    }

    private void selectCar(ClickEvent evt)
    {
        
    }

    private void viewProfile(ClickEvent evt)
    {
        
    }

    private void viewShop(ClickEvent evt)
    {
        
    }

    private void quitGame(ClickEvent evt)
    {
        Application.Quit();
    }
}
