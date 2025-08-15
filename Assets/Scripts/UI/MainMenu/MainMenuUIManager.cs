using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenuUI;

    private VisualElement selectCarElement;
    private VisualElement selectMapElement;
    private VisualElement root;

    private void Start()
    {
        startFunction();
    }

    private void playMenu(ClickEvent evt)
    {
        selectMapElement.AddToClassList("animationUp");
    }

    private void selectCarMenu(ClickEvent evt)
    {
        selectCarElement.AddToClassList("animationUp");
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

    private void startFunction()
    {
        root = mainMenuUI.rootVisualElement;

        root.Q<Button>("PlayButton").RegisterCallback<ClickEvent>(playMenu);
        root.Q<Button>("SelectCarButton").RegisterCallback<ClickEvent>(selectCarMenu);
        root.Q<Button>("ProfileButton").RegisterCallback<ClickEvent>(viewProfile);
        root.Q<Button>("ShopButton").RegisterCallback<ClickEvent>(viewShop);
        root.Q<Button>("ExitButton").RegisterCallback<ClickEvent>(quitGame);

        selectCarElement = root.Q<VisualElement>("SelectCarMenu");
        selectMapElement = root.Q<VisualElement>("SelectMapMenu");
    }
}
