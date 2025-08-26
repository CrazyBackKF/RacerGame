using MyVisualElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SelectMapUIScript : MonoBehaviour
{
    public static SelectMapUIScript Instance { get; private set; }

    [SerializeField] private UIDocument mainMenuUI;
    [SerializeField] private VisualTreeAsset mapTemplate;

    List<MapsSO> mapsSOList;

    private VisualElement root;

    private VisualElement mapsContainer;

    List<VisualElement> mapsSelectButtons;
    private int currentSelectIndex = 0;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        fillSelectMapMenu();
        addEventToButtonsInScrollView();
        selectMap(0);

        root.Q<Button>("PlayButtonSelectMenu").RegisterCallback<ClickEvent>((ClickEvent e) =>
        {
            root.Q<VisualElement>("SelectMapMenu").RemoveFromClassList("animationUp");
            SceneLoader.Instance.loadLevel(mapsSOList[currentSelectIndex].sceneIndex);
        });

        root.Q<Button>("ReturnButtonSelectMapMenu").RegisterCallback<ClickEvent>((ClickEvent e) =>
        {
            root.Q<VisualElement>("SelectMapMenu").RemoveFromClassList("animationUp");
        });
    }

    private void fillSelectMapMenu()
    {
        root = mainMenuUI.rootVisualElement;

        mapsSOList = GameManager.Instance.getMapsSO();
        mapsContainer = root.Q<VisualElement>("MapsContainer");

        // Dodaje do listy mapy zgodnie z List¹ z MapSO w GameManager
        foreach (MapsSO map in mapsSOList)
        {
            VisualElement mapVisualElement = mapTemplate.CloneTree();
            mapVisualElement.Q<Label>("MapName").text = map.mapName;
            mapsContainer.Add(mapVisualElement);
        }

        //carScrollView.RegisterCallback<PointerDownEvent>(evt =>
        //{
        //    isDraggingCarScrollView = true;
        //    lastPointerPos = evt.position;
        //    carScrollView.CapturePointer(evt.pointerId);
        //});

        //carScrollView.RegisterCallback<PointerMoveEvent>(evt =>
        //{
        //    if (!isDraggingCarScrollView) return;

        //    Vector2 delta = new Vector2(evt.position.x, evt.position.y) - lastPointerPos;
        //    lastPointerPos = evt.position;

        //    carScrollView.scrollOffset -= delta;
        //});

        //carScrollView.RegisterCallback<PointerUpEvent>(evt =>
        //{
        //    isDraggingCarScrollView = false;
        //    carScrollView.ReleasePointer(evt.pointerId);
        //});
    }

    private void addEventToButtonsInScrollView()
    {
        mapsSelectButtons = mapsContainer.Children().ToList();

        // Dodaje eventy na klikniêcie
        for (int i = 0; i < mapsSelectButtons.Count; i++)
        {
            int index = i;
            mapsSelectButtons[i].RegisterCallback<ClickEvent>((ClickEvent evt) => selectMap(index));
        }
    }

    private void selectMap(int index)
    {
        mapsSelectButtons[currentSelectIndex].Q<VisualElement>("MainBorder").RemoveFromClassList("mapSelectionBoxBorderSelected");
        mapsSelectButtons[currentSelectIndex].Q<VisualElement>("SecondBorder").RemoveFromClassList("mapSelectionBoxBorderSelected");
        mapsSelectButtons[currentSelectIndex].Q<VisualElement>("MapNameBorder").RemoveFromClassList("mapSelectionBoxBorderSelected");

        mapsSelectButtons[index].Q<VisualElement>("MainBorder").AddToClassList("mapSelectionBoxBorderSelected");
        mapsSelectButtons[index].Q<VisualElement>("SecondBorder").AddToClassList("mapSelectionBoxBorderSelected");
        mapsSelectButtons[index].Q<VisualElement>("MapNameBorder").AddToClassList("mapSelectionBoxBorderSelected");

        currentSelectIndex = index;
    }
}
