using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class SelectCarUIScript : MonoBehaviour
{
    [SerializeField] private UIDocument mainMenuUI;
    [SerializeField] private VisualTreeAsset carTemplate;
    [SerializeField] private CinemachineOrbitalFollow cameraOrbitalFollow;
    [SerializeField] private Transform carModelPlace;

    List<CarsSO> carsSOList;

    private VisualElement root;

    private ScrollView carScrollView;
    private VisualElement carRenderTexture;
    private bool isDraggingCarScrollView;
    private Vector2 lastPointerPos;

    List<VisualElement> carSelectButtons;
    private int? currentSelectIndex = null;

    private Label carNameLabel;

    private void Start()
    {
        cameraOrbitalFollow.enabled = false;
        fillSelectCarMenu();
        addEventToButtonsInScrollView();
        dragCarRenderTexture();
    }

    private void fillSelectCarMenu()
    {
        root = mainMenuUI.rootVisualElement;

        carsSOList = GameManager.Instance.getCarsSO();
        carScrollView = root.Q<ScrollView>("CarScrollView");

        // Dodaje do listy samochody zgodnie z List¹ z CarSO w GameManager
        foreach (CarsSO car in carsSOList)
        {
            VisualElement carVisualElement = carTemplate.CloneTree();
            carVisualElement.Q<Label>("CarName").text = car.carName;
            carScrollView.Add(carVisualElement);
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
        carNameLabel = root.Q<Label>("CarNameLabel");

        carSelectButtons = carScrollView.contentContainer.Children().ToList();

        // Dodaje eventy na klikniêcie
        for (int i = 0; i < carSelectButtons.Count; i++)
        {
            int index = i;
            carSelectButtons[i].RegisterCallback<ClickEvent>((ClickEvent evt) => selectCar(index));
        }
    }

    private void dragCarRenderTexture()
    {
        // Zmieniam parametry ¿eby mo¿na by³o ogl¹daæ model auta z ró¿nych perspektyw
        carRenderTexture = root.Q<VisualElement>("CarRenderTexture");

        carRenderTexture.RegisterCallback<PointerDownEvent>((PointerDownEvent evt) =>
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            carRenderTexture.CapturePointer(evt.pointerId);
            setOrbitalFollow(true);
        });

        carRenderTexture.RegisterCallback<PointerUpEvent>((PointerUpEvent evt) =>
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            carRenderTexture.ReleasePointer(evt.pointerId);
            setOrbitalFollow(false);
        });
    }

    private void setOrbitalFollow(bool enabled)
    {
        cameraOrbitalFollow.enabled = enabled;
    }

    private void selectCar(int index)
    {
        // Ustawiam klasy ¿eby zmieniæ kolor po zaznaczeniu
        if (currentSelectIndex != null)
        {
            carSelectButtons[(int)currentSelectIndex].Q<VisualElement>("MainBorder").RemoveFromClassList("carSelectionBoxBorderSelected");
            carSelectButtons[(int)currentSelectIndex].Q<VisualElement>("SecondBorder").RemoveFromClassList("carSelectionBoxBorderSelected");
            carSelectButtons[(int)currentSelectIndex].Q<VisualElement>("CarNameBorder").RemoveFromClassList("carSelectionBoxBorderSelected");
        }

        carSelectButtons[index].Q<VisualElement>("MainBorder").AddToClassList("carSelectionBoxBorderSelected");
        carSelectButtons[index].Q<VisualElement>("SecondBorder").AddToClassList("carSelectionBoxBorderSelected");
        carSelectButtons[index].Q<VisualElement>("CarNameBorder").AddToClassList("carSelectionBoxBorderSelected");

        currentSelectIndex = index;

        // Ustawiam wartoœci w menu, takie jak w SO

        //tekst
        carNameLabel.text = carsSOList[index].carName;

        // model na odpowiednim miejscu
        foreach (Transform child in carModelPlace)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject carModel = carsSOList[index].model;

        // Wy³¹czam kolidery ¿eby nie krzycza³o w konsoli lol
        WheelCollider[] wheelColliders = carModel.GetComponentsInChildren<WheelCollider>();
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.enabled = false;
        }

        GameObject car = Instantiate(carModel, carModelPlace.position, carModel.transform.rotation, carModelPlace);
    }
}
