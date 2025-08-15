using MyVisualElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SelectCarUIScript : MonoBehaviour
{
    public static SelectCarUIScript Instance { get; private set; }

    [SerializeField] private UIDocument mainMenuUI;
    [SerializeField] private VisualTreeAsset carTemplate;
    [SerializeField] private CinemachineInputAxisController cameraInputAxisController;
    [SerializeField] private Transform carModelPlace;

    List<CarsSO> carsSOList;

    private VisualElement root;

    private ScrollView carScrollView;
    private VisualElement carRenderTexture;
    private bool isDraggingCarScrollView;
    private Vector2 lastPointerPosScrollView;
    private Vector2 lastPointerPos;

    List<VisualElement> carSelectButtons;
    private int currentSelectIndex = 0;
    private int lastSelectIndex = 0;

    private Label carNameLabel;

    private Label massLabel;
    private FillBarVisualElement maxSpeedBar;
    private FillBarVisualElement accelerationBar;
    private FillBarVisualElement brakesBar;

    private float maxSpeed = 50;
    private float maxAcceleration = 2000;
    private float maxBrakes = 3500;

    public event EventHandler<OnCarSelectedEventArgs> onCarSelected;

    public class OnCarSelectedEventArgs : EventArgs
    {
        public int carIndex;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        setOrbitalFollow(false);
        setBars();
        fillSelectCarMenu();
        addEventToButtonsInScrollView();
        dragCarRenderTexture();
        selectCar(0);

        root.Q<Button>("SelectButton").RegisterCallback<ClickEvent>((ClickEvent e) =>
        {
            lastSelectIndex = currentSelectIndex;
            onCarSelected?.Invoke(this, new OnCarSelectedEventArgs { carIndex = (int)currentSelectIndex });
            root.Q<VisualElement>("SelectCarMenu").RemoveFromClassList("animationUp");
            carScrollView.ScrollTo(carSelectButtons[currentSelectIndex]);
        });

        root.Q<Button>("ReturnButtonSelectCarMenu").RegisterCallback<ClickEvent>((ClickEvent e) =>
        {
            selectCar(lastSelectIndex);
            currentSelectIndex = lastSelectIndex;
            root.Q<VisualElement>("SelectCarMenu").RemoveFromClassList("animationUp");
        });
    }

    private void setBars()
    {
        root = mainMenuUI.rootVisualElement;

        massLabel = root.Q<Label>("MassLabel");
        maxSpeedBar = root.Q<FillBarVisualElement>("MaxSpeedBar");
        accelerationBar = root.Q<FillBarVisualElement>("AccelerationBar");
        brakesBar = root.Q<FillBarVisualElement>("BrakesBar");
    }

    private void fillSelectCarMenu()
    {

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
            lastPointerPos = Mouse.current.position.ReadValue();
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            carRenderTexture.CapturePointer(evt.pointerId);
            setOrbitalFollow(true);
        });

        carRenderTexture.RegisterCallback<PointerUpEvent>((PointerUpEvent evt) =>
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            Mouse.current.WarpCursorPosition(lastPointerPos);
            carRenderTexture.ReleasePointer(evt.pointerId);
            setOrbitalFollow(false);
        });
    }

    private void setOrbitalFollow(bool enabled)
    {
        foreach (CinemachineInputAxisController.Controller controller in cameraInputAxisController.Controllers)
        {
            controller.Enabled = enabled;
        }

    }

    private void selectCar(int index)
    {
        // Ustawiam klasy ¿eby zmieniæ kolor po zaznaczeniu
        carSelectButtons[currentSelectIndex].Q<VisualElement>("MainBorder").RemoveFromClassList("carSelectionBoxBorderSelected");
        carSelectButtons[currentSelectIndex].Q<VisualElement>("SecondBorder").RemoveFromClassList("carSelectionBoxBorderSelected");
        carSelectButtons[currentSelectIndex].Q<VisualElement>("CarNameBorder").RemoveFromClassList("carSelectionBoxBorderSelected");

        carSelectButtons[index].Q<VisualElement>("MainBorder").AddToClassList("carSelectionBoxBorderSelected");
        carSelectButtons[index].Q<VisualElement>("SecondBorder").AddToClassList("carSelectionBoxBorderSelected");
        carSelectButtons[index].Q<VisualElement>("CarNameBorder").AddToClassList("carSelectionBoxBorderSelected");

        currentSelectIndex = index;
        carScrollView.ScrollTo(carSelectButtons[index]);

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

        // Zmieniam staty w barach
        maxSpeedBar.fillPercentage = Mathf.Clamp01(carsSOList[index].maxSpeed / maxSpeed);
        accelerationBar.fillPercentage = Mathf.Clamp01(carsSOList[index].acceleration / maxAcceleration);
        brakesBar.fillPercentage = Mathf.Clamp01(carsSOList[index].braking / maxBrakes);
        massLabel.text = carsSOList[index].mass.ToString() + " kg";

        maxSpeedBar.MarkDirtyRepaint();
        accelerationBar.MarkDirtyRepaint();
        brakesBar.MarkDirtyRepaint();
    }
}
