using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenuUI;
    [SerializeField] private VisualTreeAsset carTemplate;

    private VisualElement selectCarElement;
    private VisualElement root;
    private ScrollView carScrollView;

    private bool isDraggingCarScrollView;
    private Vector2 lastPointerPos;

    private void Start()
    {
        startFunction();

        fillSelectCarMenu();
    }

    private void playMenu(ClickEvent evt)
    {
        
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
    }

    private void fillSelectCarMenu()
    {
        List<CarsSO> carsSoList = GameManager.Instance.getCarsSO();
        carScrollView = root.Q<ScrollView>("CarScrollView");

        foreach (CarsSO car in carsSoList)
        {
            VisualElement carVisualElement = carTemplate.CloneTree();
            carVisualElement.Q<Label>("CarName").text = car.carName;
            carScrollView.Add(carVisualElement);
        }

        carScrollView.RegisterCallback<PointerDownEvent>(evt =>
        {
            isDraggingCarScrollView = true;
            lastPointerPos = evt.position;
            carScrollView.CapturePointer(evt.pointerId);
        });

        carScrollView.RegisterCallback<PointerMoveEvent>(evt =>
        {
            if (!isDraggingCarScrollView) return;

            Vector2 delta = new Vector2(evt.position.x, evt.position.y) - lastPointerPos;
            lastPointerPos = evt.position;

            carScrollView.scrollOffset -= new Vector2(delta.x, delta.y);
        });

        carScrollView.RegisterCallback<PointerUpEvent>(evt =>
        {
            isDraggingCarScrollView = false;
            carScrollView.ReleasePointer(evt.pointerId);
        });
    }
}
