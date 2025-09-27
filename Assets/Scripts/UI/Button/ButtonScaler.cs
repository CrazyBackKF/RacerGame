using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scale;
    [SerializeField] private float time;
    [SerializeField] private LeanTweenType easeType;
    [SerializeField] private bool ignoreTimeScale;

    private RectTransform rectTransform;
    private float currentScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.value(1, scale, time)
            .setOnUpdate((float value) =>
            {
                currentScale = value;
                rectTransform.localScale = new Vector2(value, value);
            })
            .setEase(easeType)
            .setIgnoreTimeScale(ignoreTimeScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.value(currentScale, 1, time)
            .setOnUpdate((float value) =>
            {
                rectTransform.localScale = new Vector2(value, value);
            })
            .setEase(easeType)
            .setIgnoreTimeScale(ignoreTimeScale);
    }
}
